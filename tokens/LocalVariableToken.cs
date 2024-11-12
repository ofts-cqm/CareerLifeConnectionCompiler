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

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (Content.CurrentChar < 'a' || Content.CurrentChar > 'z') return false;
            Content.Push();
            string name = Tokens.matchName();

            if (Lexer.Current?.tryGetLocalVariable(name, out LocalVariableToken token) ?? false)//if (Lexer.LocalVariables.TryGetValue(name, out LocalVariableToken? token))
            {
                if (add) allTokens.Add(token);
                result = token;
                Content.Ignore();
                return true;
            }
            else if (Lexer.GlobalVariables.TryGetValue(name, out GlobalVariableToken token2))//if (Lexer.LocalVariables.TryGetValue(name, out LocalVariableToken? token))
            {
                if (add) allTokens.Add(token2);
                result = token2;
                Content.Ignore();
                return true;
            }

            Content.Pop();
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
