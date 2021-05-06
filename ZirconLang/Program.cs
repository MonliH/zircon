using System;
using CommandLine;

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
            Runner runner = new Runner();
            SourceMap smap = new SourceMap();
            try
            {
                if (opts.Filename != null)
                {
                    string filename = opts.Filename;
                    string fileContents = System.IO.File.ReadAllText(filename);
                    SourceId sid = smap.AddSource(fileContents, filename);
                    Runner.Run(smap.LookupSource(sid), sid, opts);
                }
            }
            catch (Diagnostics.ErrorDisplay e)
            {
                e.DisplayError(smap);
                Environment.Exit(1);
            }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);
        }
    }
}