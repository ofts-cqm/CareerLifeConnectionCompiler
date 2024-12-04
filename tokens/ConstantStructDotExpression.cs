using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLCC.tokens
{
    public class ConstantStructDotExpression : IValueToken
    {
        public ConstantStructDotExpression(DataType type) : base(type)
        {
        }

        public override Destination GetDestination()
        {
            throw new NotImplementedException();
        }

        public override KeyValuePair<string, string> getVariabele(int position)
        {
            throw new NotImplementedException();
        }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            throw new NotImplementedException();
        }

        public override void print(string indentation)
        {
            throw new NotImplementedException();
        }
    }
}
