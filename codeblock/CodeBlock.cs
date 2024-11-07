using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class CodeBlock : IToken
    {
        public bool right = false;
        public List<IToken> insideTokens = new();

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            if (Content.Match("{"))
            {
                result = new CodeBlock();
                IToken? matched = Tokens.match(((CodeBlock)result).insideTokens);

                while (!(matched is CodeBlock parenthesis && parenthesis.right))
                {
                    //insideTokens.Add(matched);
                    matched = Tokens.match(((CodeBlock)result).insideTokens);
                }

                if (add) allTokens.Add(result);
                return true;
            }
            else if (Content.Match("}"))
            {
                result = new CodeBlock() { right = true };
                return true;
            }
            result = null;
            return false;
        }

        public void print(string indentation)
        {
            Console.WriteLine("Start");
            foreach (IToken token in insideTokens)
            {
                token.print(indentation + "    ");
            }
            Console.WriteLine(indentation + "End Codeblock\n");
        }

        public void writeAss(StringBuilder file, Destination destination)
        {
            foreach (IToken token in insideTokens)
            {
                token.writeAss(file, destination);
            }
        }
    }
}
