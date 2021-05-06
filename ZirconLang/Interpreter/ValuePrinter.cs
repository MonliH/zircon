using System;

namespace ZirconLang.Interpreter
{
    public class ValuePrinter : IValueVisitor<string>
    {
        public static string Print(Value val)
        {
            return val.Accept(new ValuePrinter());
        }

        public string visit(Value.VLambda lam)
        {
            return "<lambda>";
        }
        
        public string visit(Value.VBool bo)
        {
            return bo.Value ? "true" : "false";
        }

        public string visit(Value.VFloat fl)
        {
            return fl.Value.ToString();
        }
        
        public string visit(Value.VInt num)
        {
            return num.Value.ToString();
        }
        
        public string visit(Value.VString s)
        {
            return s.Value;
        }
        
        public string visit(Value.VUnit _)
        {
            return "unit";
        }
    }
}