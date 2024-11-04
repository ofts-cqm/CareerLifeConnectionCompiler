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

        public override bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!str.StartsWith('(')) return false;
            str = str[1..];
            Tokens.fixString(ref str);
            if (!DataType.TryParseDataType(ref str, out DataType castedType))
            {
                str = '(' + str;
                return false;
            }

            IToken matched = Tokens.match(ref str, allTokens, false);
            if (matched is IValueToken value)
            {
                result = new NoEffectVariableCastToken(castedType, value);
                if (add) allTokens.Add(result);
                return true;
            }
            else if(matched is IExpressionToken expression)
            {
                result = matched;
                expression.Type = castedType;
                if (add) allTokens.Add(result);
                return true;
            }
            return false;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}NoEffectCast to {Type.name} Token");
            baseToken.print(indentation + "    ");
        }
    }
}
