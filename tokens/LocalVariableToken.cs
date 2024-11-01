using clcc;

namespace CLCC.tokens
{
    public class LocalVariableToken : IValueToken
    {
        public int Offset { get; set; }
        public string Name { get; set; }

        public LocalVariableToken(int offset, string name, DataType type) : base(type)
        {
            Offset = offset;
            Name = name;
        }

        public LocalVariableToken(): base(DataType.NULL) { }

        public override bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (str[0] < 'a' || str[0] > 'z') return false;
            string name = Tokens.matchName(ref str);
            Tokens.fixString(ref str);

            if (Lexer.Current?.tryGetLocalVariable(name, out LocalVariableToken token) ?? false)//if (Lexer.LocalVariables.TryGetValue(name, out LocalVariableToken? token))
            {
                if (add) allTokens.Add(token);
                result = token;
                return true;
            }

            str = name + " " + str;
            return false;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}Local {Type} {Name} (offset: {Offset})");
        }

        public override KeyValuePair<string, string> getVariabele(int position)
        {
            return new KeyValuePair<string, string>($"|imm{position}|mem{position}|sta{position}", Offset + "");
        }

        public override Destination GetDestination()
        {
            return new Destination() {Type = Destination.STACK, OffSet = Offset };
        }
    }
}
