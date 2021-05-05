using System;

namespace ZirconLang
{
    public class Runner
    {
        public static void Run(string code, SourceId sid)
        {
            var lexer = new Lexer.Lexer(code, sid);
            var tokens = lexer.Lex();
            foreach (Lexer.Token token in tokens)
            {
                Console.WriteLine($"{token.Ty}:{token.Span.S}:{token.Span.E}: `{token.Contents}`");
            }

            var operators = new Parser.ExtractOps(tokens);
            operators.Extract();
            operators.PrintEntries();
        }
    }
}