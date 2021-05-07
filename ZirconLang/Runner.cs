using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using ZirconLang.Diagnostics;
using ZirconLang.Interpreter;
using ZirconLang.Lexer;
using ZirconLang.Parser;

namespace ZirconLang
{
    public class Runner
    {
        private Env _env;
        private ExtractOps _ops;

        public Runner()
        {
            GlobalVars globals = new GlobalVars();
            _env = globals.GetGlobals();
            _ops = new ExtractOps(new List<Token>());
        }

        public Value Run(string code, SourceId sid, Options opts)
        {
            var lexer = new Lexer.Lexer(code, sid);
            var tokens = lexer.Lex();
            if (opts.DebugLexer)
            {
                foreach (Lexer.Token token in tokens)
                {
                    Console.WriteLine(
                        $"{token.Span.S.ToString()}-{token.Span.E.ToString()}: {token.Ty.Display()}: `{token.Contents}`");
                }
            }

            _ops.SetStream(tokens);
            _ops.Extract();
            if (opts.DebugParser)
            {
                _ops.PrintEntries();
            }

            var parser = new Parser.Parser(_ops, tokens, new Span(0, 0, sid));
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