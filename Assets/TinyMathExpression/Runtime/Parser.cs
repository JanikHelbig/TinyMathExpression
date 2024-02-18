using System;
using System.Text;

namespace Jnk.TinyMathExpression
{
    public ref struct Parser<T, TOperationHandler>
        where T : unmanaged
        where TOperationHandler : unmanaged, IOperationHandler<T>
    {
        private int _tokenIndex;
        private readonly ReadOnlySpan<Token<T>> _tokens;

        private int _bufferIndex;
        private readonly Span<Instruction<T>> _buffer;

        private readonly TOperationHandler _handler;

        private readonly TokenType CurrentToken => _tokens[_tokenIndex].Type;

        public Parser(ReadOnlySpan<Token<T>> tokens, Span<Instruction<T>> buffer, TOperationHandler handler)
        {
            _tokenIndex = 0;
            _tokens = tokens;

            _bufferIndex = 0;
            _buffer = buffer;

            _handler = handler;
        }

        public int BuildInstructions()
        {
            Expression();
            return _bufferIndex;
        }

        private void AddInstruction(Instruction<T> instruction)
        {
            _buffer[_bufferIndex++] = instruction;
        }

        private void Expression()
        {
            Term();
            while (OptionalTerm()) { }
        }

        private bool OptionalTerm()
        {
            switch (CurrentToken)
            {
                case TokenType.Plus:
                    ConsumeToken();
                    Term();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Add));
                    return true;

                case TokenType.Minus:
                    ConsumeToken();
                    Term();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Sub));
                    return true;

                default:
                    return false;
            }
        }


        private void Term()
        {
            Factor();
            while (OptionalFactor()) { }
        }

        private bool OptionalFactor()
        {
            switch (CurrentToken)
            {
                case TokenType.Mul:
                    ConsumeToken();
                    Factor();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Mul));
                    return true;

                case TokenType.Div:
                    ConsumeToken();
                    Factor();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Div));
                    return true;

                default:
                    return false;
            }
        }

        private void Factor()
        {
            Potential();
            while (OptionalPotential()) { }
        }

        private bool OptionalPotential()
        {
            switch (CurrentToken)
            {
                case TokenType.Pow:
                    ConsumeToken();
                    Potential();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Pow));
                    return true;

                default:
                    return false;
            }
        }

        private void Potential()
        {
            TokenType tokenType = CurrentToken;

            switch (tokenType)
            {
                case TokenType.Round:
                case TokenType.Floor:
                case TokenType.Ceil:
                case TokenType.Sqrt:
                case TokenType.Log:
                case TokenType.Cos:
                case TokenType.Sin:
                    ConsumeToken();
                    MatchType(TokenType.OpenParenthesis);
                    Expression();
                    MatchType(TokenType.ClosedParenthesis);
                    AddInstruction(Instruction<T>.FromOperator(tokenType switch
                    {
                        TokenType.Round => InstructionType.Round,
                        TokenType.Floor => InstructionType.Floor,
                        TokenType.Ceil  => InstructionType.Ceil,
                        TokenType.Sqrt  => InstructionType.Sqrt,
                        TokenType.Log   => InstructionType.Log,
                        TokenType.Sin   => InstructionType.Sin,
                        TokenType.Cos   => InstructionType.Cos,
                        _ => throw new ArgumentOutOfRangeException()
                    }));
                    break;

                case TokenType.PI:
                    AddInstruction(Instruction<T>.FromNumber(_handler.PI));
                    ConsumeToken();
                    break;

                case TokenType.TAU:
                    AddInstruction(Instruction<T>.FromNumber(_handler.TAU));
                    ConsumeToken();
                    break;

                case TokenType.E:
                    AddInstruction(Instruction<T>.FromNumber(_handler.E));
                    ConsumeToken();
                    break;

                case TokenType.OpenParenthesis:
                    ConsumeToken();
                    Expression();
                    MatchType(TokenType.ClosedParenthesis);
                    break;

                case TokenType.Minus:
                    ConsumeToken();
                    Expression();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.UnaryMinus));
                    break;

                case TokenType.Literal:
                    AddInstruction(Instruction<T>.FromNumber(_tokens[_tokenIndex].Value));
                    ConsumeToken();
                    break;

                case >= TokenType.Parameter0 and <= TokenType.Parameter9:
                    AddInstruction(Instruction<T>.FromOperator((InstructionType)tokenType));
                    ConsumeToken();
                    break;

                default:
                    string tokenString = GetTokenString(_tokens, _tokenIndex);
                    throw new ArgumentException($"Error in expression. Unexpected token '{CurrentToken}' after '{tokenString}'.");
            }
        }

        private void MatchType(TokenType tokenType)
        {
            if (CurrentToken != tokenType)
            {
                string tokenString = GetTokenString(_tokens, _tokenIndex);
                throw new ArgumentException($"Error in expression. Unexpected token '{CurrentToken}' after '{tokenString}', expected '{tokenType}'.");
            }

            ConsumeToken();
        }

        private void ConsumeToken()
        {
            if (++_tokenIndex > _tokens.Length && CurrentToken != TokenType.EndOfText)
                throw new ArgumentException("End of tokens reached but expected more.");
        }

        private static string GetTokenString(ReadOnlySpan<Token<T>> tokens, int index = -1)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < tokens.Length; i++)
            {
                if (index > 0 && index <= i)
                    return builder.ToString();

                Token<T> token = tokens[i];
                switch (token.Type)
                {
                    case TokenType.None:
                        builder.Append(' ');
                        break;
                    case TokenType.Plus:
                        builder.Append('+');
                        break;
                    case TokenType.Minus:
                        builder.Append('-');
                        break;
                    case TokenType.Mul:
                        builder.Append('*');
                        break;
                    case TokenType.Div:
                        builder.Append('/');
                        break;
                    case TokenType.Pow:
                        builder.Append('^');
                        break;
                    case TokenType.OpenParenthesis:
                        builder.Append('(');
                        break;
                    case TokenType.ClosedParenthesis:
                        builder.Append(')');
                        break;
                    case TokenType.Comma:
                        builder.Append(',');
                        break;
                    case TokenType.Round:
                        builder.Append("round");
                        break;
                    case TokenType.Floor:
                        builder.Append("floor");
                        break;
                    case TokenType.Ceil:
                        builder.Append("ceil");
                        break;
                    case TokenType.Sqrt:
                        builder.Append("sqrt");
                        break;
                    case TokenType.Log:
                        builder.Append("log");
                        break;
                    case TokenType.Sin:
                        builder.Append("sin");
                        break;
                    case TokenType.Cos:
                        builder.Append("cos");
                        break;
                    case TokenType.PI:
                        builder.Append("PI");
                        break;
                    case TokenType.TAU:
                        builder.Append("TAU");
                        break;
                    case TokenType.E:
                        builder.Append("E");
                        break;
                    case TokenType.Literal:
                        builder.Append(token.Value);
                        break;
                    case >= TokenType.Parameter0 and <= TokenType.Parameter9:
                        builder.Append('{').Append((int)token.Type).Append('}');
                        break;
                    case TokenType.EndOfText:
                        builder.Append("EOT");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return builder.ToString();
        }
    }
}