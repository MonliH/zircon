using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZirconLang.Diagnostics
{
    public enum ErrorType
    {
        Lexer = 0,
        Syntax = 1,
        UnboundVariable = 2,
        Type = 3,
        Scope = 4,
        Runtime = 5,
    }

    static class ErrorTypeMethods
    {
        public static string Name(this ErrorType ty)
        {
            switch (ty)
            {
                case ErrorType.Syntax: return "syntax";
                case ErrorType.Lexer: return "lexer";
                case ErrorType.UnboundVariable: return "unbound_variable";
                case ErrorType.Type: return "type";
                case ErrorType.Scope: return "scope";
                case ErrorType.Runtime: return "runtime";
                default: return "";
            }
        }
    }

    public class ErrorBuilder
    {
        private Span? _errorSpan;
        private string? _errorMsg;
        private ErrorType? _ty;

        public ErrorBuilder Span(Span s)
        {
            _errorSpan = s;
            return this;
        }

        public ErrorBuilder Msg(string s)
        {
            _errorMsg = s;
            return this;
        }

        public ErrorBuilder Type(ErrorType ty)
        {
            _ty = ty;
            return this;
        }

        public Error Build()
        {
            if (_errorMsg != null && _errorSpan != null && _ty != null)
            {
                return new Error((Span) _errorSpan, _errorMsg, (ErrorType) _ty);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }

    public abstract class ErrorDisplay : Exception
    {
        public abstract void DisplayError(SourceMap sourceMap);
    }

    public class Error : ErrorDisplay
    {
        private Span _errorSpan;
        private string _errorMsg;
        private ErrorType _type;
        private List<Span> _trace;

        public Error(Span errorSpan, string errorMsg, ErrorType type)
        {
            this._errorSpan = errorSpan;
            this._errorMsg = errorMsg;
            this._type = type;
            _trace = new List<Span>();
        }

        public void AddTrace(Span trace)
        {
            _trace.Add(trace);
        }

        private (string, int, int, int, int) GetHumanPos(Span sp, SourceMap sourceMap)
        {
            string filename = sourceMap.LookupFilename(sp.Sid);
            string file = sourceMap.LookupSource(sp.Sid);
            var (linenoS, colnoS) = Span.LineCol(sp.S, file);
            var (linenoE, colnoE) = Span.LineCol(sp.E, file);
            return (filename, linenoS, colnoS, linenoE, colnoE);
        }

        private string FormatLines(string[] lines, int lineS, int colS, int lineE, int colE)
        {
            StringBuilder builder = new StringBuilder();
            int digits = (int) Math.Floor(Math.Log10(lineE) + 1);
            for (int lIdx = lineS; lIdx <= lineE; lIdx++)
            {
                string line = lines[lIdx - 1];
                builder.Append(
                    $"  {Color.Reset.ToS()}{Color.NicePurple.ToS()}{lIdx.ToString().PadLeft(digits, ' ')}{Color.Reset.ToS()} | ");

                if (lineS == lineE)
                {
                    // Starting line
                    builder.Append($"{line.Substring(0, colS - 1)}{Color.Bold.ToS()}{Color.Red.ToS()}");
                    builder.Append(line.Substring(colS - 1, colE - colS));
                    builder.Append(Color.Reset.ToS());
                    builder.Append(line.Substring(colE - 1));
                    builder.Append("\n");
                    continue;
                }

                if (lIdx == lineS)
                {
                    // Starting line
                    builder.Append($"{line.Substring(0, colS - 1)}{Color.Bold.ToS()}{Color.Red.ToS()}");
                    builder.Append(line.Substring(colS - 1));
                }
                else if (lIdx > lineS && lIdx < lineE)
                {
                    // In between lines
                    builder.Append(Color.Red.ToS());
                    builder.Append(Color.Bold.ToS());
                    builder.Append(line);
                }
                else
                {
                    // Last line
                    builder.Append(Color.Red.ToS());
                    builder.Append(Color.Bold.ToS());
                    builder.Append(line.Substring(0, colE - 1));
                    builder.Append(Color.Reset.ToS());
                    builder.Append(line.Substring(colE - 1));
                }

                builder.Append(Color.Reset.ToS());
                builder.Append('\n');
            }

            return builder.ToString();
        }

        private string FormatLocation(Span sp, SourceMap sourceMap)
        {
            var (filename, linenoS, colnoS, linenoE, colnoE) = GetHumanPos(sp, sourceMap);
            return $"{filename}:{linenoS}:{colnoS}-{linenoE}:{colnoE}";
        }

        private void FormatEntry(Span sp, SourceMap sourceMap, string additional)
        {
            Console.Write($"{Color.NicePurple.ToS()}{FormatLocation(sp, sourceMap)}{Color.Reset.ToS()}: ");
            Console.WriteLine($"{Color.Bold.ToS()}{Color.Red.ToS()}{additional}{Color.Reset.ToS()}");
            var (_, a, b, c, d) = GetHumanPos(sp, sourceMap);
            Console.Write(FormatLines(sourceMap.LookupSource(sp.Sid).Split("\n"), a, b, c, d));
        }
        
        public override void DisplayError(SourceMap sourceMap)
        {
            if (_trace.Any())
            {
                _trace.RemoveAt(_trace.Count - 1);
                _trace.Reverse();
                _trace.RemoveAt(_trace.Count - 1);
                foreach (Span trace in _trace)
                {
                    FormatEntry(trace, sourceMap, ColorExt.Surround("trace", Color.Red));
                }
            }

            string newErrMsg = ColorExt.Surround(_errorMsg, Color.Bold);
            string errName = ColorExt.Surround($"{_type.Name()}_error", Color.Bold, Color.Red);
            FormatEntry(_errorSpan, sourceMap, $"{errName}: {newErrMsg}");
        }
    }

    public class Errors : ErrorDisplay
    {
        private readonly List<Error> _errs;

        public Errors(List<Error> errs)
        {
            this._errs = errs;
        }

        public override void DisplayError(SourceMap sourceMap)
        {
            foreach (Error err in _errs)
            {
                err.DisplayError(sourceMap);
            }
        }
    }
}