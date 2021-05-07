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
            _globals.Define("print'", new Thunk(() => new Value.VLambda((Thunk th) =>
            {
                Console.WriteLine(ValuePrinter.Print(th.Force()));
                return new Value.VUnit(GlobalSpan);
            }, GlobalSpan)), GlobalSpan);
            _globals.Define("input", new Thunk(() =>
            {
                string input = Console.ReadLine()!;
                return new Value.VString(input, GlobalSpan);
            }), GlobalSpan);
            _globals.Define("unit", new Thunk(() => new Value.VUnit(GlobalSpan)), GlobalSpan);
            _globals.Define("true", new Thunk(() => new Value.VBool(true, GlobalSpan)), GlobalSpan);
            _globals.Define("false", new Thunk(() => new Value.VBool(false, GlobalSpan)), GlobalSpan);
            _globals.Define("quit", new Thunk(() => new Value.VLambda((code) =>
            {
                Value exitCode = code.Force();
                if (exitCode.Type() != TypeName.Int)
                    throw new ErrorBuilder()
                        .Msg($"invalid exit code type `{exitCode.Type().Display()}`").Span(exitCode.Span)
                        .Type(ErrorType.Type).Build();
                Environment.Exit((int) ((Value.VInt) exitCode).Value);
                throw new InvalidOperationException("shouldn't be here");
            }, GlobalSpan)), GlobalSpan);

            _globals.Define("lt", new Thunk(() => new Value.VLambda((Thunk fst) =>
                new Value.VLambda((Thunk snd) =>
                {
                    Value first = fst.Force();
                    Value second = snd.Force();
                    TypeName fstTy = first.Type();
                    TypeName sndTy = second.Type();
                    Span combined = first.Span.Combine(second.Span);
                    bool isLt;
                    switch ((fstTy, sndTy))
                    {
                        case (TypeName.Int, TypeName.Int):
                            isLt = ((Value.VInt) first).Value < ((Value.VInt) second).Value;
                            break;
                        case (TypeName.Real, TypeName.Real):
                            isLt = ((Value.VFloat) first).Value < ((Value.VFloat) second).Value;
                            break;
                        default:
                            throw new ErrorBuilder()
                                .Msg($"cannot compare types `{fstTy.Display()}` and `{sndTy.Display()}`")
                                .Type(ErrorType.Type).Span(combined).Build();
                    }

                    return new Value.VBool(isLt, combined);
                }, GlobalSpan), GlobalSpan)), GlobalSpan);

            _globals.Define("eq", new Thunk(() => new Value.VLambda((Thunk fst) =>
                new Value.VLambda((Thunk snd) =>
                {
                    Value first = fst.Force();
                    Value second = snd.Force();
                    TypeName fstTy = first.Type();
                    TypeName sndTy = second.Type();
                    Span combined = first.Span.Combine(second.Span);
                    bool isEq = false;
                    switch ((fstTy, sndTy))
                    {
                        case (TypeName.Int, TypeName.Int):
                            isEq = ((Value.VInt) first).Value == ((Value.VInt) second).Value;
                            break;
                        case (TypeName.Real, TypeName.Real):
                            isEq = ((Value.VFloat) first).Value == ((Value.VFloat) second).Value;
                            break;
                        case (TypeName.String, TypeName.String):
                            isEq = ((Value.VString) first).Value == ((Value.VString) second).Value;
                            break;
                        case (TypeName.Unit, TypeName.Unit):
                            isEq = true;
                            break;
                    }

                    return new Value.VBool(isEq, combined);
                }, GlobalSpan), GlobalSpan)), GlobalSpan);

            _globals.Define("if", new Thunk(() => new Value.VLambda((Thunk cond) => new Value.VLambda((Thunk fst) =>
                new Value.VLambda((Thunk snd) =>
                {
                    Value cnd = cond.Force();
                    if (cnd.Type() != TypeName.Bool)
                        throw new ErrorBuilder()
                            .Msg($"first operand of `if` must be boolean, not `{cnd.Type()}`")
                            .Type(ErrorType.Type).Span(cnd.Span).Build();
                    Value.VBool cd = (Value.VBool) cnd;
                    return cd.Value ? fst.Force() : snd.Force();
                }, GlobalSpan), GlobalSpan), GlobalSpan)), GlobalSpan);

            _globals.Define("div", new Thunk(() => new Value.VLambda((Thunk fst) =>
                new Value.VLambda((Thunk snd) =>
                {
                    Value first = fst.Force();
                    Value second = snd.Force();
                    TypeName fstTy = first.Type();
                    TypeName sndTy = second.Type();
                    Span combined = first.Span.Combine(second.Span);
                    switch ((fstTy, sndTy))
                    {
                        case (TypeName.Int, TypeName.Int):
                            return new Value.VInt(((Value.VInt) first).Value / ((Value.VInt) second).Value, combined);
                        case (TypeName.Real, TypeName.Real):
                            return new Value.VFloat(((Value.VFloat) first).Value / ((Value.VFloat) second).Value,
                                combined);
                        default:
                            throw new ErrorBuilder()
                                .Msg($"cannot divide types `{fstTy.Display()}` and `{sndTy.Display()}`")
                                .Type(ErrorType.Type).Span(combined).Build();
                    }
                }, GlobalSpan), GlobalSpan)), GlobalSpan);

            _globals.Define("mul", new Thunk(() => new Value.VLambda((Thunk fst) =>
                new Value.VLambda((Thunk snd) =>
                {
                    Value first = fst.Force();
                    Value second = snd.Force();
                    TypeName fstTy = first.Type();
                    TypeName sndTy = second.Type();
                    Span combined = first.Span.Combine(second.Span);
                    switch ((fstTy, sndTy))
                    {
                        case (TypeName.Int, TypeName.Int):
                            return new Value.VInt(((Value.VInt) first).Value * ((Value.VInt) second).Value, combined);
                        case (TypeName.Real, TypeName.Real):
                            return new Value.VFloat(((Value.VFloat) first).Value * ((Value.VFloat) second).Value,
                                combined);
                        default:
                            throw new ErrorBuilder()
                                .Msg($"cannot multiply types `{fstTy.Display()}` and `{sndTy.Display()}`")
                                .Type(ErrorType.Type).Span(combined).Build();
                    }
                }, GlobalSpan), GlobalSpan)), GlobalSpan);

            _globals.Define("mod", new Thunk(() => new Value.VLambda((Thunk fst) =>
                new Value.VLambda((Thunk snd) =>
                {
                    Value first = fst.Force();
                    Value second = snd.Force();
                    TypeName fstTy = first.Type();
                    TypeName sndTy = second.Type();
                    Span combined = first.Span.Combine(second.Span);
                    if (fstTy != TypeName.Int || sndTy != TypeName.Int)
                        throw new ErrorBuilder()
                            .Msg($"cannot modulo types `{fstTy.Display()}` and `{sndTy.Display()}`")
                            .Type(ErrorType.Type).Span(combined).Build();
                    return new Value.VInt(((Value.VInt) first).Value % ((Value.VInt) second).Value, combined);
                }, GlobalSpan), GlobalSpan)), GlobalSpan);

            _globals.Define("neg", new Thunk(() => new Value.VLambda((Thunk fst) =>
                {
                    Value first = fst.Force();
                    TypeName fstTy = first.Type();
                    switch (fstTy)
                    {
                        case TypeName.Int:
                            return new Value.VInt(-((Value.VInt) first).Value, first.Span);
                        case TypeName.Real:
                            return new Value.VFloat(-((Value.VFloat) first).Value, first.Span);
                        default:
                            throw new ErrorBuilder()
                                .Msg($"cannot negate the type `{fstTy.Display()}`")
                                .Type(ErrorType.Type).Span(first.Span).Build();
                    }
                }, GlobalSpan)), GlobalSpan);
            
            _globals.Define("sub", new Thunk(() => new Value.VLambda((Thunk fst) =>
                new Value.VLambda((Thunk snd) =>
                {
                    Value first = fst.Force();
                    Value second = snd.Force();
                    TypeName fstTy = first.Type();
                    TypeName sndTy = second.Type();
                    Span combined = first.Span.Combine(second.Span);
                    switch ((fstTy, sndTy))
                    {
                        case (TypeName.Int, TypeName.Int):
                            return new Value.VInt(((Value.VInt) first).Value - ((Value.VInt) second).Value, combined);
                        case (TypeName.Real, TypeName.Real):
                            return new Value.VFloat(((Value.VFloat) first).Value - ((Value.VFloat) second).Value,
                                combined);
                        default:
                            throw new ErrorBuilder()
                                .Msg($"cannot subtract types `{fstTy.Display()}` and `{sndTy.Display()}`")
                                .Type(ErrorType.Type).Span(combined).Build();
                    }
                }, GlobalSpan), GlobalSpan)), GlobalSpan);

            _globals.Define("add",
                new Thunk(() =>
                    new Value.VLambda((Thunk fst) =>
                            new Value.VLambda((Thunk snd) =>
                            {
                                Value first = fst.Force();
                                Value second = snd.Force();
                                TypeName fstTy = first.Type();
                                TypeName sndTy = second.Type();

                                Span combined = first.Span.Combine(second.Span);
                                switch ((fstTy, sndTy))
                                {
                                    case (TypeName.Int, TypeName.Int):
                                        return new Value.VInt(((Value.VInt) first).Value + ((Value.VInt) second).Value,
                                            combined);
                                    case (TypeName.Real, TypeName.Real):
                                        return new Value.VFloat(
                                            ((Value.VFloat) first).Value + ((Value.VFloat) second).Value,
                                            combined);
                                    case (TypeName.String, TypeName.String):
                                        return new Value.VString(
                                            ((Value.VString) first).Value + ((Value.VString) second).Value,
                                            combined);
                                    default:
                                        throw new ErrorBuilder()
                                            .Msg($"cannot add types `{fstTy.Display()}` and `{sndTy.Display()}`")
                                            .Type(ErrorType.Type).Span(combined).Build();
                                }
                            }, GlobalSpan),
                        GlobalSpan)), GlobalSpan);
        }

        public Env GetGlobals()
        {
            return _globals;
        }
    }
}