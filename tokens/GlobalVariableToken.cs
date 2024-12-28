using clcc;
using System.Text;

namespace CLCC.tokens
{
    public class GlobalVariableToken : IValueToken
    {
        public int Offset { get; set; }
        public string Name { get; set; }
        public bool Initialized { get; set; }

        public int tempReg;

        public GlobalVariableToken(DataType type, string name, int offset) : base(type)
        {
            Name = name;
            Offset = offset;
            Initialized = false;
        }

        public override void PrepareValue(StringBuilder file)
        {
            tempReg = Tokens.registerUsed++;
        }

        public override Destination GetDestination()
        {
            return new Destination() { Type = Destination.REGISTER, OffSet = tempReg };
        }

        public override void DumpValue(StringBuilder file)
        {
            file.AppendLine($"mov|imm3|mem3 {Destination.RegisterName[tempReg]} null {Offset}");
            Tokens.registerUsed--;
        }

        public override KeyValuePair<string, string> getVariabele(int position)
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
