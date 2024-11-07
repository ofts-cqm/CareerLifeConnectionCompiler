using System.Text;

namespace CLCC.tokens
{
    public abstract class IExpressionToken: IToken
    {
        public IExpressionToken(DataType type)
        {
            Type = type;
        }

        public DataType Type { get; set; }

        public abstract bool match(List<IToken> allTokens, out IToken? result, bool add = true);

        public abstract void print(string indentation);

        public abstract void writeAss(StringBuilder file, Destination destination);
    }
}
