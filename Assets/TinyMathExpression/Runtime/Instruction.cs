using System.Globalization;

namespace Jnk.TinyMathExpression
{
    public enum InstructionType
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
        Add,
        Sub,
        Mul,
        Div,
        Pow,
        UnaryMinus,
        Round,
        Floor,
        Ceil,
        Sqrt,
        Log,
        Sin,
        Cos,
        Number
    }

    public readonly struct Instruction<T> where T : unmanaged
    {
        public readonly InstructionType Type;
        public readonly T Value;

        private Instruction(InstructionType type, T value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return Type switch
            {
                InstructionType.Number => Value.ToString(),
                InstructionType.Parameter0 => "{0}",
                InstructionType.Parameter1 => "{1}",
                InstructionType.Parameter2 => "{2}",
                InstructionType.Parameter3 => "{3}",
                InstructionType.Parameter4 => "{4}",
                InstructionType.Parameter5 => "{5}",
                InstructionType.Parameter6 => "{6}",
                InstructionType.Parameter7 => "{7}",
                InstructionType.Parameter8 => "{8}",
                InstructionType.Parameter9 => "{9}",
                _ => Type.ToString()
            };
        }

        public static Instruction<T> FromOperator(InstructionType type) => new(type, default);
        public static Instruction<T> FromNumber(T value) => new(InstructionType.Number, value);
    }
}