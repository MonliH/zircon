using System;
using ZirconLang.Diagnostics;
using ZirconLang.Parser;

namespace ZirconLang.Interpreter
{
    public class InterpreterVisitor : IExprVisitor<Value>
    {
        private Env _env;

        public InterpreterVisitor(Env? env = null)
        {
            _env = env ?? new Env();
        }

        public static Value Interpret(Env env, Expr expr)
        {
            return expr.Accept(new InterpreterVisitor(env));
        }

        public Value Visit(Expr.Integer expr)
        {
            return new Value.VInt(expr.Value, expr.Span);
        }

        public Value Visit(Expr.Float expr)
        {
            return new Value.VFloat(expr.Value, expr.Span);
        }

        public Value Visit(Expr.String expr)
        {
            return new Value.VString(expr.Value, expr.Span);
        }

        public Value Visit(Expr.Assignment assign)
        {
            _env.Define(assign.Name.Name, new Thunk(() => InterpreterVisitor.Interpret(_env, assign.Value)), assign.Span);
            return _env.Lookup(assign.Name);
        }

        public Value Visit(Expr.Sequence seq)
        {
            Value evaled = new Value.VUnit(seq.Span);
            foreach (Expr e in seq.Exprs)
            {
                evaled = InterpreterVisitor.Interpret(_env, e);
            }

            return evaled;
        }

        public Value Visit(Expr.Application app)
        {
            Value fn = InterpreterVisitor.Interpret(_env, app.Fn);
            if (fn.Type() != TypeName.Lambda)
                throw new ErrorBuilder().Msg($"expected a lambda, found a {fn.Type().Display()}").Type(ErrorType.Type)
                    .Span(fn.Span).Build();


            Value.VLambda lam = (Value.VLambda) fn;
            return lam.Lam(new Thunk(() => InterpreterVisitor.Interpret(_env, app.Arg)));
        }

        public Value Visit(Expr.Variable var)
        {
            return _env.Lookup(var);
        }

        public Value Visit(Expr.Lambda lam)
        {
            return new Value.VLambda(_env, lam.Arg, lam.Body, lam.Span);
        }
    }
}