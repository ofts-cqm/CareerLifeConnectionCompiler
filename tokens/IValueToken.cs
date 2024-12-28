using System.Text;

namespace CLCC.tokens
{
    public abstract class IValueToken: IExpressionToken, IAssignable
    {
        protected IValueToken(DataType type) : base(type) { }

        public virtual void DumpValue(StringBuilder file) { }

        public abstract Destination GetDestination();

        public abstract KeyValuePair<string, string> getVariabele(int position);

        public virtual void PrepareValue(StringBuilder file) { }

        public override void writeAss(StringBuilder file, Destination destination) { }
    }
}
