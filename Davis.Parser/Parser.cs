using System;
using System.Collections.Generic;
using Davis.SyntaxTree;

namespace Davis.Parser
{
	public static class Parser
	{
		public static List<Token> Output;

		internal static int current_start = 0;
		internal static int current_end = 0;
		internal static string src = null;

		public static void Parse(string text)
		{
			current_start = 0;
			current_end = 0;
			src = text;
			Output = new List<Token>();

			while (current_end < src.Length)
			{
                switch (src[current_end])
				{
					case '#': 
						{
							SkipWhitespace();
							while (!MatchNext('\n'))
								++current_end;

							++current_end;
							current_start = current_end;

							Emit(TokenType.PreprocessorDirective);
							break;
						}
					case '/': 
						{
							if (MatchNext('/'))
							{
								// Comment
								MatchUntil('\n');
							}
							else if (MatchNext('*'))
							{
								/* Comment */
								while (current_end < src.Length && !MatchNext('/'))
								{
									MatchUntil('*');
								}
								++current_end;
							}
							else
							{
								Emit(TokenType.Slash);
							}
							break;
						}
					case '{': 
						{
							Emit(TokenType.CurlyBracketOpen);
                            break;
                        }
					case '}': 
						{
							Emit(TokenType.CurlyBracketClose);
                            break;
                        }
					case '(': 
						{
                            Emit(TokenType.CurveBracketOpen);
                            break;
                        }
					case ')': 
						{
                            Emit(TokenType.CurveBracketClose);
                            break;
                        }
					case '[': 
						{
							Emit(TokenType.SquareBracketOpen);
                            break;
                        }
					case ']': 
						{
							Emit(TokenType.SquareBracketClose);
                            break;
                        }
					case '<': 
						{
							Emit(TokenType.OpenAngleBracket);
                            break;
                        }
					case '>': 
						{
							Emit(TokenType.CloseAngleBracket);
                            break;
                        }
					default:
						{
							if (IsAlphanumeric(src[current_end]))
							{
								string literal = Identifier();

                                if (literal != "assembly") break;

								Output.RemoveAt(Output.Count - 1);

								SkipWhitespace();
								current_end--;
								Consume('{', "Expected block after assembly statement.");

								current_start = current_end + 1;

								MatchUntil('}');

								if (current_end == src.Length) throw new Exception("Unterminated assembly literal.");

								--current_end;

								Emit(TokenType.Assembly);

								++current_end;

								break;
							}
							else
							{
								throw new NotImplementedException($"Unrecognized character '{src[current_end]}'");
							}
						}
				}

				current_end++;
				SkipWhitespace();
			}
		}

		private static void Emit(TokenType token)
		{
			Output.Add(
				new Token()
				{
					Literal = src.Substring(current_start, current_end - current_start),
					type = token
				}
			);
			current_start = current_end;
		}

		private static bool MatchNext(char c) => src[current_end + 1] == c;
		private static bool MatchPrevious(char c) => src[current_end - 1] == c;

		private static void MatchUntil(char c)
		{
			++current_end;
			while (current_end < src.Length && src[current_end] != c)
			{
				++current_end;
			}
		}

		private static void MatchAlphanumeric()
		{
			++current_end;
			while (current_end < src.Length)
			{
				char c = src[current_end];
				if (
					IsAlphanumeric(c)
				)
				{
					++current_end;
				}
				else
				{
                    break;
				}
			}
		}

		private static bool IsAlphanumeric(char c) =>
			(c >= 'a' &&
			c <= 'z') ||
			(c >= 'A' &&
			c <= 'Z') ||
			(c >= '1' &&
			c <= '0');

		private static void Consume(char c, string on_error)
		{
			if (MatchNext(c)) ++current_end;
			else throw new System.Exception(on_error);
		}

		private static void SkipWhitespace()
		{
			while (current_end < src.Length)
			{
				switch (src[current_end])
				{
					case ' ':
					case '\n':
					case '\t':
					case '\r':
						break;
					default:
                        return;
				}
				++current_end;
			}
            current_start = current_end;
		}

		private static string Identifier()
		{
            SkipWhitespace();
            current_start = current_end;
			MatchAlphanumeric();

			string s = src.Substring(current_start, current_end - current_start);
            Console.WriteLine($"{s}|");
            switch (s)
			{
				case "stackalloc":
					{
						Emit(TokenType.Stackalloc);
						return s;
					}
				case "heapalloc":
					{
						Emit(TokenType.Heapalloc);
						return s;
					}
				case "while":
					{
						Emit(TokenType.While);
						return s;
					}
				case "if":
					{
						Emit(TokenType.If);
						return s;
					}
				case "else":
					{
						Emit(TokenType.Else);
						return s;
					}
				case "var":
					{
						Emit(TokenType.Var);
						return s;
					}
				case "sizeof":
					{
						Emit(TokenType.Sizeof);
						return s;
					}
				case "assembly":
					{
						Emit(TokenType.Assembly);
						return s;
					}
				case "function":
					{
                        Emit(TokenType.Function);
                        return s;
                    }
				default:
					{
						Emit(TokenType.Identifier);
						return s;
					}
			}
		}
	}
}