using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Lang.Scanner
{
    public class Lexer
    {
        private string _input;
        private int _position;
        private int _line;
        private int _column;

        public Lexer(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                _input = streamReader.ReadToEnd();

            }
            _position = 0;
            _column = 0;
            _line = 0;
        }

        public List<Token> GetTokens()
        {
            var tokens = new List<Token>();
            while (true)
            {
                var token = NextToken();
                tokens.Add(token);
                if (token.TokenKind == TokenKind.END_OF_INPUT)
                    break;
            }
            return tokens;
        }

        private Token NextToken()
        {
            if (_position >= _input.Length)
                return new Token(TokenKind.END_OF_INPUT, "", _line, _column);
            var character = _input[_position];

            if (char.IsLetter(character))
                return RecognizeIdentifier();
            if (char.IsDigit(character))
                return RecognizeDigit();
            if ("!=+-<>*/;,".Contains(character))
                return RecognizeOperator();
            if ("()".Contains(character))
                return RecognizeParenthesis();
            if ("{}".Contains(character))
                return RecognizeBrackets();
            if ("'".Contains(character))
                return RecognizeString();
            if ('\r' == character && _input[_position + 1] == '\n')
            {
                _position += 2;
                _line++;
                _column = 0;
                return NextToken();
            }
            if (' ' == character)
            {
                _position++;
                _column++;
                return NextToken();
            }
            throw new Exception("Error: Unrecognized token at line " + (_line + 1) + ", column " + (_column + 1));
        }
        private Token RecognizeIdentifier()
        {
            var line = _line;
            var column = _column;
            var identifier = "";
            while (_position < _input.Length)
            {
                var character = _input[_position];
                if (!char.IsLetterOrDigit(character) && (character != '_'))
                    break;
                identifier += character;
                _position++;
                _column++;
            }
            TokenKind identifierTokenKind;
            switch (identifier)
            {
                case "function":
                    identifierTokenKind = TokenKind.FUNCTION;
                    break;
                case "var":
                    identifierTokenKind = TokenKind.VARIABLE;
                    break;
                case "while":
                    identifierTokenKind = TokenKind.WHILE;
                    break;
                case "print":
                    identifierTokenKind = TokenKind.PRINT;
                    break;
                case "if":
                    identifierTokenKind = TokenKind.IF;
                    break;
                case "return":
                    identifierTokenKind = TokenKind.RETURN;
                    break;
                case "false":
                case "true":
                    identifierTokenKind = TokenKind.BOOLEAN;
                    break;
                default:
                    identifierTokenKind = TokenKind.IDENTIFIER;
                    break;
            }
            return new Token(identifierTokenKind, identifier, line, column);
        }

        private Token RecognizeDigit()
        {
            var column = _column;
            var line = _line;
            var digit = "";
            var isFloat = false;

            while (_position < _input.Length)
            {
                var character = _input[_position];
                if (!char.IsDigit(character))
                {
                    if (character == '.' && isFloat == false)
                        isFloat = true;
                    else
                        break;
                }
                digit += character;
                _position++;
                _column++;
            }
            return new Token(TokenKind.NUMBER, digit, line, column);
        }

        private Token RecognizeOperator()
        {
            var character = _input[_position];
            var line = _line;
            var column = _column;
            char nextCharacter = new char();

            if (_position + 1 < _input.Length)
                nextCharacter = _input[_position + 1];

            if ("=".Contains(nextCharacter))
            {
                _position += 2;
                _column += 2;
                return new Token(TokenKind.BINARY_OPERATOR, new string(new[] { character, nextCharacter }), line, column);
            }
            else
            {
                _position++;
                _column++;
                TokenKind operatorTokenKind;
                switch (character)
                {
                    case '=':
                        operatorTokenKind = TokenKind.ASSIGN;
                        break;
                    case ';':
                        operatorTokenKind = TokenKind.END_OF_STATEMENT;
                        break;
                    case ',':
                        operatorTokenKind = TokenKind.COMMA;
                        break;
                    default:
                        operatorTokenKind = TokenKind.BINARY_OPERATOR;
                        break;
                }
                return new Token(operatorTokenKind, character.ToString(), line, column);

            }
        }

        private Token RecognizeBrackets()
        {
            var line = _line;
            var column = _column;
            var bracket = _input[_position];
            _position++;
            _column++;

            if (bracket == '{')
                return new Token(TokenKind.L_BRACKET, bracket.ToString(), line, column);
            else
                return new Token(TokenKind.R_BRACKET, bracket.ToString(), line, column);
        }

        private Token RecognizeParenthesis()
        {
            var line = _line;
            var column = _column;
            var parenthesis = _input[_position];
            _position++;
            _column++;

            if (parenthesis == '(')
                return new Token(TokenKind.L_PAREN, parenthesis.ToString(), line, column);
            else
                return new Token(TokenKind.R_PAREN, parenthesis.ToString(), line, column);
        }

        private Token RecognizeString()
        {
            _position++;
            _column++;
            var line = _line;
            var column = _column;
            var text = "";

            while (!"'".Contains(_input[_position]))
            {
                text += _input[_position];
                _position++;
                _column++;
            }
            _position++;
            _column++;

            return new Token(TokenKind.STRING, text, line, column);

        }


    }
}