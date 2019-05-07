using System;
using System.Collections.Generic;
using Lang.Scanner;

namespace Lang.Parser
{
    public class Analyzer
    {
        private readonly List<Token> _tokens;
        private int _position;
        private Token _currentToken;

        public Analyzer(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
            _currentToken = _tokens[_position];

        }

        private void Move(TokenKind expectedTokenKind)
        {
            if (_currentToken.TokenKind == expectedTokenKind)
                NextToken();
            else
                Error(expectedTokenKind);
        }

        private void Error(TokenKind expectedTokenKind)
        {
            var line = _currentToken.Line + 1;
            var column = _currentToken.Column + 1;
            throw new Exception("Parser error: Expected: " + expectedTokenKind + ", but found: " + _currentToken.Value + " at line: " + line + " column: " + column);
        }

        private void NextToken()
        {
            _position++;
            _currentToken = _tokens[_position];
        }

        public Compound Parse()
        {
            var root = Compound();
            if (_currentToken.TokenKind != TokenKind.END_OF_INPUT)
                Error(TokenKind.END_OF_INPUT);
            return root;
        }

        private Compound Compound()
        {
            Move(TokenKind.L_BRACKET);
            var nodes = StatementList();
            Move(TokenKind.R_BRACKET);
            var root = new Compound();

            foreach (var node in nodes)
            {
                root.Children.Add(node);
            }
            return root;
        }

        private List<INode> StatementList()
        {
            var statementList = new List<INode>();
            while (_currentToken.TokenKind != TokenKind.R_BRACKET)
            {
                var node = Statement();
                statementList.Add(node);
            }
            return statementList;
        }

        private INode Statement()
        {
            INode node;
            if (_currentToken.TokenKind == TokenKind.IDENTIFIER)
            {
                if (_tokens[_position + 1].TokenKind == TokenKind.L_PAREN)
                {
                    node = FunctionCall();
                    Move(TokenKind.END_OF_STATEMENT);
                }
                else
                {
                    node = Assignment();
                }
            }
            else if (_currentToken.TokenKind == TokenKind.VARIABLE)
            {
                node = VarDeclaration();
            }
            else if (_currentToken.TokenKind == TokenKind.PRINT)
            {
                node = PrintStatement();
            }
            else if (_currentToken.TokenKind == TokenKind.WHILE)
            {
                node = WhileLoop();
            }
            else if (_currentToken.TokenKind == TokenKind.IF)
            {
                node = IfStatement();
            }
            else if (_currentToken.TokenKind == TokenKind.FUNCTION)
            {
                node = FunctionDefinition();
            }
            else if (_currentToken.TokenKind == TokenKind.RETURN)
            {
                node = ReturnStatement();
            }
            else
            {
                var line = _currentToken.Line + 1;
                var column = _currentToken.Column + 1;
                throw new Exception("Parser error: Unexcepted token " + _currentToken.Value + ", Excepted Statement at line: " + line + " column: " + column);

            }
            return node;
        }

        private Assignment Assignment()
        {
            var variable = Identifier();
            Move(TokenKind.ASSIGN);
            var assignable = Assignable();
            Move(TokenKind.END_OF_STATEMENT);
            var node = new Assignment(variable, assignable);
            return node;
        }
        private Declaration VarDeclaration()
        {
            Move(TokenKind.VARIABLE);
            var variable = Identifier();
            Move(TokenKind.ASSIGN);
            var assignable = Assignable();
            Move(TokenKind.END_OF_STATEMENT);
            var node = new Declaration(variable, assignable);
            return node;
        }
        private PrintStatement PrintStatement()
        {
            Move(TokenKind.PRINT);
            var expression = Assignable();
            Move(TokenKind.END_OF_STATEMENT);
            var node = new PrintStatement(expression);
            return node;
        }
        private IfStatement IfStatement()
        {
            Move(TokenKind.IF);
            var condition = Condition();
            var body = Compound();
            var node = new IfStatement(condition, body);
            return node;
        }

        private WhileLoop WhileLoop()
        {
            Move(TokenKind.WHILE);
            var condition = Condition();
            var body = Compound();
            var node = new WhileLoop(condition, body);
            return node;
        }
        private FunctionDefinition FunctionDefinition()
        {
            Move(TokenKind.FUNCTION);
            var identifier = Identifier();
            var arguments = Identifiers();
            var body = Compound();
            var node = new FunctionDefinition(identifier, arguments, body);
            return node;
        }
        private Identifiers Identifiers()
        {
            Move(TokenKind.L_PAREN);
            var identifiers = new Identifiers();
            while (_currentToken.TokenKind == TokenKind.IDENTIFIER)
            {
                var token = _currentToken;
                Move(TokenKind.IDENTIFIER);
                identifiers.Children.Add(new Identifier(token.Value));
                if (_currentToken.TokenKind == TokenKind.COMMA)
                    Move(TokenKind.COMMA);
                else
                    break;
            }
            Move(TokenKind.R_PAREN);
            return identifiers;
        }

