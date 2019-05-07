using System;
using System.Collections.Generic;

namespace Lang.Scanner
{
    public enum TokenKind
    {
        // [a-zA-Z][a-zA-Z0-9_]*
        IDENTIFIER,
        // [0-9]* + [0-9]*.[0-9]*
        NUMBER,
        // ['.*']
        STRING,
        // [true] + [false]
        BOOLEAN,
        // [==] + [+] + [-] + [<] + [>]
        BINARY_OPERATOR,
        // [=]
        ASSIGN,
        // [()]
        L_PAREN,
        // [)]
        R_PAREN,
        // [{}]
        L_BRACKET,
        // [}]
        R_BRACKET,
        // end of file
        END_OF_INPUT,
        // [function]
        FUNCTION,
        UNKNOWN,
        // [print]
        PRINT,
        // [var]
        VARIABLE,
        // [while]
        WHILE,
        // [if]
        IF,
        // [return]
        RETURN,
        // [;]
        END_OF_STATEMENT,
        // [,]
        COMMA
    }


    public class Token
    {
        public TokenKind TokenKind { get; private set; }
        public string Value { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public Token(TokenKind tokenKind, string value, int line, int column)
        {
            this.TokenKind = tokenKind;
            this.Value = value;
            this.Line = line;
            this.Column = column;

        }
    }
}