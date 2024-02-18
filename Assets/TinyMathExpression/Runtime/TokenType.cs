using System;

namespace Jnk.TinyMathExpression
{
    public enum TokenType
    {
        Parameter0 = 0,
        Parameter1 = 1,
        Parameter2 = 2,
        Parameter3 = 3,
        Parameter4 = 4,
        Parameter5 = 5,
        Parameter6 = 6,
        Parameter7 = 7,
        Parameter8 = 8,
        Parameter9 = 9,

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
        EndOfText,
    }

    public static class TokenTypeUtility
    {
        public static readonly TokenType[] ParameterLookup =
        {
            TokenType.Parameter0,
            TokenType.Parameter1,
            TokenType.Parameter2,
            TokenType.Parameter3,
            TokenType.Parameter4,
            TokenType.Parameter5,
            TokenType.Parameter6,
            TokenType.Parameter7,
            TokenType.Parameter8,
            TokenType.Parameter9,
        };
    }

    public readonly struct Token<T> where T : unmanaged
    {
        public readonly TokenType Type;
        public readonly T Value;

        public Token(TokenType type, T value = default)
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
                TokenType.Parameter0 => "Parameter0",
                TokenType.Parameter1 => "Parameter1",
                TokenType.Parameter2 => "Parameter2",
                TokenType.Parameter3 => "Parameter3",
                TokenType.Parameter4 => "Parameter4",
                TokenType.Parameter5 => "Parameter5",
                TokenType.Parameter6 => "Parameter6",
                TokenType.Parameter7 => "Parameter7",
                TokenType.Parameter8 => "Parameter8",
                TokenType.Parameter9 => "Parameter9",
                TokenType.EndOfText => "EndOfText",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}