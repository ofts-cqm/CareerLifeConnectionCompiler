using CLCC.codeblock;
using System.Text;

namespace CLCC.tokens
{
    public class StructDotExpression : PostfixUnaryOperatorToken
    {
        public int Offset;
        public const byte StructDot = 2;

        public StructDotExpression(int Offset, IExpressionToken Left, DataType type) : base(Left, StructDot)
        {
            this.Offset = Offset;
            this.Left = Left;
        }

        public StructDotExpression() : base(null, StructDot) { }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            IExpressionToken? lastToken = findLast(allTokens, out IExpressionToken? parent);
            if (lastToken == null) return false;
            if (!lastToken.Type.isPrimitive) return false;

            if (!Content.Match(".")) return false;
            if(!StructToken.Structs.TryGetValue(lastToken.Type, out StructToken @struct))
            {
                Content.LogError("Unknown Struct Type");
                return false;
            }
            string name = Tokens.matchName();
            int offset = @struct.variables.FindIndex(0, (p) => p.Key == name);

            if (offset == -1)
            {
                Content.LogError($"{name} Not Found in Struct {lastToken.Type.name}");
                return false;
            }

            result = new StructDotExpression(offset, lastToken, @struct.variables[offset].Value);
            replaceChild(allTokens, parent, (IExpressionToken)result);
            if (add) allTokens.Add(result);
            return true;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}{Left.Type.name}.{StructToken.Structs[Left.Type].variables[Offset].Key} (Offset {Offset}): {Type}");
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            if (destination.Type == Destination.CLOSE) return;

            Left.writeAss(file, destination);
            if (Left is IValueToken value)
            {
                file.Append("mov");
                BinaryOperatorToken.decodeDestination(destination, file, out string off);
                var variable = value.getVariabele(1);
                file.Append(variable.Key).Append(' ')
                    .Append(variable.Value).Append(" null ").Append(off).Append('\n');
            }

            string registerName = Destination.RegisterName[Tokens.registerUsed];

            file.Append("add|imm2");
            BinaryOperatorToken.decodeDestination(destination, file, out string start, 1);
            file.Append($" {start} {Offset} {registerName}\n");

            file.Append("mov|heap1");
            BinaryOperatorToken.decodeDestination(destination, file, out string end);
            file.Append($"{registerName} null {end}\n");
        }
    }
}
