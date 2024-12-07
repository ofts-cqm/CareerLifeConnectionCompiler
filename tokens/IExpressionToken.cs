using System.Text;

namespace CLCC.tokens
{
    public abstract class IExpressionToken: IExpressionBaseToken
    {
        public IExpressionToken(DataType type)
        {
            Type = type;
        }

        public DataType Type { get; set; }

        public abstract bool match(List<IToken> allTokens, out IToken? result, bool add = true);

        public abstract void print(string indentation);

        public abstract void writeAss(StringBuilder file, Destination destination);

        public static void decodeDestination(Destination destination, StringBuilder code, out string value, int pos = 3)
        {
            switch (destination.Type)
            {
                case Destination.REGISTER:
                    {
                        value = Destination.RegisterName[destination.OffSet];
                        break;
                    }
                case Destination.STACK:
                    {
                        code.Append($"|imm{pos}|mem{pos}|sta{pos}");
                        value = destination.OffSet.ToString();
                        break;
                    }
                case Destination.HEAP:
                    {
                        code.Append($"|imm{pos}|mem{pos}");
                        value = destination.OffSet.ToString();
                        break;
                    }
                case Destination.DEREFERENCE:
                    {
                        code.Append($"|mem{pos}");
                        value = Destination.RegisterName[destination.OffSet];
                        break;
                    }
                default:
                    {
                        value = "";
                        break;
                    }
            }
        }
    }
}
