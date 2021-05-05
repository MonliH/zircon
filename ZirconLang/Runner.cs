using System;
using ZirconLang.Parser;

namespace ZirconLang
{
    public class Runner
    {
        public static void Run(string code, SourceId sid)
        {
            var lexer = new Lexer.Lexer(code, sid);
            var tokens = lexer.Lex();

            var operators = new Parser.ExtractOps(tokens);
            operators.Extract();
            operators.PrintEntries();

            var parser = new Parser.Parser(operators, tokens, new Span(0, 0, sid));
            var expr = parser.ParseProgram();
            var prettyPrinter = new AstPrinter();
            Console.WriteLine(prettyPrinter.Print(expr));
        }
    }
}