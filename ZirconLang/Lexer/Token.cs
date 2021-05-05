using System;
using ZirconLang.Diagnostics;

namespace ZirconLang.Lexer
{
    public enum TokenType
    {
        // Symbols
        LParen, RParen, LBrace, RBrace,
        
        // Keywords
        Let, If, Elsif, Else, Opdef,
        
        // Literals
        String, Ident, Int, Float, Operator,
        
        LineBreak, Eof
    }

    public static class TokenTypeExtension
    {
        public static string Display(this TokenType ty)
        {
            switch (ty)
            {
                case TokenType.LParen: return "token `(`";
                case TokenType.RParen: return "token `)`";
                case TokenType.LBrace: return "token `{`";
                case TokenType.RBrace: return "token `}`";
                
                case TokenType.Let: return "keyword `let`";
                case TokenType.If: return "keyword `if`";
                case TokenType.Elsif: return "keyword `elsif`";
                case TokenType.Else: return "keyword `else`";
                case TokenType.Opdef: return "keyword `opdef`";
                
                case TokenType.String: return "string";
                case TokenType.Ident: return "identifier";
                case TokenType.Int: return "integer";
                case TokenType.Float: return "float";
                case TokenType.Operator: return "operator";
                
                case TokenType.LineBreak: return "line break";
                case TokenType.Eof: return "end of file";
                
                default: throw new InvalidOperationException("Seriously broken");
            }
        }
    }

    public record Token(string? Contents, TokenType Ty, Span Span);
}