using FluentOMatic.States;
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

		public void GenerateCode(State rootState, TextWriter output)
		{
			_syntaxTypes.Clear();

			var code = new CodeCompileUnit();
			var ns = new CodeNamespace("Kebas");
			code.Namespaces.Add(ns);
			ns.Imports.Add(new CodeNamespaceImport("System"));

			ns.Types.Add(new CodeTypeDeclaration(_syntaxEnd)
			{
				Attributes = MemberAttributes.Public,
				TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public,
			});

			GenerateCode(t => ns.Types.Add(t), rootState, new HashSet<string>());

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

		private void GenerateCode(Action<CodeTypeDeclaration> addType, State entryState, HashSet<string> usedNames)
		{
			var stateType = new CodeTypeDeclaration(GenerateName("{0}{1}Builder", entryState.Name, usedNames))
			{
			};

			addType(stateType);


			var states = new HashSet<State>();
			FlattenStateTree(entryState, states);
			foreach (var state in states)
			{
				// Create syntax interface type
				var interfaceType = new CodeTypeDeclaration(GenerateName("{0}{1}Syntax", state.Name, usedNames))
				{
					Attributes = MemberAttributes.Public,
					TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public,
				};
				addType(interfaceType);

				stateType.BaseTypes.Add(interfaceType.Name);

				var syntaxType = stateType;

				// Create data container
				if (!state.IsRoot)
				{
					syntaxType = new CodeTypeDeclaration(GenerateName("{0}{1}Data", state.Name, usedNames))
					{
						Attributes = MemberAttributes.Final | MemberAttributes.Public,
						TypeAttributes = TypeAttributes.Sealed | TypeAttributes.Public,
					};
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
						nextStateField = new CodeMemberField(
							new CodeTypeReference(string.Format("System.Collections.Generic.IList<{0}>", syntaxType.Name)),
							state.Name
						)
						{
							Attributes = MemberAttributes.Public,
							InitExpression = new CodeObjectCreateExpression(
								string.Format("System.Collections.Generic.List<{0}>", syntaxType.Name)
							)
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
							GenerateCode(addType, state.InnerState, usedNames);
							innerStateData = _syntaxTypes[state.InnerState];
						}

						syntaxType.Members.Add(new CodeMemberField(
							innerStateData.SyntaxType.Name,
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
					stateTransitionInterfaceMethod.ReturnType = new CodeTypeReference(nextStateData.InterfaceType.Name);

					// Add state transition method to type
					var stateTransitionMethod = new CodeMemberMethod
					{
						Name = nextState.Name,
						PrivateImplementationType = new CodeTypeReference(stateData.InterfaceType.Name),
					};

					stateType.Members.Add(stateTransitionMethod);
					stateTransitionMethod.ReturnType = new CodeTypeReference(nextStateData.InterfaceType.Name);

					var variable = new CodeVariableDeclarationStatement(
						nextStateData.SyntaxType.Name,
						"_" + nextState.Name,
						new CodeObjectCreateExpression(nextStateData.SyntaxType.Name)
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

						var methodParameter = new CodeParameterDeclarationExpression(
							string.Format("Func<{0}, {1}>", innerStateData.InterfaceType.Name, _syntaxEnd),
							_inner
						);

						stateTransitionMethod.Parameters.Add(methodParameter);
						stateTransitionInterfaceMethod.Parameters.Add(methodParameter);

						stateTransitionMethod.Statements.Add(
							new CodeAssignStatement(
								new CodeFieldReferenceExpression(
									new CodeSnippetExpression("_" + nextState.Name),
									_inner
								),
								new CodeObjectCreateExpression(innerStateData.SyntaxType.Name)
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

				if (state.NextStates.All(s => s.IsOptional))
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
