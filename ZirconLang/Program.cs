using System;
using CommandLine;
using ZirconLang.Builtins;
using ZirconLang.Interpreter;

namespace ZirconLang
{
    public class Options
    {
        [Value(0, MetaName = "<FILE>", Required = false,
            HelpText = "Filename to execute. If not passed, boot up the REPL.")]
        public string? Filename { get; set; }

        [Option("debug-parser", Required = false, HelpText = "Print AST and operator information")]
        public bool DebugParser { get; set; }

        [Option("debug-lexer", Required = false, HelpText = "Print tokens emitted from the lexer")]
        public bool DebugLexer { get; set; }
    }

    class Program
    {
        static void RunOptions(Options opts)
        {
            Runner runner = new();
            SourceMap smap = new();

            try
            {
                LoadBuiltins.Load(smap, runner, new Options());
            }
            catch (Diagnostics.ErrorDisplay e)
            {
                e.DisplayError(smap);
                Environment.Exit(1);
            }

            if (opts.Filename != null)
            {
                string filename = opts.Filename;
                string fileContents = System.IO.File.ReadAllText(filename);
                SourceId sid = smap.AddSource(fileContents, filename);

                try
                {
                    runner.Run(smap.LookupSource(sid), sid, opts);
                }
                catch (Diagnostics.ErrorDisplay e)
                {
                    e.DisplayError(smap);
                    Environment.Exit(1);
                }
            }
            else
            {
                // Boot into REPL
                while (true)
                {
                    Console.Write("zλ> ");
                    string input = Console.ReadLine() ?? ":q";
                    if (input == ":q")
                    {
                        Console.WriteLine("\nbye.");
                        break;
                    }

                    SourceId sid = smap.AddSource(input, "<repl_input>");
                    try
                    {
                        Value val = runner.Run(smap.LookupSource(sid), sid, opts);
                        if (val.Type() != TypeName.Unit)
                            Console.WriteLine(ValuePrinter.Print(val));
                    }
                    catch (Diagnostics.ErrorDisplay e)
                    {
                        e.DisplayError(smap);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);
        }
    }
}