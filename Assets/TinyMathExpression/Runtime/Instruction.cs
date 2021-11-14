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
        public readonly InstructionType type;
        public readonly double value;

        private Instruction(InstructionType type, double value)
        {
            this.type = type;
            this.value = value;
        }

        public override string ToString()
        {
            return type switch
            {
                InstructionType.Number => value.ToString(CultureInfo.InvariantCulture),
                InstructionType.Parameter => $"{{{value}}}",
                _ => type.ToString()
            };
        }

        public static Instruction FromOperator(InstructionType type) => new Instruction(type, 0);
        public static Instruction FromNumber(double value) => new Instruction(InstructionType.Number, value);
        public static Instruction FromParameter(int index) => new Instruction(InstructionType.Parameter, index);
    }
}