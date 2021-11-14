using System;
using System.Collections.Generic;
using System.Text;

namespace Jnk.TinyMathExpression
{
    public class MathExpression
    {
        private readonly string _source;
        private readonly List<Instruction> _instructions;
        private readonly Stack<double> _stack = new Stack<double>();
        private readonly double[] _args;

        public MathExpression(string expression)
        {
            _source = expression;

            List<Token> tokens = new Lexer().Tokenize(_source);
            _instructions = new Parser().BuildInstructions(tokens);
            _args = new double[CountParameters(_instructions)];
        }

        private static int CountParameters(IReadOnlyList<Instruction> instructions)
        {
            var parameterCount = 0;
            for (var i = 0; i < instructions.Count; i++)
                if (instructions[i].type == InstructionType.Parameter)
                    parameterCount++;

            return parameterCount;
        }

        private double EvaluateInstructions()
        {
            _stack.Clear();

            foreach (Instruction instruction in _instructions)
                _stack.Push(ExecuteInstruction(instruction));

            return _stack.Pop();
        }

        private double ExecuteInstruction(Instruction instruction)
        {
            return instruction.type switch
            {
                InstructionType.Number     => instruction.value,
                InstructionType.Parameter  => _args[(int) instruction.value],
                InstructionType.Add        => _stack.Pop() + _stack.Pop(),
                InstructionType.Sub        => SubInv(_stack.Pop(), _stack.Pop()),
                InstructionType.Mul        => _stack.Pop() * _stack.Pop(),
                InstructionType.Div        => DivInv(_stack.Pop(), _stack.Pop()),
                InstructionType.Pow        => PowInv(_stack.Pop(), _stack.Pop()),
                InstructionType.UnaryMinus => -_stack.Pop(),
                InstructionType.Round      => Math.Round(_stack.Pop()),
                InstructionType.Floor      => Math.Floor(_stack.Pop()),
                InstructionType.Ceil       => Math.Ceiling(_stack.Pop()),
                InstructionType.Sqrt       => Math.Sqrt(_stack.Pop()),
                InstructionType.Log        => Math.Log(_stack.Pop()),
                InstructionType.Sin        => Math.Sin(_stack.Pop()),
                InstructionType.Cos        => Math.Cos(_stack.Pop()),
                _ => throw new ArgumentOutOfRangeException(nameof(instruction.type),
                    $"Encountered unhandled instruction type: {instruction.type}")
            };

            double SubInv(double b, double a) => a - b;
            double DivInv(double b, double a) => a / b;
            double PowInv(double b, double a) => Math.Pow(a, b);
        }

        #region Evaluation Calls

        public double Evaluate()
        {
            AssertParameterCount(0);
            return EvaluateInstructions();
        }

        public double Evaluate(double arg)
        {
            AssertParameterCount(1);
            _args[0] = arg;
            return EvaluateInstructions();
        }

        public double Evaluate(double arg0, double arg1)
        {
            AssertParameterCount(2);
            _args[0] = arg0;
            _args[1] = arg1;
            return EvaluateInstructions();
        }

        public double Evaluate(double arg0, double arg1, double arg2)
        {
            AssertParameterCount(3);
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            return EvaluateInstructions();
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3)
        {
            AssertParameterCount(4);
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            _args[3] = arg3;
            return EvaluateInstructions();
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4)
        {
            AssertParameterCount(5);
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            _args[3] = arg3;
            _args[4] = arg4;
            return EvaluateInstructions();
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5)
        {
            AssertParameterCount(6);
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            _args[3] = arg3;
            _args[4] = arg4;
            _args[5] = arg5;
            return EvaluateInstructions();
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6)
        {
            AssertParameterCount(7);
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            _args[3] = arg3;
            _args[4] = arg4;
            _args[5] = arg5;
            _args[6] = arg6;
            return EvaluateInstructions();
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7)
        {
            AssertParameterCount(8);
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            _args[3] = arg3;
            _args[4] = arg4;
            _args[5] = arg5;
            _args[6] = arg6;
            _args[7] = arg7;
            return EvaluateInstructions();
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8)
        {
            AssertParameterCount(9);
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            _args[3] = arg3;
            _args[4] = arg4;
            _args[5] = arg5;
            _args[6] = arg6;
            _args[7] = arg7;
            _args[8] = arg8;
            return EvaluateInstructions();
        }

        public double Evaluate(double arg0, double arg1, double arg2, double arg3, double arg4, double arg5, double arg6, double arg7, double arg8, double arg9)
        {
            AssertParameterCount(10);
            _args[0] = arg0;
            _args[1] = arg1;
            _args[2] = arg2;
            _args[3] = arg3;
            _args[4] = arg4;
            _args[5] = arg5;
            _args[6] = arg6;
            _args[7] = arg7;
            _args[8] = arg8;
            _args[9] = arg9;
            return EvaluateInstructions();
        }

        private void AssertParameterCount(int argumentCount)
        {
            if (_args.Length != argumentCount)
                throw new ArgumentException($"Number of provided arguments ({argumentCount}) does not match number of parameters ({_args.Length}).");
        }

        #endregion

        public string ToInstructionString()
        {
            var stringBuilder = new StringBuilder();

            foreach (Instruction instruction in _instructions)
                stringBuilder.Append(instruction.ToString()).Append(" ");

            return stringBuilder.ToString();
        }

        public override string ToString() => _source;
    }
}