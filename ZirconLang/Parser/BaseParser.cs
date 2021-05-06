using System.Collections.Generic;
using ZirconLang.Diagnostics;
using ZirconLang.Lexer;

namespace ZirconLang.Parser
{
    public class BaseParser : Failable
    {
        private List<Token> _stream;
        private int _index;

        protected BaseParser(List<Token> stream)
        {
            _index = 0;
            _stream = stream;
        }

        protected Token Consume(TokenType ty)
        {
            return Consume(ty, $"Expected {ty.Display()}, found {Peek().Ty.Display()}");
        }
        
        protected Token Consume(string value, TokenType ty)
        {
            return Consume(value, ty, $"Expected {ty.Display()} `{value}`, found {Peek().Ty.Display()}");
        }
        
        protected Token Consume(string value, TokenType ty, string msg)
        {
            if (Check(ty, value)) return Advance();
            throw new ErrorBuilder()
                .Msg(msg)
                .Span(Peek().Span)
                .Type(ErrorType.Syntax)
                .Build();
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

        protected Span CurrentSpan()
        {
            return Peek().Span;
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

        protected bool CheckNext (TokenType tok)
        {
            if (IsAtEnd()) return false;
            return PeekNext().Ty == tok;
        }
        
        protected bool Check(TokenType tok)
        {
            if (IsAtEnd()) return false;
            return Peek().Ty == tok;
        }

        protected bool Check(TokenType tok, string contents)
        {
            if (IsAtEnd()) return false;
            var (s, tokenType, _) = Peek();
            return tokenType == tok && s == contents;
        }

        protected void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Prev().Ty == TokenType.LineBreak) return;
                switch (Peek().Ty)
                {
                    case TokenType.Let:
                    case TokenType.Prefix:
                    case TokenType.Postfix:
                    case TokenType.Binary:
                        return;
                }

                Advance();
            }
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
        
        protected Token PeekNext()
        {
            return Peek().Ty == TokenType.Eof ? Peek() : _stream[_index+1];
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