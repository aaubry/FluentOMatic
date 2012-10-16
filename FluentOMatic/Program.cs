using FluentOMatic.Emission;
using FluentOMatic.States;
using System;
using System.IO;
using System.Linq;

namespace FluentOMatic
{
	class Program
	{
		static void Main(string[] args)
		{
			var parser = new Parser(new Scanner("test.txt"));
			parser.Parse();

			var graphBuilder = new StateGraphBuilder();
			var states = graphBuilder.BuildGraph(parser.Syntax);

			//// ----
			//int index = 0;
			//var stateNames = states.ToDictionary(s => s, s => "S" + index++);
			//Console.WriteLine("digraph {");
			//foreach (var state in states)
			//{
			//	Console.WriteLine("  {0} [label=\"{1}\"];", stateNames[state], state.Name);

			//	foreach (var nextState in state.NextStates)
			//	{
			//		Console.WriteLine("  {0} -> {1};", stateNames[state], stateNames[nextState]);
			//	}
			//}
			//Console.WriteLine("}");
			//// ----

			using (var output = File.CreateText(@"D:\Work\FluentOMatic\ConsoleApplication1\output.cs"))
			{
				var generator = new CodeGenerator();
				generator.GenerateCode(states.First(), output);
			}
		}
	}

	//public class EmitBuilderCodeVisitor
	//{
	//	private readonly TextWriter _output;

	//	public EmitBuilderCodeVisitor(TextWriter output)
	//	{
	//		_output = output;
	//	}

	//	private CodeTypeDeclaration _rootType;
	//	private CodeTypeDeclaration _currentSyntaxType;
	//	private CodeTypeDeclaration _currentStateType;
	//	private CodeMemberMethod _currentMethod;

	//	private int syntaxCounter;
	//	private int stateCounter;

	//	public void Visit(FluentSyntax syntax)
	//	{
	//		var code = new CodeCompileUnit();
	//		var ns = new CodeNamespace("Kebas");
	//		code.Namespaces.Add(ns);

	//		_currentStateType = _rootType = _currentSyntaxType = new CodeTypeDeclaration(syntax.Name);
	//		ns.Types.Add(_currentSyntaxType);

	//		//syntax.Operations.Accept(this);

	//		var codeProvider = new CSharpCodeProvider();
	//		codeProvider.GenerateCodeFromCompileUnit(code, _output, new CodeGeneratorOptions());
	//	}

	//	public void Visit(Operation operation)
	//	{
	//		_currentMethod = new CodeMemberMethod
	//		{
	//			Name = operation.Name,
	//			Attributes = MemberAttributes.Public | MemberAttributes.Final,
	//		};
	//		_currentSyntaxType.Members.Add(_currentMethod);

	//		switch (operation.Multiplicity)
	//		{
	//			case Multiplicity.One:
	//			case Multiplicity.ZeroOrOne:
	//				_currentSyntaxType = new CodeTypeDeclaration("X" + syntaxCounter++);
	//				_rootType.Members.Add(_currentSyntaxType);

	//				var ctor = new CodeConstructor { Attributes = MemberAttributes.Public };
	//				ctor.Parameters.Add(new CodeParameterDeclarationExpression(_currentStateType.Name, "state"));
	//				_currentSyntaxType.Members.Add(ctor);

	//				_currentSyntaxType.Members.Add(new CodeMemberField(_currentStateType.Name, "_state"));

	//				ctor.Statements.Add(new CodeAssignStatement(
	//					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_state"),
	//					new CodeSnippetExpression("state")));

	//				break;
	//		}

	//		_currentMethod.ReturnType = new CodeTypeReference(_currentSyntaxType.Name);

	//		var previousStateType = _currentStateType;
	//		switch (operation.Multiplicity)
	//		{
	//			case Multiplicity.One:
	//			case Multiplicity.ZeroOrOne:
	//				_currentMethod.Statements.Add(new CodeVariableDeclarationStatement(_currentStateType.Name, "myState",
	//					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_state")));
	//				break;

