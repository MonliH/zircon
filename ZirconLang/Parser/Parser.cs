using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZirconLang.Diagnostics;
using ZirconLang.Lexer;

namespace ZirconLang.Parser
{
    public class Parser : BaseParser
    {
        private ExtractOps _ops;
        private readonly Span _def;
        private static string _fnApp = "<fn app>";

        public Parser(ExtractOps ops, List<Token> stream, Span def) : base(stream)
        {
            _ops = ops;
            _def = def;
            _ops.Binary.Add(_fnApp, (10, Assoc.Left));
        }

        public Expr ParseProgram()
        {
            var exprs = new List<Expr>();
            while (!IsAtEnd() && !Check(TokenType.RBrace))
            {
                if (Check(TokenType.Prefix) || Check(TokenType.Postfix) || Check(TokenType.Binary))
                {
                    Token keyword = Advance();
                    Consume(TokenType.LParen);
                    Consume(TokenType.Operator);
                    Consume(TokenType.RParen);
                    Consume(TokenType.Int);
                    if (keyword.Ty == TokenType.Binary) Consume(TokenType.Ident);
                }
                else exprs.Add(ParseExpr());

                while (Check(TokenType.LineBreak)) Consume(TokenType.LineBreak);
            }

            if (exprs.Any()) return new Expr.Sequence(exprs, exprs.First().Span.Combine(exprs.Last().Span));
            return new Expr.Sequence(new List<Expr>(), _def);
        }

        private Expr ParseBlock()
        {
            Consume(TokenType.LBrace);
            Expr program = ParseProgram();
            Consume(TokenType.RBrace);
            return program;
        }

        private Expr.Variable ParseVar()
        {
            if (Check(TokenType.Ident))
            {
                Token var = Consume(TokenType.Ident);
                return new Expr.Variable(var.Contents!, false, var.Span);
            }

            if (!Check(TokenType.LParen) && !Check(TokenType.Operator, "@"))
                throw new ErrorBuilder()
                    .Msg($"Expected identifier or operator, found {Peek().Ty.Display()}")
                    .Span(Peek().Span)
                    .Type(ErrorType.Syntax)
                    .Build();

            var prefix = false;
            var postfix = false;
            if (Match((TokenType.Operator, "@")))
            {
                prefix = true;
            }

            Consume(TokenType.LParen);
            Token op = Consume(TokenType.Operator);
            Consume(TokenType.RParen);

            if (Match((TokenType.Operator, "@")))
            {
                postfix = true;
            }

            var binary = prefix == postfix;
            var prefixStr = prefix ? "@(" : "(";
            var postfixStr = postfix ? ")@" : ")";
            string contents = binary ? op.Contents! : $"{prefixStr}{op.Contents}{postfixStr}";

            return new Expr.Variable(contents, true, op.Span);
        }

        private Expr ParseLet()
        {
            Span prev = CurrentSpan();
            Consume(TokenType.Let);
            Expr.Variable variable = ParseVar();
            Consume("=", TokenType.Operator);
            Expr expr = ParseExpr();
            return new Expr.Assignment(variable, expr, prev.Combine(expr.Span));
        }

        private Expr ParseLambda()
        {
            Span prev = CurrentSpan();
            Consume(TokenType.Backslash);
            List<Expr.Variable> args = new List<Expr.Variable>();
            args.Add(ParseVar());
            while (!IsAtEnd() && Check(TokenType.Ident))
            {
                args.Add(ParseVar());
            }

            Consume("->", TokenType.Operator);

            Expr e = ParseExpr();
            Span lambdaSpan = prev.Combine(e.Span);
            Expr final = new Expr.Lambda(args.Last(), e, lambdaSpan);
            args.RemoveAt(args.Count - 1);
            for (int i = args.Count - 1; i >= 0; i--)
            {
                final = new Expr.Lambda(args[i], final, lambdaSpan);
            }

            return final;
        }

        private Expr ParseExpr(int minBp = 0)
        {
            Expr item;
            if (Check(TokenType.Operator))
            {
                // Prefix operator
                Token op = Consume(TokenType.Operator);
                int rBp = _ops.PrefixBp(op.Contents!, op.Span);
                Expr opE = new Expr.Variable($"@({op.Contents})", true, op.Span);
                Expr rhs = ParseExpr(rBp);
                item = new Expr.Application(opE, rhs, op.Span.Combine(rhs.Span));
            }
            else
            {
                item = ParseExprItem();
            }

            while (true)
            {
                if (IsAtEnd() || Check(TokenType.LineBreak) || Check(TokenType.RParen)) break;
                Span span = CurrentSpan();
                Token op = Check(TokenType.Operator)
                    ? Peek()
                    : new Token(_fnApp, TokenType.Operator, item.Span.Between(span));

                if (op.Contents != _fnApp)
                {
                    // Possible postfix operator
                    if (_ops.Postfix.ContainsKey(op.Contents!) && // If it's contained as a postfix op...
                        // ... and it isn't a binary operator, or it's at the end of a line/eof/paren
                        (!_ops.Binary.ContainsKey(op.Contents!) || IsAtEnd() || Check(TokenType.LineBreak) || Check(TokenType.RParen)))
                    {
                        // then it's a postfix operator!
                        var lBpPost = _ops.PostfixBp(op.Contents!, op.Span);
                        if (lBpPost < minBp)
                        {
                            break;
                        }

                        Advance();
                        Expr opE = new Expr.Variable($"({op.Contents!})@", true, op.Span);
                        item = new Expr.Application(opE, item, item.Span.Combine(opE.Span));
                        continue;
                    }
                }

                var (lBp, rBp) = _ops.BinaryBp(op.Contents!, op.Span);
                if (lBp < minBp)
                {
                    break;
                }

                if (op.Contents != _fnApp)
                {
                    Advance();
                }

                Expr rhs = ParseExpr(rBp);
                Span sp = item.Span.Combine(rhs.Span);
                if (op.Contents == _fnApp)
                {
                    item = new Expr.Application(item, rhs, sp);
                }
                else
                {
                    Expr opE = new Expr.Variable(op.Contents!, true, op.Span);
                    item = new Expr.Application(new Expr.Application(opE, item, sp), rhs, sp);
                }
            }

            return item;
        }

        private Expr ParseExprItem()
        {
            if (Check(TokenType.LBrace))
            {
                return ParseBlock();
            }

            if (Check(TokenType.Let))
            {
                return ParseLet();
            }

            if (Check(TokenType.Int))
            {
                Token integer = Consume(TokenType.Int);
                return new Expr.Integer(long.Parse(integer.Contents!), integer.Span);
            }

            if (Check(TokenType.Float))
            {
                Token integer = Consume(TokenType.Float);
                return new Expr.Float(double.Parse(integer.Contents!), integer.Span);
            }

            if (Check(TokenType.String))
            {
                Token str = Consume(TokenType.String);
                return new Expr.String(str.Contents!, str.Span);
            }

            if (Check(TokenType.Ident) || (Check(TokenType.LParen) && CheckNext(TokenType.Operator)))
            {
                return ParseVar();
            }

            if (Check(TokenType.LParen))
            {
                var s = CurrentSpan();
                Advance();
                Expr e = ParseExpr();
                var paren = Consume(TokenType.RParen);
                e.Span = s.Combine(paren.Span);
                return e;
            }

            if (Check(TokenType.Backslash))
            {
                return ParseLambda();
            }

            throw new ErrorBuilder().Msg($"Expected an expression item, found {Peek().Ty.Display()}")
                .Span(CurrentSpan()).Type(ErrorType.Syntax).Build();
        }
    }
}