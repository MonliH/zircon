using System;
using ZirconLang.Diagnostics;
using ZirconLang.Interpreter;

namespace ZirconLang
{
    public class GlobalVars
    {
        private static Span GlobalSpan = new Span(0, 0, SourceMap.GlobalsSid);

        private Env _globals;

        public GlobalVars()
        {
            _globals = new Env();
            _globals.Define("print", new Thunk(() => new Value.VLambda((Thunk th) =>
            {
                Console.WriteLine(ValuePrinter.Print(th.Force()));
                return new Value.VUnit(GlobalSpan);
            }, GlobalSpan)), GlobalSpan);
            _globals.Define("unit", new Thunk(() => new Value.VUnit(GlobalSpan)), GlobalSpan);
            _globals.Define("add", new Thunk(() => new Value.VLambda((Thunk fst) =>
            {
                return new Value.VLambda((Thunk snd) =>
                {
                    Value first = fst.Force();
                    Value second = snd.Force();
                    TypeName fstTy = first.Type();
                    TypeName sndTy = second.Type();
                    Span combined = first.Span.Combine(second.Span);
                    switch ((fstTy, sndTy))
                    {
                        case (TypeName.Int, TypeName.Int):
                            return new Value.VInt(((Value.VInt) first).Value + ((Value.VInt) second).Value, combined);
                        case (TypeName.Real, TypeName.Real):
                            return new Value.VFloat(((Value.VFloat) first).Value + ((Value.VFloat) second).Value,
                                combined);
                        default:
                            throw new ErrorBuilder().Msg($"cannot add types {fstTy.Display()} and {sndTy.Display()}")
                                .Type(ErrorType.Type).Span(combined).Build();
                    }
                }, GlobalSpan);
            }, GlobalSpan)), GlobalSpan);
        }

        public Env GetGlobals()
        {
            return _globals;
        }
    }
}