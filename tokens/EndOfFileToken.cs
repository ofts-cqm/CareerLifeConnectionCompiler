using System.Text;

namespace CLCC.tokens
{
    public class EndOfFileToken : IToken
    {
        public bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = this;
            if (str == null || str.Length == 0)
            {
                str = "";
                return true;
            }
            return false;
        }

        public void print(string indentation)
        {
            Console.WriteLine(indentation + "End of File");
        }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
