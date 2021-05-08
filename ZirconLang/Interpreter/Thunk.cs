using System;

namespace ZirconLang.Interpreter
{
    public class Thunk
    {
        private Func<Value> _thunk;

        public Thunk(Func<Value> thunk)
        {
            _thunk = thunk;
        }

        public void Update(Value val)
        {
            _thunk = () => val;
        }

        public Value Force()
        {
            Value val = _thunk();
            Update(val);
            return val;
        }
    }
}