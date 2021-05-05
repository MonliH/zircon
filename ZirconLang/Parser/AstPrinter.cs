using System.Globalization;
using System.Text;

namespace ZirconLang.Parser
{
    public class AstPrinter : IExprVisitor<string>
    {
        private int _indentation;
        
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string Visit(Expr.Float lit)
        {
            return lit.Value.ToString(CultureInfo.CurrentCulture);
        }
        
        public string Visit(Expr.String lit)
        {
            return $"\"{lit.Value}\"";
        }
        
        public string Visit(Expr.Integer lit)
        {
            return lit.Value.ToString();
        }

        public string Visit(Expr.Application app)
        {
            return MakeParend("app", app.Fn, app.Arg);
        }

        public string Visit(Expr.Variable variable)
        {
            return $"`{variable.Name}`";
        }

        public string Visit(Expr.Assignment assign)
        {
            return MakeParend("assign", assign.Name, assign.Value);
        }

        public string Visit(Expr.Sequence seq)
        {
            return MakeParend("seq", seq.Exprs.ToArray());
        }
        
        public string Visit(Expr.Lambda lam)
        {
            return MakeParend("lam", lam.Arg, lam.Body);
        }

        private string MakeParend(string str, params Expr[] exprs)
        {
            return MakeParend(new string[1] {str}, exprs);
        }

        private string Indent()
        {
            return new string(' ', _indentation * 2);
        }

        private string MakeParend(string[] strs, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();
            ++_indentation;

            builder.Append("(\n").Append(Indent()).Append(string.Join(" ", strs));

            foreach (Expr expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }

            --_indentation;
            builder.Append($"\n{Indent()})");

            return builder.ToString();
        }
    }
}