using CommandLine;

namespace ZirconLang
{
    class Options
    {
        [Value(0, MetaName = "<FILE>", Required = false,
            HelpText = "Filename to execute. If not passed, boot up the REPL.")]
        public string? Filename { get; set; }
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
                    Runner.Run(smap.LookupSource(sid), sid);
                }
            }
            catch (Diagnostics.ErrorDisplay e)
            {
                e.DisplayError(smap);
            }
        }

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(RunOptions);
        }
    }
}