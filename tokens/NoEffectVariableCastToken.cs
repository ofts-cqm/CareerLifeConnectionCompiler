using System.Text;

namespace CLCC.tokens
{
    public class NoEffectVariableCastToken : IValueToken
    {
        public IValueToken baseToken;

        public NoEffectVariableCastToken(DataType type, IValueToken value) : base(type)
        {
            baseToken = value;
        }

        public NoEffectVariableCastToken() : base(DataType.NULL) { }

        public override Destination GetDestination() => baseToken.GetDestination();

        public override KeyValuePair<string, string> getVariabele(int position) => baseToken.getVariabele(position);

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            Content.Push();
            if (!Content.Match("("))
            {
                Content.Pop();
                return false;
            }
            if (!DataType.TryParseDataType(out DataType castedType))
            {
                Content.Pop();
                return false;
            }

            IToken matched = Tokens.match(allTokens, false);
            if (matched is IValueToken value)
            {
                result = new NoEffectVariableCastToken(castedType, value);
                if (add) allTokens.Add(result);
                Content.Ignore();
                return true;
            }
            else if(matched is IExpressionToken expression)
            {
                result = matched;
                expression.Type = castedType;
                if (add) allTokens.Add(result);
                Content.Ignore();
                return true;
            }

            Content.Pop() ;
            return false;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}NoEffectCast to {Type.name} Token");
            baseToken.print(indentation + "    ");
        }
    }
}
