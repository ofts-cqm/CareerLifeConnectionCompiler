using CLCC.codeblock;
using System.Text;

namespace CLCC.tokens
{
    public class FunctionArgumentsToken : IToken
    {
        public List<IToken> insideTokens = new();
        public FunctionBlock currentContext;

        public FunctionArgumentsToken(List<IToken> insideTokens, FunctionBlock functionBlock)
        {
            this.insideTokens = insideTokens;
            this.currentContext = functionBlock;
        }

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            throw new NotImplementedException();
        }

        public void print(string indentation)
        {
            Console.WriteLine(indentation + "ParenthesisToken");
            foreach (IToken token in insideTokens)
            {
                token.print(indentation + "    ");
            }
        }

        public void writeAss(StringBuilder file, Destination destination)
        {
            for (int i = 0; i < insideTokens.Count; i++)
            {
                insideTokens[i].writeAss(file, new Destination() { Type = Destination.STACK, OffSet = currentContext.LocalCount + currentContext.SubVariableCount + 2 + i});
            }
        }
    }
}
