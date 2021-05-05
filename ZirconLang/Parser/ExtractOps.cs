using System;
using System.Collections.Generic;
using ZirconLang.Lexer;

namespace ZirconLang.Parser
{
    public class ExtractOps : BaseParser
    {
        public Dictionary<string, int> Prefix;
        public Dictionary<string, int> Postfix;
        public Dictionary<string, int> Infix;

        public ExtractOps(List<Token> stream) : base(stream)
        {
            Prefix = new Dictionary<string, int>();
            Postfix = new Dictionary<string, int>();
            Infix = new Dictionary<string, int>();
        }

        public void Extract()
        {
            while (!IsAtEnd())
            {
                if (Match(TokenType.Opdef))
                {
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

                    Token num = Consume(TokenType.Int);
                    Consume(TokenType.LineBreak);

                    string contents = op.Contents!;
                    var parsed = int.Parse(num.Contents!);
                    if (prefix == postfix) Infix.Add(contents, parsed);
                    else if (prefix) Prefix.Add(contents, parsed);
                    else Postfix.Add(contents, parsed);
                }
                else Advance();
            }
        }

        public void PrintEntries()
        {
            Console.WriteLine("Infix Operators:");
            foreach (var (op, prec) in Infix)
            {
                Console.WriteLine($"  {op}: prec {prec}");
            }
            
            Console.WriteLine("Prefix Operators:");
            foreach (var (op, prec) in Prefix)
            {
                Console.WriteLine($"  {op}: prec {prec}");
            }
            
            Console.WriteLine("Postfix Operators:");
            foreach (var (op, prec) in Postfix)
            {
                Console.WriteLine($"  {op}: prec {prec}");
            }
        }
    }
}