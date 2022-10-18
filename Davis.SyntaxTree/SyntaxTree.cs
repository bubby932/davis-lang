namespace Davis.SyntaxTree {
    public enum TokenType
    {
        // Preprocessor
        Pound, OpenAngleBracket, CloseAngleBracket,
		PreprocessorDirective,

        // Literals
        Identifier, Literal,
        QuoteDouble, QuoteSingle,

        // Keywords
        While, If, Else, Var, Sizeof, Assembly,

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

    public struct Token {
		public string Literal;
		public TokenType type;

		public override string ToString() {
            return $"Davis Token:\nLiteral:\n  {Literal}\nType:\n  {type}";
        }
	}
}