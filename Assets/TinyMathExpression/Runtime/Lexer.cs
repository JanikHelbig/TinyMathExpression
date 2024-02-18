using System;
using System.Globalization;

namespace Jnk.TinyMathExpression
{
    public ref struct Lexer<T, TOperationHandler>
        where T : unmanaged
        where TOperationHandler : unmanaged, IOperationHandler<T>
    {
        private int _index;
        private readonly ReadOnlySpan<char> _expression;
        private readonly Span<Token<T>> _buffer;
        private readonly TOperationHandler _handler;

        public Lexer(ReadOnlySpan<char> expression, Span<Token<T>> buffer, TOperationHandler handler)
        {
            _index = 0;
            _expression = expression;
            _buffer = buffer;
            _handler = handler;

            if (_buffer.Length < expression.Length)
                throw new ArgumentException("The result buffer needs to have at least the same length as the expression.", nameof(_buffer));
        }

        public int Tokenize()
        {
            _index = 0;

            var bufferIndex = 0;
            while (_index < _expression.Length)
            {
                while (_expression[_index] == ' ')
                    _index++;

                ref Token<T> token = ref _buffer[bufferIndex++];

                if (!TryParse(ref token))
                    throw new ArgumentException($"Error when parsing expression '{_expression.ToString()}' after '{_expression[.._index].ToString()}'.");
            }

            _buffer[bufferIndex++] = new Token<T>(TokenType.EndOfText);
            return bufferIndex;
        }

        private bool TryParse(ref Token<T> token)
        {
            return TryParseOperator(ref token)
                   || TryParseSeparator(ref token)
                   || TryParseKeyword(ref token)
                   || TryParseLiteral(ref token)
                   || TryParseConstant(ref token)
                   || TryParseIdentifier(ref token);
        }

        private bool TryParseOperator(ref Token<T> token)
        {
            TokenType tokenType = _expression[_index] switch
            {
                '+' => TokenType.Plus,
                '-' => TokenType.Minus,
                '*' => TokenType.Mul,
                '/' => TokenType.Div,
                '^' => TokenType.Pow,
                _ => TokenType.None
            };

            if (tokenType == TokenType.None)
                return false;

            token = new Token<T>(tokenType);
            _index++;
            return true;
        }

        private bool TryParseSeparator(ref Token<T> token)
        {
            TokenType tokenType = _expression[_index] switch
            {
                '(' => TokenType.OpenParenthesis,
                ')' => TokenType.ClosedParenthesis,
                ',' => TokenType.Comma,
                _ => TokenType.None
            };

            if (tokenType == TokenType.None)
                return false;

            token = new Token<T>(tokenType);
            _index++;
            return true;
        }

        private bool TryParseKeyword(ref Token<T> token)
        {
            var length = 0;
            while (_index + length < _expression.Length &&
                   _expression[_index + length] >= 'a' &&
                   _expression[_index + length] <= 'z')
                length++;

            var word = _expression.Slice(_index, length).ToString();
            TokenType tokenType = word switch
            {
                "round" => TokenType.Round,
                "floor" => TokenType.Floor,
                "ceil" => TokenType.Ceil,
                "sqrt" => TokenType.Sqrt,
                "log" => TokenType.Log,
                "sin" => TokenType.Sin,
                "cos" => TokenType.Cos,
                _ => TokenType.None
            };

            if (tokenType == TokenType.None)
                return false;

            token = new Token<T>(tokenType);
            _index += length;
            return true;
        }

        private bool TryParseLiteral(ref Token<T> token)
        {
            var length = 0;
            while (_index + length < _expression.Length && _handler.IsCharacterValidInLiteral(_expression[_index + length]))
                length++;

            ReadOnlySpan<char> literal = _expression.Slice(_index, length);
            if (!_handler.TryParseLiteral(literal, out T value))
                return false;

            token = new Token<T>(TokenType.Literal, value);
            _index += length;
            return true;
        }

        private bool TryParseConstant(ref Token<T> token)
        {
            var length = 0;
            while (_index + length < _expression.Length &&
                   _expression[_index + length] >= 'A' &&
                   _expression[_index + length] <= 'Z')
                length++;

            var word = _expression.Slice(_index, length).ToString();
            TokenType tokenType = word switch
            {
                "PI" => TokenType.PI,
                "TAU" => TokenType.TAU,
                "E" => TokenType.E,
                _ => TokenType.None
            };

            if (tokenType == TokenType.None)
                return false;

            token = new Token<T>(tokenType);
            _index += length;
            return true;
        }


        private bool TryParseIdentifier(ref Token<T> token)
        {
            bool isIdentifier = _expression[_index] == '{' &&
                                _expression[_index + 1] >= '0' &&
                                _expression[_index + 1] <= '9' &&
                                _expression[_index + 2] == '}';

            if (!isIdentifier)
                return false;

            int value = _expression[_index + 1] - '0';
            var parameter = (TokenType) value;

            token = new Token<T>(parameter);
            _index += 3;
            return true;
        }
    }
}