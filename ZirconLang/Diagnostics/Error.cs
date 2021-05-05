using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZirconLang.Diagnostics
{
    public enum ErrorType
    {
        Lexer = 0,
        Syntax = 1,
    }

    static class ErrorTypeMethods
    {
        public static string Name(this ErrorType ty)
        {
            switch (ty)
            {
                case ErrorType.Syntax: return "syntax";
                case ErrorType.Lexer: return "lexer";
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
                return new Error(_errorSpan, _errorMsg, (ErrorType)_ty);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
    }

    public class Error
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

        public void DisplayError(SourceMap sourceMap)
        {
            string filename = sourceMap.LookupFilename(errorSpan.Sid);
            Console.WriteLine($"{filename}:{errorSpan.S}-{errorSpan.E}: {type.Name()}: {errorMsg}");
        }
    }

    public class Errors : Exception
    {
        private readonly List<Error> _errs;

        public Errors(List<Error> errs)
        {
            this._errs = errs;
        }

        public void DisplayErrors(SourceMap sourceMap)
        {
            foreach (Error err in _errs)
            {
                err.DisplayError(sourceMap);
            }
        }
    }
}