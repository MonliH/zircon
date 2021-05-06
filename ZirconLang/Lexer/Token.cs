using System;
using ZirconLang.Diagnostics;

namespace ZirconLang.Lexer
{
    public enum TokenType
    {
        // Symbols
        LParen, RParen, LBrace, RBrace, Backslash, Arrow,
        
        // Keywords
        Let, Prefix, Postfix, Binary,
        
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
                case TokenType.Arrow: return "token `->`";
                case TokenType.Backslash: return "token `\\`";
                
                case TokenType.Let: return "keyword `let`";
                case TokenType.Prefix: return "keyword `preop`";
                case TokenType.Postfix: return "keyword `postop`";
                case TokenType.Binary: return "keyword `binop`";
                
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