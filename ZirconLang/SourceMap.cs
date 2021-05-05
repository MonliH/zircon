using System.Collections.Generic;
using System.Linq;

namespace ZirconLang
{
    public class SourceMap
    {
        private readonly Dictionary<SourceId, string> _sources;
        private readonly Dictionary<SourceId, string> _filenames;
        private int _currentIdx;

        public SourceMap()
        {
            _sources = new Dictionary<SourceId, string>();
            _filenames = new Dictionary<SourceId, string>();
        }

        public string LookupSource(SourceId sid)
        {
            return _sources[sid];
        }
        
        public string LookupFilename(SourceId sid)
        {
            return _filenames[sid];
        }

        public SourceId AddSource(string contents, string filename)
        {
            SourceId source = new SourceId(_currentIdx++);
            _sources[source] = contents;
            _filenames[source] = filename;
            return source;
        }
    }
}