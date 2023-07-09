using System.Globalization;

namespace Jnk.TinyMathExpression
{
    public enum InstructionType
    {
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
        Parameter,
        Number
    }

    public readonly struct Instruction
    {
        public readonly InstructionType Type;
        public readonly double Value;

        private Instruction(InstructionType type, double value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return Type switch
            {
                InstructionType.Number => Value.ToString(CultureInfo.InvariantCulture),
                InstructionType.Parameter => $"{{{Value}}}",
                _ => Type.ToString()
            };
        }

        public static Instruction FromOperator(InstructionType type) => new(type, 0);
        public static Instruction FromNumber(double value) => new(InstructionType.Number, value);
        public static Instruction FromParameter(char index) => new(InstructionType.Parameter, index);
    }
}