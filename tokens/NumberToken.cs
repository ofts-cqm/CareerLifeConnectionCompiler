namespace CLCC.tokens
{
    internal class NumberToken : IValueToken
    {
        public int number { get; set; }
        public NumberToken(int number, DataType type):base(type) { this.number = number; }
        public NumberToken(): base(DataType.NULL) { }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;

            if (Content.Match("true"))
            {
                result = createNumberToken(1, allTokens, add, DataType.BOOL);
                return true;
            }
            else if (Content.Match("false"))
            {
                result = createNumberToken(0, allTokens, add, DataType.BOOL);
                return true;
            }

            if (Content.CurrentChar < '0' || Content.CurrentChar > '9') return false;

            int num = 0;
            while (!Content.IsEnd() && Content.CurrentChar >= '0' && Content.CurrentChar <= '9')
            {
                num *= 10;
                num += Content.CurrentChar - '0';
                Content.Advance();
            }

            if (Content.Match("."))
            {
                Content.Advance();
                return parseFloat(num, allTokens, add, out result);
            }

            Content.Fix();
            result = createNumberToken(num, allTokens, add, DataType.INT);
            return true;
        }

        public bool parseFloat(int num, List<IToken> allTokens, bool add, out IToken token)
        {
            float result = num;
            for (int i = -1; !Content.IsEnd() && Content.CurrentChar >= '0' && Content.CurrentChar <= '9'; i--)
            {
                result += (float)((Content.CurrentChar - '0') * Math.Pow(10, i));
                Content.Advance();
            }
            Content.Fix();
            token = createNumberToken(BitConverter.SingleToInt32Bits(result), allTokens, add, DataType.FLOAT);
            return true;
        }

        public IToken createNumberToken(int num, List<IToken> allTokens, bool add, DataType type)
        {
            IToken token = new NumberToken(num, type);
            if (add) allTokens.Add(token);
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
