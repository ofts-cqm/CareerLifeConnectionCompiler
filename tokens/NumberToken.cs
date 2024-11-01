namespace CLCC.tokens
{
    internal class NumberToken : IValueToken
    {
        public int number { get; set; }
        public NumberToken(int number, DataType type):base(type) { this.number = number; }
        public NumberToken(): base(new()) { }

        public override bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;

            if (str.StartsWith("true"))
            {
                result = createNumberToken(1, ref str, allTokens, add, new("bool"));
                return true;
            }
            else if (str.StartsWith("false"))
            {
                result = createNumberToken(0, ref str, allTokens, add, new("bool"));
                return true;
            }

            if (str[0] < '0' || str[0] > '9') return false;

            int num = 0;
            while (str.Length > 0 && str[0] >= '0' && str[0] <= '9')
            {
                num *= 10;
                num += str[0] - '0';
                str = str[1..];
            }

            if (str.StartsWith('.'))
            {
                str = str[1..];
                return parseFloat(num, ref str, allTokens, add, out result);
            }

            result = createNumberToken(num, ref str, allTokens, add, new("int"));
            return true;
        }

        public bool parseFloat(int num, ref string str, List<IToken> allTokens, bool add, out IToken token)
        {
            float result = num;
            for (int i = -1; str.Length > 0 && str[0] >= '0' && str[0] <= '9'; i--)
            {
                result += (float)((str[0] - '0') * Math.Pow(10, i));
                str = str[1..];
            }
            token = createNumberToken(BitConverter.SingleToInt32Bits(result), ref str, allTokens, add, new("float"));
            return true;
        }

        public IToken createNumberToken(int num, ref string str, List<IToken> allTokens, bool add, DataType type)
        {
            IToken token = new NumberToken(num, type);
            if (add) allTokens.Add(token);
            Tokens.fixString(ref str);
            return token;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}Number Token {number} Type {Type}");
        }

        public override KeyValuePair<string, string> getVariabele(int position)
        {
            return new KeyValuePair<string, string>("|imm" + position, number + "");
        }

        public override Destination GetDestination()
        {
            throw new AccessViolationException("Cannot Set Constant");
        }
    }
}
