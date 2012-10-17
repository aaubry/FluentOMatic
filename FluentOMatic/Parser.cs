
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
//    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

using System;
using SOperation = FluentOMatic.Syntax.Operation;
using SOperationList = FluentOMatic.Syntax.OperationList;
using SParameter = FluentOMatic.Syntax.Parameter;
using SParameterList = FluentOMatic.Syntax.ParameterList;
using SMultiplicity = FluentOMatic.Syntax.Multiplicity;
using FluentSyntax = FluentOMatic.Syntax.FluentSyntax;

namespace FluentOMatic {



public class Parser {
	public const int _EOF = 0;
	public const int _dot = 1;
	public const int _ident = 2;
	public const int _number = 3;
	public const int _openParen = 4;
	public const int _closeParen = 5;
	public const int _zeroOrMany = 6;
	public const int _zeroOrOne = 7;
	public const int _oneOrMany = 8;
	public const int _parameterSep = 9;
	public const int maxT = 11;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

	public FluentSyntax Syntax { get; private set; }



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void FluentOMatic() {
		Syntax = new FluentSyntax(); 
		string syntaxName; 
		SyntaxName(out syntaxName);
		Syntax.Name = syntaxName; 
		SOperationList operations; 
		OperationList(out operations);
		Syntax.Operations.AddRange(operations); 
	}

	void SyntaxName(out string name) {
		Expect(10);
		Expect(2);
		name = t.val; 
	}

	void OperationList(out SOperationList result) {
		result = new SOperationList(); 
		SOperation operation; 
		Operation(out operation);
		result.Add(operation); 
		while (la.kind == 1) {
			Operation(out operation);
			result.Add(operation); 
		}
	}

	void Operation(out SOperation result) {
		result = new SOperation(); 
		Expect(1);
		Expect(2);
		result.Name = t.val; 
		Expect(4);
		if (la.kind == 2) {
			SParameterList parameters; 
			ParameterList(out parameters);
			result.Parameters.AddRange(parameters); 
		}
		if (la.kind == 1) {
			SOperationList operations; 
			OperationList(out operations);
			result.Operations.AddRange(operations); 
		}
		Expect(5);
		if (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 7) {
				Get();
				result.Multiplicity = SMultiplicity.ZeroOrOne; 
			} else if (la.kind == 6) {
				Get();
				result.Multiplicity = SMultiplicity.ZeroOrMany; 
			} else {
				Get();
				result.Multiplicity = SMultiplicity.OneOrMany; 
			}
		}
	}

	void ParameterList(out SParameterList result) {
		result = new SParameterList(); 
		SParameter parameter; 
		Parameter(out parameter);
		result.Add(parameter); 
		while (la.kind == 9) {
			Get();
			Parameter(out parameter);
			result.Add(parameter); 
		}
	}

	void Parameter(out SParameter result) {
		result = new SParameter(); 
		Expect(2);
		result.Type = t.val; 
		Expect(2);
		result.Name = t.val; 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		FluentOMatic();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "dot expected"; break;
			case 2: s = "ident expected"; break;
			case 3: s = "number expected"; break;
			case 4: s = "openParen expected"; break;
			case 5: s = "closeParen expected"; break;
			case 6: s = "zeroOrMany expected"; break;
			case 7: s = "zeroOrOne expected"; break;
			case 8: s = "oneOrMany expected"; break;
			case 9: s = "parameterSep expected"; break;
			case 10: s = "\"syntax\" expected"; break;
			case 11: s = "??? expected"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}