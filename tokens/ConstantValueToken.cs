using clcc;
using CLCC.codeblock;
using System.Text;

namespace CLCC.tokens
{
    public class ConstantValueToken : IValueToken
    {
        public string label;
        public static Dictionary<string, ConstantValueToken> names = new();
        public int[] values;

        public ConstantValueToken(string label, int[] values, DataType type) : base(type)
        {
            this.label = "CArray_" + label;
            this.values = values;
            names.TryAdd(label, this);
        }

        public ConstantValueToken() : base(DataType.NULL) { }

        public override Destination GetDestination()
        {
            throw new NotImplementedException();
        }

        public override KeyValuePair<string, string> getVariabele(int position)
        {
            return new($"|imm{position}|mem{position}", $"const|{label}");
        }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            Content.Push();
            string tempName = Tokens.matchName();
            if (names.TryGetValue(tempName, out ConstantValueToken? val))
            {
                result = val;
                if (add) allTokens.Add(val);
                Content.Ignore();
                return true;
            }

            Content.Pop();
            if (!Content.Match("const")) return false;
            if (!DataType.TryParseDataType(out DataType type))
            {
                Content.LogError("Datatype not found");
                return false;
            }
            string name = Tokens.matchName();

            if (!Content.Match("[]")) Content.LogWarn("Unnecesary Global Constant");
            else type = new(type) { isArray = true };
            Content.Match("=");
            Pos pos = Content.GetPos();

            if (Tokens.match(new(), false) is not CodeBlock block)
            {
                Content.LogError("Codeblock not found", pos);
                return false;
            }

            List<int> values = new();
            foreach (IToken token in block.insideTokens)
            {
                if (token is NumberToken num)
                {
                    values.Add(num.number);
                }
                else if (token is not EndOfItemToken)
                {
                    Content.LogError($"Array Element {token} is not a number");
                }
            }

            result = new ConstantValueToken(name, values.ToArray(), type);
            Lexer.SegmentData.Add((ConstantValueToken)result);
            return true;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}Const Value Type {Type}, label {label}");
        }

        public void writeData(StringBuilder file)
        {
            file.Append("label ").AppendLine(label);
            foreach (int i in values)
            {
                file.Append(i).Append(' ');
            }
            file.AppendLine();
        }
    }
}
