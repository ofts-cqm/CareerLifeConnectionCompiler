using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLCC.tokens
{
    internal class EndOfStatementToken : IToken
    {
        public bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!str.StartsWith(';')) return false;
            result = new EndOfStatementToken();
            str = str[1..];
            Tokens.fixString(ref str);
            if (add) allTokens.Add(result);
            return true;
        }

        public void print(string indentation)
        {
            Console.WriteLine(indentation + "END");
        }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
