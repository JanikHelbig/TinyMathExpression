using System;
using System.Collections.Generic;

namespace Jnk.TinyMathExpression
{
    public struct Parser
    {
        private IReadOnlyList<Token> _tokens;
        private int _currentTokenIndex;
        private Token _currentToken;
        private List<Instruction> _instructions;

        public List<Instruction> BuildInstructions(IReadOnlyList<Token> tokens)
        {
            _tokens = tokens;
            _currentTokenIndex = 0;
            _currentToken = _tokens[_currentTokenIndex];
            _instructions = new List<Instruction>();

            Expression();
            return _instructions;
        }

        private void Expression()
        {
            Term();
            Expression1();
        }

        private void Expression1()
        {
            switch (_currentToken.type)
            {
                case TokenType.Plus:
                    GetNextToken();
                    Term();
                    Expression1();
                    _instructions.Add(Instruction.FromOperator(InstructionType.Add));
                    break;

                case TokenType.Minus:
                    GetNextToken();
                    Term();
                    Expression1();
                    _instructions.Add(Instruction.FromOperator(InstructionType.Sub));
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
            switch (_currentToken.type)
            {
                case TokenType.Mul:
                    GetNextToken();
                    Factor();
                    Term1();
                    _instructions.Add(Instruction.FromOperator(InstructionType.Mul));
                    break;

                case TokenType.Div:
                    GetNextToken();
                    Factor();
                    Term1();
                    _instructions.Add(Instruction.FromOperator(InstructionType.Div));
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
            switch (_currentToken.type)
            {
                case TokenType.Pow:
                    GetNextToken();
                    Potential();
                    Factor1();
                    _instructions.Add(Instruction.FromOperator(InstructionType.Pow));
                    break;
            }
        }

        private void Potential()
        {
            Token token = _currentToken;
            switch (_currentToken.type)
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
                    _instructions.Add(Instruction.FromOperator(token.type switch
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
                    GetNextToken();
                    _instructions.Add(Instruction.FromNumber(Math.PI));
                    break;

                case TokenType.TAU:
                    GetNextToken();
                    _instructions.Add(Instruction.FromNumber(2 * Math.PI));
                    break;

                case TokenType.E:
                    GetNextToken();
                    _instructions.Add(Instruction.FromNumber(Math.E));
                    break;

                case TokenType.OpenParenthesis:
                    GetNextToken();
                    Expression();
                    MatchType(TokenType.ClosedParenthesis);
                    break;

                case TokenType.Minus:
                    GetNextToken();
                    Expression();
                    _instructions.Add(Instruction.FromOperator(InstructionType.UnaryMinus));
                    break;

                case TokenType.Literal:
                    GetNextToken();
                    _instructions.Add(Instruction.FromNumber(token.value));
                    break;

                case TokenType.Parameter:
                    GetNextToken();
                    _instructions.Add(Instruction.FromParameter((int) token.value));
                    break;

                default:
                    throw new ArgumentException($"Unexpected token '{_currentToken}' at index '{_currentTokenIndex}'");
            }
        }

        private void MatchType(TokenType tokenType)
        {
            if (_currentToken.type != tokenType)
                throw new ArgumentException($"Expected token '{tokenType}' but encountered '{_tokens[_currentTokenIndex]}' at index '{_currentTokenIndex}'!");

            GetNextToken();
        }

        private void GetNextToken()
        {
            if (_currentTokenIndex + 1 >= _tokens.Count)
                throw new ArgumentException("End of tokens reached but expected more.");

            _currentToken = _tokens[++_currentTokenIndex];
        }
    }
}