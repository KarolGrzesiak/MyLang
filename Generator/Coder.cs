using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Lang.Parser;

namespace Lang.Generator
{
    public class NodeVisitor
    {
        public object Visit(Coder coder, INode node)
        {
            return typeof(Coder).GetMethod("Visit" + node.GetType().Name).Invoke(coder, new object[] { node });

        }
    }

    public class Coder
    {
        private readonly Compound _ast;
        private int _depth;
        private List<string> _declaredIdentifiers;
        private NodeVisitor _nodeVisitor;

        public Coder(Compound ast)
        {
            _ast = ast;
            _depth = 0;
            _declaredIdentifiers = new List<string>();
            _nodeVisitor = new NodeVisitor();
        }

        public object GenerateCode()
        {
            return _nodeVisitor.Visit(this, _ast);
        }

        public string VisitCompound(Compound compound)
        {
            var block = "";
            var indent = "".PadLeft(4 * _depth);
            foreach (var child in compound.Children)
            {
                _depth++;
                var line = indent + _nodeVisitor.Visit(this, child);
                _depth--;
                block = block + line + "\n";
            }
            return block.Length > 0 ? block : indent + "pass\n";
        }

        public string VisitPrintStatement(PrintStatement printStatement)
        {
            return "print(" + _nodeVisitor.Visit(this, printStatement.Expression) + ")";
        }
        public string VisitStringLiteral(StringLiteral stringLiteral)
        {
            return "\"" + stringLiteral.Value + "\"";
        }
        public string VisitNumberLiteral(NumberLiteral numberLiteral)
        {
            return (numberLiteral.Value);
        }
        public string VisitBooleanLiteral(BooleanLiteral booleanLiteral)
        {
            return (booleanLiteral.Value == "true") ? "True" : "False";
        }

        public string VisitBinaryOperation(BinaryOperation binaryOperation)
        {
            return _nodeVisitor.Visit(this, binaryOperation.LeftExpression) + binaryOperation.Value + _nodeVisitor.Visit(this, binaryOperation.RightExpression);
        }
        public string VisitDeclaration(Declaration declaration)
        {
            _declaredIdentifiers.Add(declaration.Identifier.Value);
            return _nodeVisitor.Visit(this, declaration.Identifier) + "=" + _nodeVisitor.Visit(this, declaration.Expression);
        }
        public string VisitAssignment(Assignment assignment)
        {
            return _nodeVisitor.Visit(this, assignment.Identifier) + "=" + _nodeVisitor.Visit(this, assignment.Expression);
        }

        public string VisitIdentifier(Identifier identifier)
        {
            if (!_declaredIdentifiers.Contains(identifier.Value))
                IdentifierNotDeclared(identifier.Value);
            return identifier.Value;
        }
        public void IdentifierNotDeclared(string identifier)
        {
            throw new Exception("Code generator error: Identifier \"" + identifier + "\" undefined");
        }
        public string VisitIfStatement(IfStatement ifStatement)
        {
            return "if " + _nodeVisitor.Visit(this, ifStatement.Condition) + ":\n" + _nodeVisitor.Visit(this, ifStatement.Body);
        }
        public string VisitCondition(Condition condition)
        {
            return "(" + _nodeVisitor.Visit(this, condition.LeftExpression) + condition.Value + _nodeVisitor.Visit(this, condition.RightExpression) + ")";
        }
        public string VisitWhileLoop(WhileLoop whileLoop)
        {
            return "while " + _nodeVisitor.Visit(this, whileLoop.Condition) + ":\n" + _nodeVisitor.Visit(this, whileLoop.Body);
        }
        public string VisitReturnStatement(ReturnStatement returnStatement)
        {
            return "return " + _nodeVisitor.Visit(this, returnStatement.Expression);
        }
        public string VisitFunctionDefinition(FunctionDefinition functionDefinition)
        {
            _declaredIdentifiers.Add(functionDefinition.Identifier.Value);

            return "def " + _nodeVisitor.Visit(this, functionDefinition.Identifier) + "(" + _nodeVisitor.Visit(this, functionDefinition.Arguments) + "):\n" + _nodeVisitor.Visit(this, functionDefinition.Body);

        }
        public string VisitIdentifiers(Identifiers identifiers)
        {
            var result = new StringBuilder();
            foreach (var child in identifiers.Children)
            {
                _declaredIdentifiers.Add(child.Value);
                result.Append(_nodeVisitor.Visit(this, child) + ",");
            }
            if (result.Length > 0)
                result.Length--;

            return result.ToString();
        }

        public string VisitFunctionCall(FunctionCall functionCall)
        {
            var result = _nodeVisitor.Visit(this, functionCall.Identifier) + "(";
            var arguments = new StringBuilder();
            foreach (var argument in functionCall.Arguments)
            {
                arguments.Append(_nodeVisitor.Visit(this, argument) + ",");
            }
            if (arguments.Length > 0)
                arguments.Length--;
            return result + arguments + ")";
        }

    }
}