using System;

namespace Jnk.TinyMathExpression
{
    public enum TokenType
    {
        None,

        Plus,
        Minus,
        Mul,
        Div,
        Pow,

        OpenParenthesis,
        ClosedParenthesis,
        Semicolon,

        Round,
        Floor,
        Ceil,
        Sqrt,
        Log,
        Sin,
        Cos,

        PI,
        TAU,
        E,

        Literal,
        Parameter,
        EndOfText,
    }

    public readonly struct Token
    {
        public readonly TokenType type;
        public readonly double value;

        public Token(TokenType type, double value = 0)
        {
            this.type = type;
            this.value = value;
        }

        public string GetValueString()
        {
            return type switch
            {
                TokenType.None => "NONE",
                TokenType.Plus => "+",
                TokenType.Minus => "-",
                TokenType.Mul => "*",
                TokenType.Div => "/",
                TokenType.Pow => "^",
                TokenType.OpenParenthesis => "(",
                TokenType.ClosedParenthesis => ")",
                TokenType.Semicolon => ";",
                TokenType.Round => "Round",
                TokenType.Floor => "Floor",
                TokenType.Ceil => "Ceil",
                TokenType.Sqrt => "Sqrt",
                TokenType.Log => "Log",
                TokenType.Sin => "Sin",
                TokenType.Cos => "Cos",
                TokenType.Literal => "Literal",
                TokenType.Parameter => "Parameter",
                TokenType.EndOfText => "EndOfText",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}