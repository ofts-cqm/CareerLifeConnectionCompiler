using clcc;
using System.Text;

namespace CLCC.tokens
{
    public class GlobalVariableToken : IExpressionToken, IValueToken
    {
        public int Offset { get; set; }
        public string Name { get; set; }
        public bool Initialized { get; set; }

        public GlobalVariableToken(DataType type, string name, int offset) : base(type)
        {
            Name = name;
            Offset = offset;
            Initialized = false;
        }

        public Destination GetDestination()
        {
            return new Destination() { Type = Destination.HEAP, OffSet = Offset };
        }

        public KeyValuePair<string, string> getVariabele(int position)
        {
            return new KeyValuePair<string, string>($"|imm{position}|mem{position}", Offset + "");
        }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            throw new NotImplementedException();
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}Global {Type} {Name} (offset: {Offset.ToString("X")})");
        }

        public override void writeAss(StringBuilder file, Destination destination) { }
    }
}