	//			case Multiplicity.OneOrMany:
	//			case Multiplicity.ZeroOrMany:
	//				_currentStateType = new CodeTypeDeclaration("S" + stateCounter++);
	//				_rootType.Members.Add(_currentStateType);

	//				var collectionType = string.Format("System.Collections.Generic<{0}>", _currentStateType.Name);
	//				var field = new CodeMemberField(collectionType, operation.Name);
	//				field.InitExpression = new CodeObjectCreateExpression(collectionType);
	//				previousStateType.Members.Add(field);

	//				_currentMethod.Statements.Add(new CodeVariableDeclarationStatement(_currentStateType.Name, "myState",
	//					new CodeObjectCreateExpression(_currentStateType.Name)));

	//				_currentMethod.Statements.Add(
	//					new CodeMethodInvokeExpression(
	//						new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(
	//							new CodeThisReferenceExpression(), "_state"), field.Name),
	//							"Add",
	//							new CodeSnippetExpression("myState")));

	//				break;
	//		}

	//		//operation.Parameters.Accept(this);
	//		_currentMethod.Statements.Add(new CodeMethodReturnStatement(
	//			new CodeObjectCreateExpression(_currentMethod.ReturnType,
	//				new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_state"))
	//		));

	//		previousStateType = _currentStateType;
	//		var previousSyntaxType = _currentSyntaxType;

	//		//operation.Operations.Accept(this);

	//		_currentStateType = previousStateType;
	//		_currentSyntaxType = previousSyntaxType;
	//	}

	//	public void Visit(Parameter parameter)
	//	{
	//		_currentMethod.Parameters.Add(new CodeParameterDeclarationExpression(parameter.Type, parameter.Name));
	//		_currentStateType.Members.Add(new CodeMemberField(parameter.Type, parameter.Name));
	//		_currentMethod.Statements.Add(new CodeAssignStatement(
	//			new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("myState"), parameter.Name),
	//			new CodeSnippetExpression(parameter.Name)));
	//	}
	//}

	//public class OsdBuilder
	//{
	//	public class OpenSearchProject_State
	//	{
	//		public string name;
	//		public List<OpenSearchProject_Described> Described = new List<OpenSearchProject_Described>();
	//	}

	//	public X1 OpenSearchProject()
	//	{
	//		return new X1(new OpenSearchProject_State());
	//	}

	//	public sealed class X1
	//	{
	//		private OpenSearchProject_State openSearchProject_State;

	//		public X1(OpenSearchProject_State openSearchProject_State)
	//		{
	//			// TODO: Complete member initialization
	//			this.openSearchProject_State = openSearchProject_State;
	//		}

	//		public X2 Named(string name)
	//		{
	//			openSearchProject_State.name = name;

	//			return new X2(openSearchProject_State);
	//		}
	//	}

	//	public class OpenSearchProject_Described
	//	{
	//		public string lang;
	//		public string description;
	//	}

	//	public sealed class X2
	//	{
	//		private OpenSearchProject_State openSearchProject_State;

	//		public X2(OpenSearchProject_State openSearchProject_State)
	//		{
	//			// TODO: Complete member initialization
	//			this.openSearchProject_State = openSearchProject_State;
	//		}

	//		public X3 Described(string lang, string description)
	//		{
	//			openSearchProject_State.Described.Add(new OpenSearchProject_Described
	//			{
	//				lang = lang,
	//				description = description,
	//			});

	//			return new X3(openSearchProject_State);
	//		}
	//	}

	//	public sealed class X3
	//	{
	//		private OpenSearchProject_State openSearchProject_State;

	//		public X3(OpenSearchProject_State openSearchProject_State)
	//		{
	//			// TODO: Complete member initialization
	//			this.openSearchProject_State = openSearchProject_State;
	//		}
	//	}
	//}
}
