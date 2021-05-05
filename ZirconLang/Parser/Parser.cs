using System.Collections.Generic;
using ZirconLang.Lexer;

namespace ZirconLang.Parser
{
    public class Parser : BaseParser
    {
        public Parser(List<Token> stream) : base(stream) {}
    }
}