        private ReturnStatement ReturnStatement()
        {
            Move(TokenKind.RETURN);
            var expression = Expression();
            Move(TokenKind.END_OF_STATEMENT);
            var node = new ReturnStatement(expression);
            return node;
        }

        private Condition Condition()
        {
            Move(TokenKind.L_PAREN);
            var leftExpression = Expression();
            var value = _currentToken.Value;
            Move(TokenKind.BINARY_OPERATOR);
            var rightExpression = Expression();
            Move(TokenKind.R_PAREN);
            var node = new Condition(value, leftExpression, rightExpression);
            return node;
        }
        private IExpression Assignable()
        {
            if (_currentToken.TokenKind == TokenKind.BOOLEAN)
                return this.Boolean();
            if (_currentToken.TokenKind == TokenKind.STRING)
                return this.String();
            else
                return Expression();

        }

        private IExpression Expression()
        {
            var node = Term();
            while (_currentToken.Value.Contains('+') || _currentToken.Value.Contains('-'))
            {
                var token = _currentToken;
                Move(TokenKind.BINARY_OPERATOR);
                node = new BinaryOperation(token.Value, node, Term());
            }
            return node;
        }
        private IExpression Term()
        {
            var node = Factor();
            while (_currentToken.Value.Contains('*') || _currentToken.Value.Contains('/'))
            {
                var token = _currentToken;
                Move(TokenKind.BINARY_OPERATOR);
                node = new BinaryOperation(token.Value, node, Factor());
            }
            return node;
        }
        private IExpression Factor()
        {
            var token = _currentToken;
            if (token.TokenKind == TokenKind.NUMBER)
            {
                Move(TokenKind.NUMBER);
                return new NumberLiteral(token.Value);
            }
            else if (token.TokenKind == TokenKind.IDENTIFIER)
            {
                if (_tokens[_position + 1].TokenKind == TokenKind.L_PAREN)
                    return FunctionCall();
                else
                {
                    Move(TokenKind.IDENTIFIER);
                    return new Identifier(token.Value);
                }
            }
            else if (token.TokenKind == TokenKind.L_PAREN)
            {
                Move(TokenKind.L_PAREN);
                var node = Expression();
                Move(TokenKind.R_PAREN);
                return node;
            }
            else if (token.TokenKind == TokenKind.BOOLEAN)
            {
                Move(TokenKind.BOOLEAN);
                return new BooleanLiteral(token.Value);
            }
            else if (token.TokenKind == TokenKind.STRING)
            {
                Move(TokenKind.STRING);
                return new StringLiteral(token.Value);
            }
            else
            {
                var line = _currentToken.Line + 1;
                var column = _currentToken.Column + 1;
                throw new Exception("Parser error: Unexcepted token " + _currentToken.Value + ", Excepted Number or Identifier at line: " + line + " column: " + column);
            }
        }
        private FunctionCall FunctionCall()
        {
            var identifier = new Identifier(_currentToken.Value);
            Move(TokenKind.IDENTIFIER);
            Move(TokenKind.L_PAREN);
            var arguments = ArgumentsList();
            Move(TokenKind.R_PAREN);
            return new FunctionCall(identifier, arguments);
        }
        private List<IExpression> ArgumentsList()
        {
            var arguments = new List<IExpression>();
            while (_currentToken.TokenKind != TokenKind.R_PAREN)
            {
                arguments.Add(Assignable());
                if (_currentToken.TokenKind == TokenKind.COMMA)
                    Move(TokenKind.COMMA);
                else
                    return arguments;
            }
            return arguments;
        }
        private BooleanLiteral Boolean()
        {
            var token = _currentToken;
            Move(TokenKind.BOOLEAN);
            return new BooleanLiteral(token.Value);
        }
        private StringLiteral String()
        {
            var token = _currentToken;
            Move(TokenKind.STRING);
            return new StringLiteral(token.Value);
        }

        private Identifier Identifier()
        {
            var node = new Identifier(_currentToken.Value);
            Move(TokenKind.IDENTIFIER);
            return node;
        }
        private Epsilon Epsilon()
        {
            return new Epsilon();
        }
    }
}