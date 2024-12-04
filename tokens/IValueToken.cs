using System.Text;

namespace CLCC.tokens
{
    public interface IValueToken: IToken
    {
        public abstract KeyValuePair<string, string> getVariabele(int position);

        public abstract Destination GetDestination();
    }
}
