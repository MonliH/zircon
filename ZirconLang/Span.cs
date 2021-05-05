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
    };
}