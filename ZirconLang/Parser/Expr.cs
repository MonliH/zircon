using System.Collections.Generic;

namespace ZirconLang.Parser
{
    public abstract class Expr
    {
        public Span Span;

        public Expr(Span span)
        {
            Span = span;
        }

        public class String : Expr
        {
            public string Value;

            public String(string value, Span span) : base(span)
            {
                Value = value;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Float : Expr
        {
            public double Value;

            public Float(double value, Span span) : base(span)
            {
                Value = value;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Integer : Expr
        {
            public long Value;

            public Integer(long value, Span span) : base(span)
            {
                Value = value;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Application : Expr
        {
            public Expr Fn;
            public Expr Arg;

            public Application(Expr fn, Expr arg, Span span) : base(span)
            {
                Fn = fn;
                Arg = arg;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Assignment : Expr
        {
            public Variable Name;
            public Expr Value;

            public Assignment(Variable name, Expr value, Span span) : base(span)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Sequence : Expr
        {
            public List<Expr> Exprs;

            public Sequence(List<Expr> exprs, Span span) : base(span)
            {
                Exprs = exprs;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Variable : Expr
        {
            public string Name;
            public bool IsOperator;

            public Variable(string name, bool isOperator, Span span) : base(span)
            {
                Name = name;
                IsOperator = isOperator;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Lambda : Expr
        {
            public Variable Arg;
            public Expr Body;

            public Lambda(Variable arg, Expr body, Span span) : base(span)
            {
                Arg = arg;
                Body = body;
            }

            public override T Accept<T>(IExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }

    public interface IExprVisitor<out T>
    {
        public T Visit(Expr.Integer expr);
        public T Visit(Expr.Float expr);
        public T Visit(Expr.String expr);
        public T Visit(Expr.Application expr);
        public T Visit(Expr.Variable expr);
        public T Visit(Expr.Assignment expr);
        public T Visit(Expr.Sequence expr);
        public T Visit(Expr.Lambda expr);
    }
}