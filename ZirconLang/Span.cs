namespace ZirconLang
{
    public record SourceId(int Sid);
    public record Span(int S, int E, SourceId Sid);
}