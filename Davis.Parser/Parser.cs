using Davis.SyntaxTree;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Davis.Parser {
	public static class Parser {
        public static List<Token> Output;

        internal static int current_start = 0;
		internal static int current_end = 0;
		internal static string src = null;

		public static void Parse(string text) {
			current_start = 0;
			current_end = 0;
			src = text;
            Output = new List<Token>();

            while (current_end < text.Length) {
                switch (src[current_end])
				{
					case '#':
                        SkipWhitespace();
                        while(!Match('\n'))
                            ++current_end;

                        Emit(TokenType.PreprocessorDirective);
                        break;
                    default: throw new NotImplementedException($"Unrecognized character '{src[current_end]}'");
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

		private static bool Match(char c) => src[current_end + 1] == c;

		private static string Match_Until(char c) {
            current_start = current_end;
            while(current_end < src.Length && src[current_end] != c) {
                current_end++;
            }

			if(current_end + 1 == src.Length)
                throw new IndexOutOfRangeException($"EOF when expecting char {c}");
			else return src.Substring(current_start, current_end - current_start);
        }

		private static void Consume(char c, string on_error) {
			if (Match(c)) current_end++;
			else throw new System.Exception(on_error);
		}

		private static void SkipWhitespace() {
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
				current_end++;
			}
			current_start = current_end;
		}
	}

	
}