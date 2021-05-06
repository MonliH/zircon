using System;
using System.Collections.Generic;
using ZirconLang.Diagnostics;
using ZirconLang.Parser;

namespace ZirconLang.Interpreter
{
    public class Env
    {
        private Dictionary<string, Thunk> _vars;
        private Env? _enclosing;

        public Env()
        {
            _vars = new Dictionary<string, Thunk>();
            _enclosing = null;
        }

        public Env(Env enclosing, string name, Thunk val)
        {
            _enclosing = enclosing;
            _vars = new Dictionary<string, Thunk> {{name, val}};
        }

        public void Define(string name, Thunk val, Span span)
        {
            try
            {
                _vars.Add(name, val);
            }
            catch (ArgumentException)
            {
                throw new ErrorBuilder().Msg($"cannot redefine the binding `{name}`").Type(ErrorType.Scope).Span(span)
                    .Build();
            }
        }

        public Value Lookup(Expr.Variable name)
        {
            if (_vars.ContainsKey(name.Name)) return _vars[name.Name].Force();
            if (_enclosing != null) return _enclosing.Lookup(name);

            throw new ErrorBuilder().Msg($"Undefined variable `{name.Name}`").Type(ErrorType.UnboundVariable)
                .Span(name.Span).Build();
        }
    }
}