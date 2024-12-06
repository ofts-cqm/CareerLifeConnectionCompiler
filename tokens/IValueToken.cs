using System.Text;

namespace CLCC.tokens
{
    public abstract class IValueToken: IExpressionToken, IAssignable
    {
        protected IValueToken(DataType type) : base(type) { }

        public void DumpValue() { }

        public abstract Destination GetDestination();

        public abstract KeyValuePair<string, string> getVariabele(int position);

        public void PrepareValue(StringBuilder file) { }

        public bool ProxyDecodeDestination(Destination destination, StringBuilder file, out string value, int pos = 3)
        {
            value = "";
            return false;
        }

        public override void writeAss(StringBuilder file, Destination destination) { }
    }
}
