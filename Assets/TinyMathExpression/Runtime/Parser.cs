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
            Expression1();
        }

        private void Expression1()
        {
            switch (_tokens[_tokenIndex].Type)
            {
                case TokenType.Plus:
                    GetNextToken();
                    Term();
                    Expression1();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Add));
                    break;

                case TokenType.Minus:
                    GetNextToken();
                    Term();
                    Expression1();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Sub));
                    break;
            }
        }

        private void Term()
        {
            Factor();
            Term1();
        }

        private void Term1()
        {
            switch (_tokens[_tokenIndex].Type)
            {
                case TokenType.Mul:
                    GetNextToken();
                    Factor();
                    Term1();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Mul));
                    break;

                case TokenType.Div:
                    GetNextToken();
                    Factor();
                    Term1();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Div));
                    break;
            }
        }

        private void Factor()
        {
            Potential();
            Factor1();
        }

        private void Factor1()
        {
            switch (_tokens[_tokenIndex].Type)
            {
                case TokenType.Pow:
                    GetNextToken();
                    Potential();
                    Factor1();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.Pow));
                    break;
            }
        }

        private void Potential()
        {
            TokenType tokenType = _tokens[_tokenIndex].Type;

            switch (tokenType)
            {
                case TokenType.Round:
                case TokenType.Floor:
                case TokenType.Ceil:
                case TokenType.Sqrt:
                case TokenType.Log:
                case TokenType.Cos:
                case TokenType.Sin:
                    GetNextToken();
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
                    GetNextToken();
                    break;

                case TokenType.TAU:
                    AddInstruction(Instruction<T>.FromNumber(_handler.TAU));
                    GetNextToken();
                    break;

                case TokenType.E:
                    AddInstruction(Instruction<T>.FromNumber(_handler.E));
                    GetNextToken();
                    break;

                case TokenType.OpenParenthesis:
                    GetNextToken();
                    Expression();
                    MatchType(TokenType.ClosedParenthesis);
                    break;

                case TokenType.Minus:
                    GetNextToken();
                    Expression();
                    AddInstruction(Instruction<T>.FromOperator(InstructionType.UnaryMinus));
                    break;

                case TokenType.Literal:
                    AddInstruction(Instruction<T>.FromNumber(_tokens[_tokenIndex].Value));
                    GetNextToken();
                    break;

                case >= TokenType.Parameter0 and <= TokenType.Parameter9:
                    AddInstruction(Instruction<T>.FromOperator((InstructionType)tokenType));
                    GetNextToken();
                    break;

                default:
                    string tokenString = GetTokenString(_tokens, _tokenIndex);
                    throw new ArgumentException($"Error in expression. Unexpected token '{_tokens[_tokenIndex].Type}' after '{tokenString}'.");
            }
        }

        private void MatchType(TokenType tokenType)
        {
            if (_tokens[_tokenIndex].Type != tokenType)
            {
                string tokenString = GetTokenString(_tokens, _tokenIndex);
                throw new ArgumentException($"Error in expression. Unexpected token '{_tokens[_tokenIndex].Type}' after '{tokenString}', expected '{tokenType}'.");
            }

            GetNextToken();
        }

        private void GetNextToken()
        {
            if (++_tokenIndex > _tokens.Length && _tokens[_tokenIndex].Type != TokenType.EndOfText)
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