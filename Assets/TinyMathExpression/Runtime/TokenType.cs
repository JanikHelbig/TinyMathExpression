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
        Comma,

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
        public readonly TokenType Type;
        public readonly double Value;

        public Token(TokenType type, double value = 0)
        {
            Type = type;
            Value = value;
        }

        public string GetValueString()
        {
            return Type switch
            {
                TokenType.None => "NONE",
                TokenType.Plus => "+",
                TokenType.Minus => "-",
                TokenType.Mul => "*",
                TokenType.Div => "/",
                TokenType.Pow => "^",
                TokenType.OpenParenthesis => "(",
                TokenType.ClosedParenthesis => ")",
                TokenType.Comma => ";",
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