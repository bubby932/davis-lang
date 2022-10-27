using System;
using System.Collections.Generic;
using Davis.IR;

namespace Davis.SyntaxTree
{
	public enum TokenType
	{
		// Preprocessor
		Pound, OpenAngleBracket, CloseAngleBracket,
		PreprocessorDirective,

		// Literals
		Identifier, Literal,
		QuoteDouble, QuoteSingle,

		// Keywords
		While, If, Else, Var, Sizeof, Assembly, Function, Type,

		// Pointers
		Ampersand,

		// Operators
		Plus, Minus, Star, Slash, Percent,

		// Assignment
		Equals, Stackalloc, Heapalloc,

		// Expressions
		CurveBracketOpen, CurveBracketClose,

		// Arrays
		SquareBracketOpen, SquareBracketClose,

		// Blocks
		CurlyBracketOpen, CurlyBracketClose,

		// Disregard
		Semicolon, Invalid, EOF,
	}

	public struct Token
	{
		public string Literal;
		public TokenType type;
        public object AdditionalData;

        public override string ToString()
		{
			return $"Davis Token:\n  Literal:\n    {Literal}\n  Type:\n    {type}";
		}
	}

	public class TokenStream {
        private CodeGenState state;

        public TokenStream(List<Token> stream) {
            state = new CodeGenState(stream);
        }

        public (byte[], object[]) Compile() {
            state.PreprocessTokens();
            state.ParseTypes();
            state.ParseFunctions();
            throw new NotImplementedException();
        }
	}

	public class BlockData {
        public BlockType type;
        public Token pair;
    }
	public enum BlockType {
		Function,
		ControlFlow
	}

	public class CodeGenState {
        private List<Token> _Stream;

        private List<object> _Constants;
        private List<Instruction> _Instructions;

        public List<string> Functions;
        public List<string> Types;

        public CodeGenState(List<Token> stream) {
            _Stream = stream;
        }

		public void ParseFunctions() {
            Functions = new List<string>();

            for (int i = 0; i < _Stream.Count; i++)
			{
                Token item = _Stream[i];
                if (item.type != TokenType.Function) continue;

				if(_Stream[i+1].type != TokenType.Identifier) throw new ParseException("Expected Identifier after function definition.");
                i++;

				if(Functions.Contains(_Stream[i].Literal)) throw new ParseException($"Duplicate function definition with name {_Stream[i].Literal}");
                Functions.Add(_Stream[i].Literal);
            }
		}

		public void ParseTypes() {
            Types = new List<string>();

            for (int i = 0; i < _Stream.Count; i++)
			{
                Token item = _Stream[i];
                if (item.type != TokenType.Type) continue;

				if(_Stream[i+1].type != TokenType.Identifier) throw new ParseException("Expected Identifier after type definition.");
                i++;

				if(Types.Contains(_Stream[i].Literal)) throw new ParseException($"Duplicate type definition with name {_Stream[i].Literal}");
                Types.Add(_Stream[i].Literal);
            }
        }

		public void PreprocessTokens() {
            int i = 0;
			Token Expect(TokenType type) {
				if(_Stream[i + 1].type != type) {
					throw new ParseException($"Expected token {type}, got token {_Stream[i + 1].type}");
				} else {
                    i++;
                    return _Stream[i];
				}
            }

            for (; i < _Stream.Count; i++)
			{
                Token token = _Stream[i];
                switch(token.type) {
					case TokenType.Function: {
						Expect(TokenType.Identifier);
						Expect(TokenType.CurveBracketOpen);
						while(_Stream[i].type != TokenType.CurveBracketClose) {
							if(i >= _Stream.Count) throw new ParseException("Expected ) after function arguments, got EOF.");
							i++;
						}

						Token bracket = Expect(TokenType.CurlyBracketOpen);
						bracket.AdditionalData = new BlockData() {
							type = BlockType.Function
						};

						int count = 0;
						while(i < _Stream.Count) {
							if(_Stream[i].type == TokenType.CurlyBracketOpen) count++;
							else if(_Stream[i].type == TokenType.CurlyBracketClose) {
								count--;
								if(count == 0) break;
							}
							i++;
						}

						Token end = _Stream[i];
						end.AdditionalData = new BlockData() {
							type = BlockType.Function,
							pair = bracket
						};

						((BlockData)bracket.AdditionalData).pair = end;

						break;
					}
					default: break;
                }
			}
		}
	}

	[System.Serializable]
	public class ParseException : System.Exception
	{
		public ParseException() { }
		public ParseException(string message) : base(message) { }
		public ParseException(string message, System.Exception inner) : base(message, inner) { }
		protected ParseException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}