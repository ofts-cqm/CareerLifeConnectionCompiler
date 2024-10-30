using System.Text;

namespace CLCC.tokens
{
    public abstract class IValueToken: IToken
    {
        public abstract KeyValuePair<string, string> getVariabele(int position);

        public abstract bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true);

        public abstract void print(string indentation);

        public void writeAss(StringBuilder file, Destination destination) { }

        public abstract Destination GetDestination();
    }
}
