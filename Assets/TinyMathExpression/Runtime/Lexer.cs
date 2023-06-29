using System;
using System.Collections.Generic;
using System.Globalization;

namespace Jnk.TinyMathExpression
{
    public struct Lexer
    {
        private int _index;
        private string _expression;

        public List<Token> Tokenize(string expression)
        {
            _index = 0;
            _expression = expression;

            var tokens = new List<Token>(8);

            while (_index < _expression.Length)
            {
                if (_expression[_index] == ' ')
                {
                    _index++;
                    continue;
                }

                if (TryParse(out Token token) == false)
                    throw new Exception($"Error when parsing expression at position {_index}.");

                tokens.Add(token);
            }

            tokens.Add(new Token(TokenType.EndOfText));
            return tokens;
        }

        private bool TryParse(out Token token)
        {
            token = default;

            return TryParseOperator(out token) ||
                   TryParseSeparator(out token) ||
                   TryParseKeyword(out token) ||
                   TryParseLiteral(out token) ||
                   TryParseConstant(out token) ||
                   TryParseIdentifier(out token);
        }

        private bool TryParseOperator(out Token token)
        {
            token = new Token(_expression[_index] switch
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

            _index++;
            return true;
        }

        private bool TryParseSeparator(out Token token)
        {
            token = new Token(_expression[_index] switch
            {
                '(' => TokenType.OpenParenthesis,
                ')' => TokenType.ClosedParenthesis,
                ',' => TokenType.Semicolon,
                _   => TokenType.None
            });

            if (token.type == TokenType.None)
                return false;

            _index++;
            return true;
        }

        private bool TryParseKeyword(out Token token)
        {
            var length = 0;
            while (_index + length < _expression.Length &&
                   _expression[_index + length] >= 'a' &&
                   _expression[_index + length] <= 'z')
                length++;

            string word = string.Intern(_expression.Substring(_index, length));

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

            _index += word.Length;
            return true;
        }

        private bool TryParseLiteral(out Token token)
        {
            var length = 0;
            while (_index + length < _expression.Length && (
                       _expression[_index + length] >= '0' &&
                       _expression[_index + length] <= '9' ||
                       _expression[_index + length] == '.'))
                length++;

            string number = _expression.Substring(_index, length);

            const NumberStyles styles = NumberStyles.Float;
            if (double.TryParse(number, styles, CultureInfo.InvariantCulture, out double value) == false)
            {
                token = default;
                return false;
            }

            token = new Token(TokenType.Literal, value);
            _index += number.Length;
            return true;
        }

        private bool TryParseConstant(out Token token)
        {
            var length = 0;
            while (_index + length < _expression.Length &&
                   _expression[_index + length] >= 'A' &&
                   _expression[_index + length] <= 'Z')
                length++;

            string word = string.Intern(_expression.Substring(_index, length));

            token = new Token(word switch
            {
                "PI"  => TokenType.PI,
                "TAU" => TokenType.TAU,
                "E"   => TokenType.E,
                _     => TokenType.None
            });

            if (token.type == TokenType.None)
                return false;

            _index += word.Length;
            return true;
        }

        private bool TryParseIdentifier(out Token token)
        {
            bool isIdentifier = _expression[_index] == '{' &&
                                _expression[_index + 1] >= '0' &&
                                _expression[_index + 1] <= '9' &&
                                _expression[_index + 2] == '}';

            if (isIdentifier == false)
            {
                token = default;
                return false;
            }

            int value = _expression[_index + 1] - '0';
            token = new Token(TokenType.Parameter, value);
            _index += 3;
            return true;
        }
    }
}