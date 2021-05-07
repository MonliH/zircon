using System;
using ZirconLang.Diagnostics;
using ZirconLang.Interpreter;
using ZirconLang.Lexer;
using ZirconLang.Parser;

namespace ZirconLang
{
    public class Runner
    {
        private Env _env;

        public Runner()
        {
            GlobalVars globals = new GlobalVars();
            _env = globals.GetGlobals();
        }

        public Value Run(string code, SourceId sid, Options opts)
        {
            var lexer = new Lexer.Lexer(code, sid);
            var tokens = lexer.Lex();
            if (opts.DebugLexer)
            {
                foreach (Lexer.Token token in tokens)
                {
                    Console.WriteLine($"{token.Span.S}-{token.Span.E}: {token.Ty.Display()}: `{token.Contents}`");
                }
            }

            var operators = new Parser.ExtractOps(tokens);
            operators.Extract();
            if (opts.DebugParser)
            {
                operators.PrintEntries();
            }

            var parser = new Parser.Parser(operators, tokens, new Span(0, 0, sid));
            var expr = parser.ParseProgram();
            if (opts.DebugParser)
            {
                var prettyPrinter = new AstPrinter();
                Console.WriteLine(prettyPrinter.Print(expr));
            }

            return InterpreterVisitor.Interpret(_env, expr);
        }
    }
}