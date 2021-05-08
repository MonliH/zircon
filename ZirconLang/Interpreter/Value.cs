using System;
using ZirconLang.Parser;

namespace ZirconLang.Interpreter
{
    public enum TypeName
    {
        Int,
        Real,
        Bool,
        String,
        Lambda,
        Unit
    }

    public static class TypeNameExtension
    {
        public static string Display(this TypeName typename)
        {
            switch (typename)
            {
                case TypeName.Int: return "integer";
                case TypeName.Real: return "real";
                case TypeName.Bool: return "boolean";
                case TypeName.String: return "string";
                case TypeName.Lambda: return "lambda";
                case TypeName.Unit: return "unit";
                default: return "unknown";
            }
        }
    }

    public abstract class Value
    {
        public abstract TypeName Type();
        public Span Span;

        public Value(Span span)
        {
            Span = span;
        }

        public class VUnit : Value
        {
            public VUnit(Span span) : base(span)
            {
            }

            public override TypeName Type()
            {
                return TypeName.Unit;
            }

            public override T Accept<T>(IValueVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }

        public class VInt : Value
        {
            public long Value;

            public VInt(long value, Span span) : base(span)
            {
                Value = value;
            }

            public override TypeName Type()
            {
                return TypeName.Int;
            }

            public override T Accept<T>(IValueVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }

        public class VBool : Value
        {
            public bool Value;

            public VBool(bool value, Span span) : base(span)
            {
                Value = value;
            }

            public override TypeName Type()
            {
                return TypeName.Bool;
            }

            public override T Accept<T>(IValueVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }

        public class VString : Value
        {
            public string Value;

            public VString(string value, Span span) : base(span)
            {
                Value = value;
            }

            public override TypeName Type()
            {
                return TypeName.String;
            }

            public override T Accept<T>(IValueVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }

        public class VFloat : Value
        {
            public double Value;

            public VFloat(double value, Span span) : base(span)
            {
                Value = value;
            }

            public override TypeName Type()
            {
                return TypeName.Real;
            }

            public override T Accept<T>(IValueVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }

        public class VLambda : Value
        {
            public Func<Thunk, Value> Lam;

            public VLambda(Env env, Expr.Variable binder, Expr expr, Span span) : base(span)
            {
                Lam = (th) => InterpreterVisitor.Interpret(new Env(env, binder.Name, th), expr);
            }

            public VLambda(Func<Thunk, Value> func, Span span) : base(span)
            {
                Lam = func;
            }

            public override TypeName Type()
            {
                return TypeName.Lambda;
            }

            public override T Accept<T>(IValueVisitor<T> visitor)
            {
                return visitor.visit(this);
            }
        }

        public abstract T Accept<T>(IValueVisitor<T> visitor);
    }

    public interface IValueVisitor<R>
    {
        public R visit(Value.VLambda value);
        public R visit(Value.VBool value);
        public R visit(Value.VFloat value);
        public R visit(Value.VInt value);
        public R visit(Value.VString value);
        public R visit(Value.VUnit value);
    }
}