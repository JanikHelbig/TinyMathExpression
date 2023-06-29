using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Assertions;

namespace Jnk.TinyMathExpression
{
    public class MathExpression
    {
        private readonly string _source;
        private readonly List<Instruction> _instructions;
        private readonly int _requiredStackSize;
        private readonly int _parameterCount;

        public MathExpression(string expression)
        {
            _source = expression;

            List<Token> tokens = new Lexer().Tokenize(_source);
            _instructions = new Parser().BuildInstructions(tokens);

            _requiredStackSize = CalculateRequiredStackSize(_instructions);
            _parameterCount = CountParameters(_instructions);
        }

        private static int CalculateRequiredStackSize(IReadOnlyList<Instruction> instructions)
        {
            var current = 0;
            var max = 0;

            foreach (var instruction in instructions)
            {
                switch (instruction.type)
                {
                    case InstructionType.Number:
                    case InstructionType.Parameter:
                        current++;
                        break;
                    case InstructionType.Add:
                    case InstructionType.Sub:
                    case InstructionType.Mul:
                    case InstructionType.Div:
                    case InstructionType.Pow:
                        current--;
                        break;
                    case InstructionType.UnaryMinus:
                    case InstructionType.Round:
                    case InstructionType.Floor:
                    case InstructionType.Ceil:
                    case InstructionType.Sqrt:
                    case InstructionType.Log:
                    case InstructionType.Sin:
                    case InstructionType.Cos:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (current > max)
                    max = current;
            }

            return max;
        }

        private static int CountParameters(IReadOnlyList<Instruction> instructions)
        {
            var parameterCount = 0;
            for (var i = 0; i < instructions.Count; i++)
                if (instructions[i].type == InstructionType.Parameter)
                    parameterCount++;

            return parameterCount;
        }

        private double EvaluateInstructions(ReadOnlySpan<double> args)
        {
            var stackTop = -1;
            Span<double> stack = stackalloc double[_requiredStackSize];

            foreach (var instruction in _instructions)
            {
                ExecuteInstruction(instruction, args, stack, ref stackTop);
            }

            Assert.AreEqual(0, stackTop);
            return stack[0];
        }

        private static void ExecuteInstruction(Instruction instruction, ReadOnlySpan<double> args, Span<double> stack, ref int stackTop)
        {
            var result = instruction.type switch
            {
                InstructionType.Number     => instruction.value,
                InstructionType.Parameter  => args[(int) instruction.value],
                InstructionType.Add        => stack[stackTop--] + stack[stackTop--],
                InstructionType.Sub        => SubInv(stack[stackTop--], stack[stackTop--]),
                InstructionType.Mul        => stack[stackTop--] * stack[stackTop--],
                InstructionType.Div        => DivInv(stack[stackTop--], stack[stackTop--]),
                InstructionType.Pow        => PowInv(stack[stackTop--], stack[stackTop--]),
                InstructionType.UnaryMinus => -stack[stackTop--],
                InstructionType.Round      => Math.Round(stack[stackTop--]),
                InstructionType.Floor      => Math.Floor(stack[stackTop--]),
                InstructionType.Ceil       => Math.Ceiling(stack[stackTop--]),
                InstructionType.Sqrt       => Math.Sqrt(stack[stackTop--]),
                InstructionType.Log        => Math.Log(stack[stackTop--]),
                InstructionType.Sin        => Math.Sin(stack[stackTop--]),
                InstructionType.Cos        => Math.Cos(stack[stackTop--]),
                _ => throw new ArgumentOutOfRangeException(nameof(instruction.type),
                    $"Encountered unhandled instruction type: {instruction.type}")
            };

            stack[++stackTop] = result;

            double SubInv(double b, double a) => a - b;
            double DivInv(double b, double a) => a / b;
            double PowInv(double b, double a) => Math.Pow(a, b);
        }

        #region Evaluation Calls

        public double Evaluate()
        {
            var args = ReadOnlySpan<double>.Empty;
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg0, double arg1)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg0, arg1 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg0, double arg1, double arg2)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg0, arg1, arg2 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg0, arg1, arg2, arg3 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8, double arg9)
        {
            ReadOnlySpan<double> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        private void AssertParameterCount(ReadOnlySpan<double> args)
        {
            if (args.Length != _parameterCount)
                throw new ArgumentException($"Number of provided arguments ({args.Length}) does not match number of parameters ({_parameterCount}).");
        }

        #endregion

        public string ToInstructionString()
        {
            var stringBuilder = new StringBuilder();

            foreach (var instruction in _instructions)
                stringBuilder.Append(instruction.ToString()).Append(" ");

            return stringBuilder.ToString();
        }

        public override string ToString() => _source;
    }
}