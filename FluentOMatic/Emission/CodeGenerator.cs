using FluentOMatic.States;
using Microsoft.CSharp;
using System;
using System.Linq;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
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

		private static Dictionary<string, string> _builtInTypes = new Dictionary<string, string>
		{
			{ "bool", "Boolean" },
			{ "byte", "Byte" },
			{ "sbyte", "SByte" },
			{ "char", "Char" },
			{ "decimal", "Decimal" },
			{ "double", "Double" },
			{ "float", "Single" },
			{ "int", "Int32" },
			{ "uint", "UInt32" },
			{ "long", "Int64" },
			{ "ulong", "UInt64" },
			{ "object", "Object" },
			{ "short", "Int16" },
			{ "ushort", "UInt16" },
			{ "string", "String" },
		};

		private static string GetTypeName(string type)
		{
			string typeName;
			return _builtInTypes.TryGetValue(type, out typeName) ? typeName : type;
		}

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
			var stateType = new CodeTypeDeclaration(GenerateName("_{0}{1}Syntax", entryState.Name, usedNames))
			{
			};

			addType(stateType);


			var states = new HashSet<State>();
			FlattenStateTree(entryState, states);
			foreach (var state in states)
			{
				var interfaceType = new CodeTypeDeclaration(GenerateName("{0}{1}Syntax", state.Name, usedNames))
				{
					Attributes = MemberAttributes.Public,
					TypeAttributes = TypeAttributes.Interface | TypeAttributes.Public,
				};
				addType(interfaceType);

				stateType.BaseTypes.Add(interfaceType.Name);

				var syntaxType = new CodeTypeDeclaration(GenerateName("{0}{1}Data", state.Name, usedNames))
				{
					Attributes = MemberAttributes.Final | MemberAttributes.Public,
					TypeAttributes = TypeAttributes.Sealed | TypeAttributes.Public,
				};
				addType(syntaxType);


				//var constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
				//syntaxType.Members.Add(constructor);

				//foreach (var parameter in state.Parameters)
				//{
				//	syntaxType.Members.Add(new CodeMemberField(
				//		GetTypeName(parameter.Type),
				//		parameter.Name
				//	) { Attributes = MemberAttributes.Public });

				//	constructor.Parameters.Add(new CodeParameterDeclarationExpression(
				//		GetTypeName(parameter.Type),
				//		parameter.Name
				//	));

				//	constructor.Statements.Add(new CodeAssignStatement(
				//		new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), parameter.Name),
				//		new CodeSnippetExpression(parameter.Name)
				//	));
				//}

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

					//constructor.Parameters.Add(new CodeParameterDeclarationExpression(
					//	string.Format("Func<{0}, {1}>", innerStateData.InterfaceType.Name, _syntaxEnd),
					//	_inner
					//));

					//constructor.Statements.Add(new CodeAssignStatement(
					//	new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _inner),
					//	new CodeObjectCreateExpression(innerStateData.SyntaxType.Name)
					//));

					//constructor.Statements.Add(new CodeExpressionStatement(
					//	new CodeDelegateInvokeExpression(
					//		new CodeSnippetExpression(_inner),
					//		new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), _inner)
					//	)
					//));
				}

				_syntaxTypes.Add(state, new StateData { SyntaxType = syntaxType, InterfaceType = interfaceType });
			}

			foreach (var state in states)
			{
				var stateData = _syntaxTypes[state];

				foreach (var nextState in state.NextStates)
				{
					var nextStateData = _syntaxTypes[nextState];

					var allowMultiple = nextState.NextStates.Contains(nextState);

					CodeMemberField nextStateField;
					if (allowMultiple)
					{
						nextStateField = new CodeMemberField(
							new CodeTypeReference(string.Format("System.Collections.Generic.IList<{0}>", nextStateData.SyntaxType.Name)),
							nextState.Name
						) {
							Attributes = MemberAttributes.Public,
							InitExpression = new CodeObjectCreateExpression(
								string.Format("System.Collections.Generic.List<{0}>", nextStateData.SyntaxType.Name)
							)
						};

						

						//var nextStateProperty = new CodeMemberProperty
						//{
						//	Name = nextState.Name,
						//	HasGet = true,
						//	Type = new CodeTypeReference(string.Format("System.Collections.Generic.IEnumerable<{0}>", nextStateData.SyntaxType.Name)),
						//	Attributes = MemberAttributes.Public | MemberAttributes.Final,
						//};

						//nextStateProperty.GetStatements.Add(new CodeMethodReturnStatement(
						//	new CodeFieldReferenceExpression(
						//		new CodeThisReferenceExpression(),
						//		nextStateField.Name
						//	)
						//));

						//stateData.SyntaxType.Members.Add(nextStateProperty);
					}
					else
					{
						nextStateField = new CodeMemberField(
							nextStateData.SyntaxType.Name,
							nextState.Name
						) { Attributes = MemberAttributes.Public };
					}

					stateType.Members.Add(nextStateField);


					//if (state == nextState)
					//{
					//	var enumerableType = string.Format("System.Collections.Generic.IEnumerable<{0}>", stateData.SyntaxType.Name);
					//	stateData.SyntaxType.BaseTypes.Add(enumerableType);

					//	var enumeratorType = string.Format("System.Collections.Generic.IEnumerator<{0}>", stateData.SyntaxType.Name);
					//	var getEnumeratorMethod = new CodeMemberMethod
					//	{
					//		Name = "GetEnumerator",
					//		PrivateImplementationType = new CodeTypeReference(enumerableType),
					//		ReturnType = new CodeTypeReference(enumeratorType),
					//	};

					//	getEnumeratorMethod.Statements.Add(
					//		new CodeIterationStatement(
					//			new CodeVariableDeclarationStatement(stateData.SyntaxType.Name, "current", new CodeThisReferenceExpression()),
					//			new CodeBinaryOperatorExpression(
					//				new CodeVariableReferenceExpression(_current),
					//				CodeBinaryOperatorType.IdentityInequality,
					//				new CodePrimitiveExpression(null)
					//			),
					//			new CodeAssignStatement(
					//				new CodeVariableReferenceExpression(_current),
					//				new CodeFieldReferenceExpression(
					//					new CodeVariableReferenceExpression(_current),
					//					nextStateField.Name
					//				)
					//			),
					//			new CodeExpressionStatement(new CodeSnippetExpression("yield return " + _current))
					//		)
					//	);

					//	stateData.SyntaxType.Members.Add(getEnumeratorMethod);

					//	var legacyGetEnumeratorMethod = new CodeMemberMethod
					//	{
					//		Name = "GetEnumerator",
					//		PrivateImplementationType = new CodeTypeReference("System.Collections.IEnumerable"),
					//		ReturnType = new CodeTypeReference("System.Collections.IEnumerator"),
					//	};

					//	legacyGetEnumeratorMethod.Statements.Add(
					//		new CodeMethodReturnStatement(
					//			new CodeMethodInvokeExpression(
					//				new CodeCastExpression(
					//					enumerableType,
					//					new CodeThisReferenceExpression()
					//				),
					//				"GetEnumerator"
					//			)
					//		)
					//	);

					//	stateData.SyntaxType.Members.Add(legacyGetEnumeratorMethod);
					//}




					//var stateTransitionMethod = new CodeMemberMethod
					//{
					//	Name = nextState.Name,
					//	PrivateImplementationType = new CodeTypeReference(stateData.InterfaceType.Name),
					//};

					//var stateTransitionInterfaceMethod = new CodeMemberMethod
					//{
					//	Name = nextState.Name,
					//};

					//var constructorParameters = new List<CodeExpression>();
					//foreach (var parameter in nextState.Parameters)
					//{
					//	var methodParameter = new CodeParameterDeclarationExpression(
					//		GetTypeName(parameter.Type),
					//		parameter.Name
					//	);

					//	stateTransitionMethod.Parameters.Add(methodParameter);
					//	stateTransitionInterfaceMethod.Parameters.Add(methodParameter);

					//	constructorParameters.Add(new CodeSnippetExpression(parameter.Name));
					//}

					//if (nextState.InnerState != null)
					//{
					//	var innerStateData = _syntaxTypes[nextState.InnerState];

					//	var methodParameter = new CodeParameterDeclarationExpression(
					//		string.Format("Func<{0}, {1}>", innerStateData.InterfaceType.Name, _syntaxEnd),
					//		_inner
					//	);

					//	stateTransitionMethod.Parameters.Add(methodParameter);
					//	stateTransitionInterfaceMethod.Parameters.Add(methodParameter);

					//	constructorParameters.Add(new CodeSnippetExpression(_inner));
					//}

					//stateTransitionMethod.Statements.Add(
					//	new CodeAssignStatement(
					//		new CodeFieldReferenceExpression(
					//			new CodeThisReferenceExpression(),
					//			nextStateField.Name
					//		),
					//		new CodeObjectCreateExpression(
					//			nextStateData.SyntaxType.Name,
					//			constructorParameters.ToArray()
					//		)
					//	)
					//);

					//stateTransitionMethod.Statements.Add(
					//	new CodeMethodReturnStatement(
					//		new CodeFieldReferenceExpression(
					//			new CodeThisReferenceExpression(),
					//			nextStateField.Name
					//		)
					//	)
					//);

					//stateData.SyntaxType.Members.Add(stateTransitionMethod);
					//stateTransitionMethod.ReturnType = new CodeTypeReference(nextStateData.InterfaceType.Name);

					//stateData.InterfaceType.Members.Add(stateTransitionInterfaceMethod);
					//stateTransitionInterfaceMethod.ReturnType = new CodeTypeReference(nextStateData.InterfaceType.Name);
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
