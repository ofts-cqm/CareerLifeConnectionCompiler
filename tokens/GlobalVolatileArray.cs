using clcc;
using System.Text;

namespace CLCC.tokens
{
    internal class GlobalVolatileArray : IToken
    {
        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            if (Content.Match("various int"))
            {
                string name = Tokens.matchName();
                Content.Get(1);
                IToken token = Tokens.match(new(), false);
                int length;
                if (token is not NumberToken num)
                {
                    Content.LogError("array length need to be constant");
                    length = 1;
                }
                else
                {
                    length = num.number;
                }
                Content.Get(1);
                result = new GlobalVariableToken(new(DataType.INT) { isArray = true, length = length}, name, Lexer.CurrentOffset);
                Lexer.CurrentOffset -= length;
                if (add)
                {
                    allTokens.Add(result);
                }
                return true;
            }
            result = null;
            return false;
        }

        public void print(string indentation)
        {
            throw new NotImplementedException();
        }

        public void writeAss(StringBuilder file, Destination destination)
        {
            throw new NotImplementedException();
        }
    }
}
