using System;
using System.Globalization;

namespace Jnk.TinyMathExpression
{
    public class MathExpression : MathExpressionBase<double, MathExpression.OperationHandler>
    {
        public readonly struct OperationHandler : IOperationHandler<double>
        {
            private const double Tau = Math.PI * 2;

            public double PI => Math.PI;
            public double TAU => Tau;
            public double E => Math.E;

            public double Add(double left, double right) => left + right;
            public double Subtract(double left, double right) => left - right;
            public double Multiply(double left, double right) => left * right;
            public double Divide(double left, double right) => left / right;
            public double Power(double left, double right) => Math.Pow(left, right);
            public double UnaryMinus(double value) => -value;
            public double Round(double value) => Math.Round(value);
            public double Floor(double value) => Math.Floor(value);
            public double Ceil(double value) => Math.Ceiling(value);
            public double Sqrt(double value) => Math.Sqrt(value);
            public double Log(double value) => Math.Log(value);
            public double Sine(double value) => Math.Sin(value);
            public double Cosine(double value) => Math.Cos(value);

            public bool IsCharacterValidInLiteral(char character) => character is (>= '0' and <= '9') or '.';
            public bool TryParseLiteral(ReadOnlySpan<char> literal, out double value)
            {
                return double.TryParse(literal, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out value);
            }
        }

        public MathExpression(string expression) : base(expression) { }
    }
}