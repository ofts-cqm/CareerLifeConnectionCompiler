using System.Text;

namespace CLCC.tokens
{
    public abstract class IValueToken: IExpressionToken
    {
        protected IValueToken(DataType type) : base(type) { }

        public abstract KeyValuePair<string, string> getVariabele(int position);

        public abstract Destination GetDestination();

        public override void writeAss(StringBuilder file, Destination destination) { }
    }
}
