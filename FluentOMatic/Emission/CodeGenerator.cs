//    Copyright 2012 Antoine Aubry
//    
//    This file is part of FluentOMatic.
//
//    FluentOMatic is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    FluentOMatic is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with FluentOMatic.  If not, see <http://www.gnu.org/licenses/>.

using FluentOMatic.States;
using FluentOMatic.Syntax;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FluentOMatic.Emission
{
	public class CodeGenerator
	{
		private class StateData
		{
			public CodeTypeDeclaration SyntaxType;
			public CodeTypeDeclaration InterfaceType;
		}

		private readonly Dictionary<State, StateData> _syntaxTypes = new Dictionary<State, StateData>();
		private const string _syntaxEnd = "_SyntaxEnd";
		private const string _inner = "inner";
		private const string _current = "current";

		public void GenerateCode(State rootState, TextWriter output, string namespaceName, UsingList usings, string[] genericArguments)
		{
			_syntaxTypes.Clear();

			var code = new CodeCompileUnit();
			var ns = new CodeNamespace(namespaceName);
			code.Namespaces.Add(ns);
			ns.Imports.Add(new CodeNamespaceImport("System"));

			foreach (var u in usings)
			{
				ns.Imports.Add(new CodeNamespaceImport(u.Namespace));
			}

			GenerateCode(t => ns.Types.Add(t), rootState, new HashSet<string>(), genericArguments);

			ns.Types.Add(new CodeTypeDeclaration(_syntaxEnd)
			{
				Attributes = MemberAttributes.Public,
				TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public,
			});

			var codeProvider = new CSharpCodeProvider();
			codeProvider.GenerateCodeFromCompileUnit(code, output, new CodeGeneratorOptions
			{
				BracingStyle = "C",
				IndentString = "\t",
			});
		}

		private string GenerateName(string template, string baseName, HashSet<string> usedNames)
		{
			if (!template.Contains("{0}") || !template.Contains("{1}"))
			{
				throw new ArgumentException("template");
			}

			var name = string.Format(template, baseName, "");
			var counter = 0;
			while (!usedNames.Add(name))
			{
				name = string.Format(template, baseName, ++counter);
			}
			return name;
		}

		private CodeTypeDeclaration AddGenericArguments(CodeTypeDeclaration type, string[] genericArguments)
		{
			foreach (var argument in genericArguments)
			{
				type.TypeParameters.Add(argument);
			}
			return type;
		}

		private CodeTypeReference AddGenericArguments(CodeTypeReference type, string[] genericArguments)
		{
			foreach (var argument in genericArguments)
			{
				type.TypeArguments.Add(argument);
			}
			return type;
		}

		private CodeTypeReference AddGenericArguments(string typeName, string[] genericArguments)
		{
			var type = new CodeTypeReference(typeName);
			foreach (var argument in genericArguments)
			{
				type.TypeArguments.Add(argument);
			}
			return type;
		}

		private void GenerateCode(Action<CodeTypeDeclaration> addType, State entryState, HashSet<string> usedNames, string[] genericArguments)
		{
			var stateType = AddGenericArguments(new CodeTypeDeclaration(GenerateName("{0}{1}Builder", entryState.Name, usedNames))
			{
			}, genericArguments);

			addType(stateType);


			var states = new HashSet<State>();
			FlattenStateTree(entryState, states);
			foreach (var state in states)
			{
				// Create syntax interface type
				var interfaceType = AddGenericArguments(new CodeTypeDeclaration(GenerateName("{0}{1}Syntax", state.Name, usedNames))
				{
					Attributes = MemberAttributes.Public,
					TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public,
				}, genericArguments);

				addType(interfaceType);

				stateType.BaseTypes.Add(AddGenericArguments(interfaceType.Name, genericArguments));

				var syntaxType = stateType;

				// Create data container
				if (!state.IsRoot)
				{
					syntaxType = AddGenericArguments(new CodeTypeDeclaration(GenerateName("{0}{1}Data", state.Name, usedNames))
					{
						Attributes = MemberAttributes.Final | MemberAttributes.Public,
						TypeAttributes = TypeAttributes.Sealed | TypeAttributes.Public,
					}, genericArguments);

					addType(syntaxType);

					foreach (var parameter in state.Parameters)
					{
						syntaxType.Members.Add(
							new CodeMemberField(
								parameter.Type,
								parameter.Name
							)
							{
								Attributes = MemberAttributes.Public
							}
						);
					}

					// Add field for each subsequent state
					var allowMultiple = state.NextStates.Contains(state);
					CodeMemberField nextStateField;
					if (allowMultiple)
					{
						var ilistType = new CodeTypeReference("System.Collections.Generic.IList");
						ilistType.TypeArguments.Add(AddGenericArguments(syntaxType.Name, genericArguments));

						var listType = new CodeTypeReference("System.Collections.Generic.List");
						listType.TypeArguments.Add(AddGenericArguments(syntaxType.Name, genericArguments));

						nextStateField = new CodeMemberField(ilistType, state.Name)
						{
							Attributes = MemberAttributes.Public,
							InitExpression = new CodeObjectCreateExpression(listType)
						};
					}
					else
					{
						nextStateField = new CodeMemberField(
							syntaxType.Name,
							state.Name
						) { Attributes = MemberAttributes.Public };
					}

					stateType.Members.Add(nextStateField);

					if (state.InnerState != null)
					{
						StateData innerStateData;
						if (!_syntaxTypes.TryGetValue(state.InnerState, out innerStateData))
						{
							GenerateCode(addType, state.InnerState, usedNames, genericArguments);
							innerStateData = _syntaxTypes[state.InnerState];
						}

						syntaxType.Members.Add(new CodeMemberField(
							AddGenericArguments(innerStateData.SyntaxType.Name, genericArguments),
							_inner
						) { Attributes = MemberAttributes.Public });
					}
				}

				_syntaxTypes.Add(state, new StateData { SyntaxType = syntaxType, InterfaceType = interfaceType });
			}

			foreach (var state in states)
			{
				var stateData = _syntaxTypes[state];

				foreach (var nextState in state.NextStates)
				{
					var nextStateData = _syntaxTypes[nextState];

					// Add state transition method to interface
					var stateTransitionInterfaceMethod = new CodeMemberMethod
					{
						Name = nextState.Name,
					};

					stateData.InterfaceType.Members.Add(stateTransitionInterfaceMethod);
					stateTransitionInterfaceMethod.ReturnType = AddGenericArguments(nextStateData.InterfaceType.Name, genericArguments);

					// Add state transition method to type
					var stateTransitionMethod = new CodeMemberMethod
					{
						Name = nextState.Name,
						PrivateImplementationType = AddGenericArguments(stateData.InterfaceType.Name, genericArguments),
					};

					stateType.Members.Add(stateTransitionMethod);
					stateTransitionMethod.ReturnType = AddGenericArguments(nextStateData.InterfaceType.Name, genericArguments);

					var variable = new CodeVariableDeclarationStatement(
						AddGenericArguments(nextStateData.SyntaxType.Name, genericArguments),
						"_" + nextState.Name,
						new CodeObjectCreateExpression(AddGenericArguments(nextStateData.SyntaxType.Name, genericArguments))
					);
					stateTransitionMethod.Statements.Add(variable);

					foreach (var parameter in nextState.Parameters)
					{
						var methodParameter = new CodeParameterDeclarationExpression(
							parameter.Type,
							parameter.Name
						);

						stateTransitionMethod.Parameters.Add(methodParameter);
						stateTransitionInterfaceMethod.Parameters.Add(methodParameter);

						stateTransitionMethod.Statements.Add(
							new CodeAssignStatement(
								new CodeFieldReferenceExpression(
									new CodeVariableReferenceExpression(variable.Name),
									parameter.Name
								),
								new CodeSnippetExpression(methodParameter.Name)
							)
						);
					}

					var allowMultiple = nextState.NextStates.Contains(nextState);
					if (allowMultiple)
					{
						stateTransitionMethod.Statements.Add(
							new CodeMethodInvokeExpression(
								new CodeFieldReferenceExpression(
									new CodeThisReferenceExpression(),
									nextState.Name
								),
								"Add",
								new CodeSnippetExpression(variable.Name)
							)
						);
					}
					else
					{
						stateTransitionMethod.Statements.Add(
							new CodeAssignStatement(
								new CodeFieldReferenceExpression(
									new CodeThisReferenceExpression(),
									nextState.Name
								),
								new CodeSnippetExpression(variable.Name)
							)
						);
					}

					// Add parameter for inner state
					if (nextState.InnerState != null)
					{
						var innerStateData = _syntaxTypes[nextState.InnerState];

						var methodParameterType = new CodeTypeReference("Func");
						methodParameterType.TypeArguments.Add(AddGenericArguments(innerStateData.InterfaceType.Name, genericArguments));
						methodParameterType.TypeArguments.Add(_syntaxEnd);

						var methodParameter = new CodeParameterDeclarationExpression(methodParameterType, _inner);

						stateTransitionMethod.Parameters.Add(methodParameter);
						stateTransitionInterfaceMethod.Parameters.Add(methodParameter);

						stateTransitionMethod.Statements.Add(
							new CodeAssignStatement(
								new CodeFieldReferenceExpression(
									new CodeSnippetExpression("_" + nextState.Name),
									_inner
								),
								new CodeObjectCreateExpression(AddGenericArguments(innerStateData.SyntaxType.Name, genericArguments))
							)
						);

						stateTransitionMethod.Statements.Add(
							new CodeDelegateInvokeExpression(
								new CodeSnippetExpression(methodParameter.Name),
								new CodeFieldReferenceExpression(
									new CodeSnippetExpression("_" + nextState.Name),
									_inner
								)
							)
						);
					}

					stateTransitionMethod.Statements.Add(
						new CodeMethodReturnStatement(
							new CodeThisReferenceExpression()
						)
					);
				}

				if (state.IsTerminal)
				{
					stateData.InterfaceType.BaseTypes.Add(_syntaxEnd);
				}
			}
		}

		private void FlattenStateTree(State entryState, HashSet<State> visitedStates)
		{
			if (visitedStates.Add(entryState))
			{
				foreach (var state in entryState.NextStates)
				{
					FlattenStateTree(state, visitedStates);
				}
			}
		}
	}
}
