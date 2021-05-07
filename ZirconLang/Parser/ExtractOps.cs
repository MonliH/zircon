using System;
using System.Collections.Generic;
using ZirconLang.Diagnostics;
using ZirconLang.Lexer;

namespace ZirconLang.Parser
{
    public enum Assoc
    {
        Left,
        Right,
    }

    public class ExtractOps : BaseParser
    {
        public Dictionary<string, int> Prefix;
        public Dictionary<string, int> Postfix;
        public Dictionary<string, (int, Assoc)> Binary;

        public (int, int) BinaryBp(string op, Span span)
        {
            if (Binary.ContainsKey(op))
            {
                var (prec, assoc) = Binary[op];
                if (assoc == Assoc.Left) return (prec - 1, prec);
                if (assoc == Assoc.Right) return (prec, prec - 1);
            }

            if (Prefix.ContainsKey(op) || Postfix.ContainsKey(op))
            {
                throw new ErrorBuilder().Msg($"`{op}` is not a binary operator").Span(span).Type(ErrorType.Syntax)
                    .Build();
            }

            InvalidOp(op, span);
            throw new InvalidOperationException("Should not be here");
        }

        public int PrefixBp(string op, Span span)
        {
            if (Prefix.ContainsKey(op))
            {
                return Prefix[op];
            }

            if (Binary.ContainsKey(op) || Postfix.ContainsKey(op))
            {
                throw new ErrorBuilder().Msg($"`{op}` is not a prefix operator").Span(span).Type(ErrorType.Syntax)
                    .Build();
            }

            InvalidOp(op, span);
            throw new InvalidOperationException("Should not be here");
        }

        public int PostfixBp(string op, Span span)
        {
            if (Postfix.ContainsKey(op))
            {
                return Postfix[op];
            }

            if (Binary.ContainsKey(op) || Prefix.ContainsKey(op))
            {
                throw new ErrorBuilder().Msg($"`{op}` is not a postfix operator").Span(span).Type(ErrorType.Syntax)
                    .Build();
            }

            InvalidOp(op, span);
            throw new InvalidOperationException("Should not be here");
        }

        private void InvalidOp(string op, Span span)
        {
            throw new ErrorBuilder().Msg($"`{op}` is not a valid operator").Span(span).Type(ErrorType.Syntax).Build();
        }

        public ExtractOps(List<Token> stream) : base(stream)
        {
            Prefix = new Dictionary<string, int>();
            Postfix = new Dictionary<string, int>();
            Binary = new Dictionary<string, (int, Assoc)>();
        }

        private Token ExtractOp()
        {
            Consume(TokenType.LParen);
            Token op = Consume(TokenType.Operator);
            Consume(TokenType.RParen);
            return op;
        }

        public void Extract()
        {
            while (!IsAtEnd())
            {
                if (Check(TokenType.Binary) || Check(TokenType.Prefix) || Check(TokenType.Postfix))
                {
                    Token next = Advance();
                    var (contents, _, sp) = ExtractOp();
                    Token num = Consume(TokenType.Int, $"expected precedence declaration for operator {contents!}");
                    var parsed = int.Parse(num.Contents!) * 10;
                    try
                    {
                        if (next.Ty == TokenType.Binary)
                        {
                            var assoc = Assoc.Left;
                            if (Check(TokenType.Ident, "left")) assoc = Assoc.Left;
                            else if (Check(TokenType.Ident, "right")) assoc = Assoc.Right;
                            else
                                throw new ErrorBuilder().Msg("expected an associativity of `left` or `right`")
                                    .Span(CurrentSpan()).Type(ErrorType.Syntax).Build();
                            Binary[contents!] = (parsed, assoc);
                        }
                        else if (next.Ty == TokenType.Prefix) Prefix[contents!] = parsed;
                        else if (next.Ty == TokenType.Postfix) Postfix[contents!] = parsed;
                    }
                    catch (ArgumentException)
                    {
                        throw new ErrorBuilder().Msg($"cannot redefine the operator {contents!}").Type(ErrorType.Scope)
                            .Span(sp).Build();
                    }
                }
                else Advance();
            }
        }

        public void PrintEntries()
        {
            Console.WriteLine("Infix Operators:");
            foreach (var (op, prec) in Binary)
            {
                Console.WriteLine($"  `{op}`: prec {prec}");
            }

            Console.WriteLine("---");

            Console.WriteLine("Prefix Operators:");
            foreach (var (op, prec) in Prefix)
            {
                Console.WriteLine($"  `{op}`: prec {prec}");
            }

            Console.WriteLine("---");

            Console.WriteLine("Postfix Operators:");
            foreach (var (op, prec) in Postfix)
            {
                Console.WriteLine($"  `{op}`: prec {prec}");
            }

            Console.WriteLine("---");
        }
    }
}