using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using Debug = UnityEngine.Debug;

namespace Jnk.TinyMathExpression
{
    public interface IOperationHandler<T> where T : unmanaged
    {
        [Pure] T PI { get; }
        [Pure] T TAU { get; }
        [Pure] T E { get; }

        [Pure] T Add(T left, T right);
        [Pure] T Subtract(T left, T right);
        [Pure] T Multiply(T left, T right);
        [Pure] T Divide(T left, T right);
        [Pure] T Power(T left, T right);
        [Pure] T UnaryMinus(T value);
        [Pure] T Round(T value);
        [Pure] T Floor(T value);
        [Pure] T Ceil(T value);
        [Pure] T Sqrt(T value);
        [Pure] T Log(T value);
        [Pure] T Sine(T value);
        [Pure] T Cosine(T value);

        [Pure] bool IsCharacterValidInLiteral(char character);
        [Pure] bool TryParseLiteral(ReadOnlySpan<char> literal, out T value);
    }

    public abstract class MathExpressionBase<T, TOperationHandler>
        where T : unmanaged
        where TOperationHandler : unmanaged, IOperationHandler<T>
    {
        private static readonly TOperationHandler Handler = default;
        private readonly string _source;

        private readonly int _parameterCount;
        private readonly int _requiredStackSize;
        private readonly Instruction<T>[] _instructions;

        protected MathExpressionBase(string expression)
        {
            _source = expression;

            Span<Token<T>> tokenBuffer = stackalloc Token<T>[expression.Length];
            int tokenCount = new Lexer<T, TOperationHandler>(expression, tokenBuffer, Handler).Tokenize();

            Span<Instruction<T>> instructionBuffer = stackalloc Instruction<T>[tokenCount];
            int instructionCount = new Parser<T, TOperationHandler>(tokenBuffer, instructionBuffer, Handler).BuildInstructions();

            ReadOnlySpan<Instruction<T>> instructions = instructionBuffer[..instructionCount];

            _parameterCount = CountParameters(instructions);
            _requiredStackSize = CalculateRequiredStackSize(instructions);

            _instructions = instructions.ToArray();
        }

        private static int CalculateRequiredStackSize(ReadOnlySpan<Instruction<T>> instructions)
        {
            var current = 0;
            var max = 0;

            for (var i = 0; i < instructions.Length; i++)
            {
                switch (instructions[i].Type)
                {
                    case InstructionType.Number:
                    case InstructionType.Parameter0:
                    case InstructionType.Parameter1:
                    case InstructionType.Parameter2:
                    case InstructionType.Parameter3:
                    case InstructionType.Parameter4:
                    case InstructionType.Parameter5:
                    case InstructionType.Parameter6:
                    case InstructionType.Parameter7:
                    case InstructionType.Parameter8:
                    case InstructionType.Parameter9:
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

        private static int CountParameters(ReadOnlySpan<Instruction<T>> instructions)
        {
            uint mask = 0;
            var count = 0;

            for (var i = 0; i < instructions.Length; i++)
            {
                if (instructions[i].Type is >= InstructionType.Parameter0 and <= InstructionType.Parameter9)
                {
                    var index = (int) instructions[i].Type;

                    if ((mask & 1u << index) > 0)
                        continue;

                    mask |= 1u << index;
                    count++;
                }
            }

            return count;
        }

        private T EvaluateInstructions(ReadOnlySpan<T> args)
        {
            int stackTop = -1;
            Span<T> stack = stackalloc T[_requiredStackSize];

            for (var i = 0; i < _instructions.Length; i++)
                ExecuteInstruction(_instructions[i], args, stack, ref stackTop);

            Trace.Assert(stackTop == 0, "Exactly one element should remain on the stack after evaluation.");
            return stack[0];
        }

        private static void ExecuteInstruction(Instruction<T> instruction, ReadOnlySpan<T> args, Span<T> stack, ref int stackTop)
        {
            T result = instruction.Type switch
            {
                >= InstructionType.Parameter0 and <= InstructionType.Parameter9 => args[(int) instruction.Type],
                InstructionType.Number     => instruction.Value,
                InstructionType.Add        => Handler.Add(stack[stackTop--], stack[stackTop--]),
                InstructionType.Sub        => SubInv(stack[stackTop--], stack[stackTop--]),
                InstructionType.Mul        => Handler.Multiply(stack[stackTop--], stack[stackTop--]),
                InstructionType.Div        => DivInv(stack[stackTop--], stack[stackTop--]),
                InstructionType.Pow        => PowInv(stack[stackTop--], stack[stackTop--]),
                InstructionType.UnaryMinus => Handler.UnaryMinus(stack[stackTop--]),
                InstructionType.Round      => Handler.Round(stack[stackTop--]),
                InstructionType.Floor      => Handler.Floor(stack[stackTop--]),
                InstructionType.Ceil       => Handler.Ceil(stack[stackTop--]),
                InstructionType.Sqrt       => Handler.Sqrt(stack[stackTop--]),
                InstructionType.Log        => Handler.Log(stack[stackTop--]),
                InstructionType.Sin        => Handler.Sine(stack[stackTop--]),
                InstructionType.Cos        => Handler.Cosine(stack[stackTop--]),
                _ => throw new ArgumentOutOfRangeException(nameof(instruction.Type),
                    $"Encountered unhandled instruction type: {instruction.Type}")
            };

            stack[++stackTop] = result;
            return;

            T SubInv(T b, T a) => Handler.Subtract(a, b);
            T DivInv(T b, T a) => Handler.Divide(a, b);
            T PowInv(T b, T a) => Handler.Power(a, b);
        }

        #region Evaluation Calls

        public T Evaluate()
        {
            var args = ReadOnlySpan<T>.Empty;
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg0, T arg1)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg0, arg1 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg0, T arg1, T arg2)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg0, arg1, arg2 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg0, T arg1, T arg2, T arg3)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg0, arg1, arg2, arg3 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg0, T arg1, T arg2, T arg3, T arg4)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg0, T arg1, T arg2, T arg3, T arg4, T arg5)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg0, T arg1, T arg2, T arg3, T arg4, T arg5, T arg6)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg0, T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg0, T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        public T Evaluate(T arg0, T arg1, T arg2, T arg3, T arg4, T arg5, T arg6, T arg7, T arg8, T arg9)
        {
            ReadOnlySpan<T> args = stackalloc[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 };
            AssertParameterCount(args);
            return EvaluateInstructions(args);
        }

        private void AssertParameterCount(ReadOnlySpan<T> args)
        {
            if (args.Length < _parameterCount)
                throw new ArgumentException($"Number of provided arguments ({args.Length}) is less than the number of parameters ({_parameterCount}) in expression '{_source}'.");

            if (args.Length > _parameterCount)
                Debug.LogWarning($"Number of provided arguments ({args.Length}) is more than the number of parameters ({_parameterCount}) in expression '{_source}'.");
        }

        #endregion

        public string ToInstructionString()
        {
            var stringBuilder = new StringBuilder();

            foreach (Instruction<T> instruction in _instructions)
                stringBuilder.Append(instruction.ToString()).Append(" ");

            return stringBuilder.ToString();
        }

        public override string ToString() => _source;
    }
}