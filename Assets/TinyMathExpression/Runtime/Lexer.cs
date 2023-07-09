using System;
using System.Collections.Generic;
using System.Globalization;

namespace Jnk.TinyMathExpression
{
    public static class Lexer
    {
        public static List<Token> Tokenize(string expression)
        {
            var index = 0;
            var tokens = new List<Token>(8);

            while (index < expression.Length)
            {
                if (expression[index] == ' ')
                {
                    index++;
                    continue;
                }

                if (TryParse(expression, ref index, out Token token) == false)
                    throw new ArgumentException($"Error when parsing expression '{expression}' after '{expression[..index]}'.");

                tokens.Add(token);
            }

            tokens.Add(new Token(TokenType.EndOfText));
            return tokens;
        }

        private static bool TryParse(string expression, ref int index, out Token token)
        {
            token = default;
            return TryParseOperator(expression, ref index, out token) ||
                   TryParseSeparator(expression, ref index, out token) ||
                   TryParseKeyword(expression, ref index, out token) ||
                   TryParseLiteral(expression, ref index, out token) ||
                   TryParseConstant(expression, ref index, out token) ||
                   TryParseIdentifier(expression, ref index, out token);
        }

        private static bool TryParseOperator(string expression, ref int index, out Token token)
        {
            token = new Token(expression[index] switch
            {
                '+' => TokenType.Plus,
                '-' => TokenType.Minus,
                '*' => TokenType.Mul,
                '/' => TokenType.Div,
                '^' => TokenType.Pow,
                _   => TokenType.None
            });

            if (token.type == TokenType.None)
                return false;

            index++;
            return true;
        }

        private static bool TryParseSeparator(string expression, ref int index, out Token token)
        {
            token = new Token(expression[index] switch
            {
                '(' => TokenType.OpenParenthesis,
                ')' => TokenType.ClosedParenthesis,
                ',' => TokenType.Comma,
                _   => TokenType.None
            });

            if (token.type == TokenType.None)
                return false;

            index++;
            return true;
        }

        private static bool TryParseKeyword(string expression, ref int index, out Token token)
        {
            var length = 0;
            while (index + length < expression.Length &&
                   expression[index + length] >= 'a' &&
                   expression[index + length] <= 'z')
                length++;

            string word = string.Intern(expression.Substring(index, length));
            token = new Token(word switch
            {
                "round" => TokenType.Round,
                "floor" => TokenType.Floor,
                "ceil"  => TokenType.Ceil,
                "sqrt"  => TokenType.Sqrt,
                "log"   => TokenType.Log,
                "sin"   => TokenType.Sin,
                "cos"   => TokenType.Cos,
                _       => TokenType.None
            });

            if (token.type == TokenType.None)
                return false;

            index += length;
            return true;
        }

        private static bool TryParseLiteral(string expression, ref int index, out Token token)
        {
            var length = 0;
            while (index + length < expression.Length && (
                       expression[index + length] >= '0' &&
                       expression[index + length] <= '9' ||
                       expression[index + length] == '.'))
                length++;

            string number = expression.Substring(index, length);

            const NumberStyles styles = NumberStyles.Float;
            if (double.TryParse(number, styles, CultureInfo.InvariantCulture, out double value) == false)
            {
                token = default;
                return false;
            }

            token = new Token(TokenType.Literal, value);
            index += length;
            return true;
        }

        private static bool TryParseConstant(string expression, ref int index, out Token token)
        {
            var length = 0;
            while (index + length < expression.Length &&
                   expression[index + length] >= 'A' &&
                   expression[index + length] <= 'Z')
                length++;

            string word = string.Intern(expression.Substring(index, length));

            token = new Token(word switch
            {
                "PI"  => TokenType.PI,
                "TAU" => TokenType.TAU,
                "E"   => TokenType.E,
                _     => TokenType.None
            });

            if (token.type == TokenType.None)
                return false;

            index += length;
            return true;
        }

        private static bool TryParseIdentifier(string expr, ref int index, out Token token)
        {
            bool isIdentifier = expr[index] == '{' &&
                                expr[index + 1] >= '0' &&
                                expr[index + 1] <= '9' &&
                                expr[index + 2] == '}';

            if (isIdentifier == false)
            {
                token = default;
                return false;
            }

            int value = expr[index + 1] - '0';
            token = new Token(TokenType.Parameter, value);
            index += 3;
            return true;
        }
    }
}