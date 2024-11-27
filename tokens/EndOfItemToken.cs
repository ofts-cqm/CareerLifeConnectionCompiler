using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLCC.tokens
{
    public class EndOfItemToken : IToken
    {
        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = this;
            if (Content.Match(","))
            {
                if (add) allTokens.Add(this);
                return true;
            }
            return false;
        }

        public void print(string indentation) { }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
