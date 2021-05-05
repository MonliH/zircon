using System.Linq;
using System.Text;

namespace ZirconLang.Parser
{
    public class AstPrinter : IExprVisitor<string>
    {
        string print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string Visit(Expr.Literal lit)
        {
            return lit.Value.ToString() ?? "null";
        }

        public string Visit(Expr.Application app)
        {
            return MakeParend("application", app.Fn, app.Arg);
        }

        public string Visit(Expr.Variable variable)
        {
            return variable.Name;
        }

        public string Visit(Expr.Assignment assign)
        {
            return MakeParend("assignment", assign.Name, assign.Value);
        }

        public string Visit(Expr.Sequence seq)
        {
            return MakeParend("seq", seq.Exprs.ToArray());
        }
        
        public string Visit(Expr.Lambda lam)
        {
            return MakeParend("lambda", lam.Arg, lam.Body);
        }

        public string Visit(Expr.Conditional cond)
        {
            return MakeParend(new string[4]
                {
                    "cond",
                    MakeParend("if", cond.IfBranch.Item1, cond.IfBranch.Item2),
                    MakeParend("elsif",
                        cond.ElsifBranches
                            .Select(
                                (c) => MakeParend("cond", c.Item1, c.Item2)
                            ).ToArray()
                    ),
                    MakeParend("else", cond.ElseBranch)
                }
            );
        }

        private string MakeParend(string str, params Expr[] exprs)
        {
            return MakeParend(new string[1] {str}, exprs);
        }

        private string MakeParend(string str, params string[] strings)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(").Append(str).Append(" ").Append(string.Join(" ", strings)).Append(")");
            return builder.ToString();
        }

        private string MakeParend(string[] strs, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(string.Join(" ", strs)).Append(" ");

            foreach (Expr expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }

            builder.Append(")");

            return builder.ToString();
        }
    }
}