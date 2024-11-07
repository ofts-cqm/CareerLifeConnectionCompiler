using System.Text;

namespace CLCC.tokens
{
    public class EndOfFileToken : IToken
    {
        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = this;
            return Content.IsEnd();
        }

        public void print(string indentation)
        {
            Console.WriteLine(indentation + "End of File");
        }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
