using System;
using System.Collections.Generic;

namespace Lang.Parser
{
    //numberliteral identfier function call binary operation
    public interface INode { }
    public interface IExpression : INode { }
    public class Compound : INode
    {
        public Compound()
        {
            Children = new List<INode>();
        }

        public List<INode> Children { get; }
    }

    public class WhileLoop : INode
    {
        public WhileLoop(Condition condition, Compound body)
        {
            Condition = condition;
            Body = body;
        }

        public Condition Condition { get; }
        public Compound Body { get; }
    }

    public class IfStatement : INode
    {
        public IfStatement(Condition condition, Compound body)
        {
            Condition = condition;
            Body = body;
        }

        public Condition Condition { get; }
        public Compound Body { get; }
    }

    public class NumberLiteral : IExpression
    {
        public NumberLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class StringLiteral : IExpression
    {
        public StringLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class BooleanLiteral : IExpression
    {
        public BooleanLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class Identifier : IExpression
    {
        public Identifier(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class Declaration : INode
    {
        public Declaration(Identifier identifier, IExpression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public Identifier Identifier { get; }
        public IExpression Expression { get; }
    }

    public class Assignment : INode
    {
        public Assignment(Identifier identifier, IExpression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public Identifier Identifier { get; }
        public IExpression Expression { get; }
    }

    public class BinaryOperation : IExpression
    {
        public BinaryOperation(string value, IExpression leftExpression, IExpression rightExpression)
        {
            Value = value;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        public string Value { get; }
        public IExpression LeftExpression { get; }
        public IExpression RightExpression { get; }
    }

    public class PrintStatement : INode
    {
        public PrintStatement(IExpression expression)
        {
            Expression = expression;
        }

        public IExpression Expression { get; }
    }

    public class ReturnStatement : INode
    {
        public ReturnStatement(IExpression expression)
        {
            Expression = expression;
        }

        public IExpression Expression { get; }
    }

    public class FunctionDefinition : INode
    {
        public FunctionDefinition(Identifier identifier, Identifiers arguments, Compound body)
        {
            Identifier = identifier;
            Arguments = arguments;
            Body = body;
        }

        public Identifier Identifier { get; }
        public Identifiers Arguments { get; }
        public Compound Body { get; }
    }

    public class Identifiers : INode
    {
        public Identifiers()
        {
            Children = new List<Identifier>();
        }

        public List<Identifier> Children { get; set; }
    }

    public class FunctionCall : IExpression
    {
        public FunctionCall(Identifier identifier, List<IExpression> arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
        }

        public Identifier Identifier { get; }
        public List<IExpression> Arguments { get; }
    }


    public class Condition : INode
    {
        public Condition(string value, IExpression leftExpression, IExpression rightExpression)
        {
            Value = value;
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        public string Value { get; }
        public IExpression LeftExpression { get; }
        public IExpression RightExpression { get; }
    }

    public class Epsilon : INode
    {

    }


}