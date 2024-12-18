﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLCC.tokens
{
    internal class EndOfStatementToken : IToken
    {
        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = this;
            if(Content.Match(";"))
            {
                NewVariableToken.IsCreatingNewVar = false;
                return true;
            }
            return false;
        }

        public void print(string indentation) { }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
