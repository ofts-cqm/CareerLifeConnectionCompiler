using System.Text;

namespace CLCC.tokens
{
    public abstract class IValueToken: IExpressionToken
    {
        public IValueToken(DataType type) : base(type) { }

        public abstract KeyValuePair<string, string> getVariabele(int position);

        public override void writeAss(StringBuilder file, Destination destination) { }

        public abstract Destination GetDestination();
    }
}
