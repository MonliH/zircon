using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ZirconLang.Builtins
{
    public static class LoadBuiltins
    {
        public static void Load(SourceMap sm, Runner runner, Options options)
        {
            string currPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Builtins");
            List<string> files = Directory.EnumerateFiles(currPath, "*.zn").ToList();
            files.Sort();
            foreach (string file in files)
            {
                string contents = File.ReadAllText(file);
                SourceId sid = sm.AddSource(contents, file);
                runner.Run(contents, sid, options);
            }
        }
    }
}