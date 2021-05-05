using System;
using System.Collections.Generic;
using System.Data;

namespace ZirconLang.Parser
{
    public abstract class Expr
    {
        public class Conditional : Expr
        {
            public (Expr, Expr) IfBranch;
            public List<(Expr, Expr)> ElsifBranches;
            public Expr Else;
            
            public override T Accept<T>(ExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Literal : Expr
        {
            public Object value;
            
            public override T Accept<T>(ExprVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public interface ExprVisitor<out T>
        {
            public T Visit(Conditional expr);
            public T Visit(Literal expr);
        }
        
        public abstract T Accept<T>(ExprVisitor<T> visitor);
    }
}