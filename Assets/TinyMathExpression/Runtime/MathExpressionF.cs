using System;
using System.Globalization;

namespace Jnk.TinyMathExpression
{
    public class MathExpressionF : MathExpressionBase<float, MathExpressionF.OperationHandler>
    {
        public readonly struct OperationHandler : IOperationHandler<float>
        {
            private const float Tau = MathF.PI * 2;

            public float PI => MathF.PI;
            public float TAU => Tau;
            public float E => MathF.E;

            public float Add(float left, float right) => left + right;
            public float Subtract(float left, float right) => left - right;
            public float Multiply(float left, float right) => left * right;
            public float Divide(float left, float right) => left / right;
            public float Power(float left, float right) => MathF.Pow(left, right);
            public float UnaryMinus(float value) => -value;
            public float Round(float value) => MathF.Round(value);
            public float Floor(float value) => MathF.Floor(value);
            public float Ceil(float value) => MathF.Ceiling(value);
            public float Sqrt(float value) => MathF.Sqrt(value);
            public float Log(float value) => MathF.Log(value);
            public float Sine(float value) => MathF.Sin(value);
            public float Cosine(float value) => MathF.Cos(value);

            public bool IsCharacterValidInLiteral(char character) => character is >= '0' and <= '9' or '.';
            public bool TryParseLiteral(ReadOnlySpan<char> literal, out float value)
            {
                return float.TryParse(literal, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out value);
            }
        }

        public MathExpressionF(string expression) : base(expression) { }
    }
}