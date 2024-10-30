using System.Text;

namespace CLCC.tokens
{
    public class ParenthesisToken : IToken
    {
        public List<IToken> insideTokens = new();
        public bool right = false;

        public bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            if (str.StartsWith('('))
            {
                str = str[1..];
                Tokens.fixString(ref str);
                result = new ParenthesisToken();
                IToken? matched = Tokens.match(ref str, ((ParenthesisToken)result).insideTokens);

                while (!(matched is ParenthesisToken parenthesis && parenthesis.right))
                {
                    //insideTokens.Add(matched);
                    matched = Tokens.match(ref str, ((ParenthesisToken)result).insideTokens);
                }

                if (add) allTokens.Add(result);
                return true;
            }
            else if (str.StartsWith(')'))
            {
                str = str[1..];
                Tokens.fixString(ref str);
                result = new ParenthesisToken() { right = true };
                return true;
            }
            result = null;
            return false;
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
            foreach (IToken token in insideTokens)
            {
                token.writeAss(file, destination);
            }
        }
    }
}
