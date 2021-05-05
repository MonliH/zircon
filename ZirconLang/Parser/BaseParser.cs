using System;
using System.Collections.Generic;
using ZirconLang.Diagnostics;
using ZirconLang.Lexer;

namespace ZirconLang.Parser
{
    public class BaseParser
    {
        private List<Token> _stream;
        private int _index;

        public BaseParser(List<Token> stream)
        {
            _index = 0;
            _stream = stream;
        }

        protected Token Consume(TokenType ty)
        {
            return Consume(ty, $"Expected {ty.Display()}, found {Peek().Ty.Display()}");
        }

        protected Token Consume(TokenType ty, string msg)
        {
            if (Check(ty)) return Advance();
            throw new ErrorBuilder()
                .Msg(msg)
                .Span(Peek().Span)
                .Type(ErrorType.Syntax)
                .Build();
        }

        protected bool Match(params TokenType[] tys)
        {
            foreach (TokenType ty in tys)
            {
                if (Check(ty))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        protected bool Match(params (TokenType, string)[] tys)
        {
            foreach (var (ty, contents) in tys)
            {
                if (Check(ty, contents))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        protected bool Check(TokenType tok)
        {
            if (IsAtEnd()) return false;
            return Peek().Ty == tok;
        }

        protected bool Check(TokenType tok, string contents)
        {
            if (IsAtEnd()) return false;
            var peeked = Peek();
            return peeked.Ty == tok && peeked.Contents == contents;
        }

        protected Token Advance()
        {
            if (!IsAtEnd()) _index++;
            return Prev();
        }

        protected Token Peek()
        {
            return _stream[_index];
        }

        protected Token Prev()
        {
            return _stream[_index - 1];
        }

        protected bool IsAtEnd()
        {
            return Peek().Ty == TokenType.Eof;
        }
    }
}