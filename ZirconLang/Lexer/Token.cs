namespace ZirconLang.Lexer
{
    public enum TokenType
    {
        // Symbols
        LParen, RParen, LBrace, RBrace, Arrow, Equals,
        
        // Keywords
        Let, If, Elsif, Else, Infix, Prefix,
        
        // Literals
        String, Ident, Number, Operator,
        
        LineBreak, Eof
    }

    public record Token(string? Contents, TokenType Ty, Span Span);
}