using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZirconLang.Diagnostics
{
    public enum ErrorType
    {
        Lexer = 0,
        Syntax = 1,
        UnboundVariable = 2,
        Type = 3,
        Scope = 4,
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
                return new Error((Span)_errorSpan, _errorMsg, (ErrorType) _ty);
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
        private Span errorSpan;
        private string errorMsg;
        private ErrorType type;

        public Error(Span errorSpan, string errorMsg, ErrorType type)
        {
            this.errorSpan = errorSpan;
            this.errorMsg = errorMsg;
            this.type = type;
        }
        
        public override void DisplayError(SourceMap sourceMap)
        {
            string filename = sourceMap.LookupFilename(errorSpan.Sid);
            string file = sourceMap.LookupSource(errorSpan.Sid);
            var (linenoS, colnoS) = Span.LineCol(errorSpan.S, file);
            var (linenoE, colnoE) = Span.LineCol(errorSpan.E, file);
            Console.WriteLine($"{filename}:{linenoS}:{colnoS}-{linenoE}:{colnoE}: {type.Name()}_error: {errorMsg}");
        }
    }

    public class Errors : ErrorDisplay
    {
        private readonly List<ErrorDisplay> _errs;

        public Errors(List<ErrorDisplay> errs)
        {
            this._errs = errs;
        }

        public override void DisplayError(SourceMap sourceMap)
        {
            foreach (ErrorDisplay err in _errs)
            {
                err.DisplayError(sourceMap);
            }
        }
    }
}