using System;
using System.Collections.Generic;
using System.Text;

namespace Jnk.TinyMathExpression
{
    public static class Parser
    {
        public static List<Instruction> BuildInstructions(IReadOnlyList<Token> tokens)
        {
            var index = 0;
            var instructions = new List<Instruction>();

            Expression();
            return instructions;

            void Expression()
            {
                Term();
                Expression1();
            }

            void Expression1()
            {
                switch (tokens[index].type)
                {
                    case TokenType.Plus:
                        GetNextToken();
                        Term();
                        Expression1();
                        instructions.Add(Instruction.FromOperator(InstructionType.Add));
                        break;

                    case TokenType.Minus:
                        GetNextToken();
                        Term();
                        Expression1();
                        instructions.Add(Instruction.FromOperator(InstructionType.Sub));
                        break;
                }
            }

            void Term()
            {
                Factor();
                Term1();
            }

            void Term1()
            {
                switch (tokens[index].type)
                {
                    case TokenType.Mul:
                        GetNextToken();
                        Factor();
                        Term1();
                        instructions.Add(Instruction.FromOperator(InstructionType.Mul));
                        break;

                    case TokenType.Div:
                        GetNextToken();
                        Factor();
                        Term1();
                        instructions.Add(Instruction.FromOperator(InstructionType.Div));
                        break;
                }
            }

            void Factor()
            {
                Potential();
                Factor1();
            }

            void Factor1()
            {
                switch (tokens[index].type)
                {
                    case TokenType.Pow:
                        GetNextToken();
                        Potential();
                        Factor1();
                        instructions.Add(Instruction.FromOperator(InstructionType.Pow));
                        break;
                }
            }

            void Potential()
            {
                switch (tokens[index].type)
                {
                    case TokenType.Round:
                    case TokenType.Floor:
                    case TokenType.Ceil:
                    case TokenType.Sqrt:
                    case TokenType.Log:
                    case TokenType.Cos:
                    case TokenType.Sin:
                        TokenType tokenType = tokens[index].type;
                        GetNextToken();
                        MatchType(TokenType.OpenParenthesis);
                        Expression();
                        MatchType(TokenType.ClosedParenthesis);
                        instructions.Add(Instruction.FromOperator(tokenType switch
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
                        instructions.Add(Instruction.FromNumber(Math.PI));
                        GetNextToken();
                        break;

                    case TokenType.TAU:
                        instructions.Add(Instruction.FromNumber(2 * Math.PI));
                        GetNextToken();
                        break;

                    case TokenType.E:
                        instructions.Add(Instruction.FromNumber(Math.E));
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
                        instructions.Add(Instruction.FromOperator(InstructionType.UnaryMinus));
                        break;

                    case TokenType.Literal:
                        instructions.Add(Instruction.FromNumber(tokens[index].value));
                        GetNextToken();
                        break;

                    case TokenType.Parameter:
                        instructions.Add(Instruction.FromParameter((char) tokens[index].value));
                        GetNextToken();
                        break;

                    default:
                        string tokenString = GetTokenString(tokens, index);
                        throw new ArgumentException($"Error in expression. Unexpected token '{tokens[index].type}' after '{tokenString}'.");
                }
            }

            void MatchType(TokenType tokenType)
            {
                if (tokens[index].type != tokenType)
                {
                    string tokenString = GetTokenString(tokens, index);
                    throw new ArgumentException($"Error in expression. Unexpected token '{tokens[index].type}' after '{tokenString}', expected '{tokenType}'.");
                }

                GetNextToken();
            }

            void GetNextToken()
            {
                if (++index >= tokens.Count)
                    throw new ArgumentException("End of tokens reached but expected more.");
            }
        }

        private static string GetTokenString(IReadOnlyList<Token> tokens, int index = -1)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < tokens.Count; i++)
            {
                if (index > 0 && index <= i)
                    return builder.ToString();

                Token token = tokens[i];
                switch (token.type)
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
                        builder.Append(token.value);
                        break;
                    case TokenType.Parameter:
                        builder.Append('{').Append((int)token.value).Append('}');
                        break;
                    case TokenType.EndOfText:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return builder.ToString();
        }
    }
}