namespace ZirconLang
{
    public record SourceId(int Sid);

    public struct Span
    {
        public int S;
        public int E;
        public SourceId Sid;

        public Span(int s, int e, SourceId sid)
        {
            S = s;
            E = e;
            Sid = sid;
        }

        public Span Combine(Span other)
        {
            return new (S, other.E, Sid);
        }
        
        public Span Between(Span other)
        {
            return new (E, other.S, Sid);
        }

        public static (int, int) LineCol(int pos, string source)
        {
            var lineno = 1;
            var colno = 1;
            foreach (var c in source[..pos])
            {
                colno += 1;
                if (c == '\n') {
                    lineno += 1;
                    colno = 1;
                }
            }

            return (lineno, colno);
        }
    };
}