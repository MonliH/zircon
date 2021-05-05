using System.Collections.Generic;
using System.Linq;

namespace ZirconLang.Lexer
{
    public class Lexer : Diagnostics.Failable
    {
        private readonly string _stream;
        private int _sIdx;
        private int _eIdx;
        private readonly SourceId _sid;
        private List<Token> Tokens { get; }
        private static readonly char EOF = '\0';

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"if", TokenType.If},
            {"else", TokenType.Else},
            {"elsif", TokenType.Elsif},
            {"let", TokenType.Let},
            {"opdef", TokenType.Opdef},
        };

        public Lexer(string fileContents, SourceId sId)
        {
            _stream = fileContents;
            _eIdx = 0;
            _sIdx = 0;
            Tokens = new List<Token>();
            _sid = sId;
        }

        private char Advance()
        {
            return _stream[_eIdx++];
        }

        private char Peek()
        {
            if (IsAtEnd()) return EOF;
            return _stream[_eIdx];
        }

        private char PeekNext()
        {
            if (_eIdx + 1 >= _stream.Length) return EOF;
            return _stream[_eIdx + 1];
        }

        private bool IsAtEnd()
        {
            return _eIdx >= _stream.Length;
        }

        private Span MakeSpan()
        {
            return new Span(_sIdx, _eIdx, _sid);
        }

        private void AddToken(TokenType ty, string? str = null)
        {
            Tokens.Add(
                new Token(
                    str,
                    ty,
                    MakeSpan()
                )
            );
        }

        private bool IsLineBreak()
        {
            switch (Tokens.Count > 0 ? Tokens.Last().Ty : TokenType.Eof)
            {
                case TokenType.RBrace:
                case TokenType.Ident:
                case TokenType.Int:
                case TokenType.Float:
                case TokenType.String:
                case TokenType.RParen: return true;

                default: return false;
            }
        }

        private void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(':
                    AddToken(TokenType.LParen);
                    break;
                case ')':
                    AddToken(TokenType.RParen);
                    break;
                case '{':
                    AddToken(TokenType.LBrace);
                    break;
                case '}':
                    AddToken(TokenType.RBrace);
                    break;
                case ';':
                    AddToken(TokenType.LineBreak);
                    break;
                case '\n':
                    if (IsLineBreak()) AddToken(TokenType.LineBreak);
                    break;
                case '#':
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                    break;

                case ' ':
                case '\t':
                    break;

                case '"':
                    EatString();
                    break;

                default:
                    if (IsDigit(c))
                    {
                        EatNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        EatIdent();
                    }
                    else if (IsOperator(c))
                    {
                        EatOperator();
                    }
                    else
                    {
                        AddError(new Diagnostics.ErrorBuilder()
                            .Msg($"unexpected character `{c}`")
                            .Type(Diagnostics.ErrorType.Lexer)
                            .Span(MakeSpan()).Build());
                    }

                    break;
            }
        }

        private void EatNumber()
        {
            while (IsDigit(Peek())) Advance();

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();
                while (IsDigit(Peek())) Advance();
                AddToken(TokenType.Float, _stream.Substring(_sIdx, _eIdx - _sIdx));
                return;
            }

            AddToken(TokenType.Int, _stream.Substring(_sIdx, _eIdx - _sIdx));
        }

        private void EatIdent()
        {
            while (IsAlphaNumeric(Peek())) Advance();
            string text = _stream.Substring(_sIdx, _eIdx - _sIdx);
            if (Keywords.ContainsKey(text))
            {
                AddToken(Keywords[text]);
            }
            else
            {
                AddToken(TokenType.Ident, text);
            }
        }

        private void EatOperator()
        {
            while (IsOperator(Peek())) Advance();
            string text = _stream.Substring(_sIdx, _eIdx - _sIdx);
            AddToken(TokenType.Operator, text);
        }

        private bool IsOperator(char c)
        {
            return "!@$%^&*:~/?><.|-=+".Contains(c);
        }

        private bool IsAlpha(char c)
        {
            return (c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_');
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c) || c == '\'';
        }


        private bool IsDigit(char c)
        {
            return c is >= '0' and <= '9';
        }

        private void EatString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                char next = Advance();
                if (next == '\\')
                {
                    // Escape code
                    char code = Advance();
                    switch (code)
                    {
                        case 'n':
                        case 't':
                        case '"':
                            break;

                        default:
                            AddError(new Diagnostics.ErrorBuilder()
                                .Msg($"invalid escape sequence `\\{code}`")
                                .Type(Diagnostics.ErrorType.Lexer)
                                .Span(MakeSpan())
                                .Build());
                            break;
                    }
                }
            }

            if (IsAtEnd())
            {
                AddError(new Diagnostics.ErrorBuilder()
                    .Msg("unterminated string")
                    .Type(Diagnostics.ErrorType.Lexer)
                    .Span(MakeSpan()).Build());
                return;
            }

            // Closing "
            Advance();

            string stringContents = _stream.Substring(_sIdx + 1, _eIdx - _sIdx - 1);
            AddToken(TokenType.String, stringContents);
        }

        public List<Token> Lex()
        {
            while (!IsAtEnd())
            {
                _sIdx = _eIdx;
                ScanToken();
            }

            AddToken(TokenType.Eof);
            RaiseErrors();

            return Tokens;
        }

        public List<Token> GetTokens()
        {
            return Tokens;
        }
    }
}