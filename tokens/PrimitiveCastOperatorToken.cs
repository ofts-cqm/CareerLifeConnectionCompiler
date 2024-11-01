using System.Text;

namespace CLCC.tokens
{
    public class PrimitiveCastOperatorToken : IExpressionToken
    {
        public IExpressionToken Right { get; set; }

        public PrimitiveCastOperatorToken() : base(DataType.NULL) { }

        public PrimitiveCastOperatorToken(string target, IExpressionToken right):base(new(target))
        {
            Right = right;
        }

        public override bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (str.Length < 5 || str[0] != '(') return false;

            string type = "";
            if (str[1..5] == "int)")
            {
                str = str[5..];
                Tokens.fixString(ref str);
                type = "int";
            }
            else if (str[1..7] == "float)")
            {
                str = str[7..];
                Tokens.fixString(ref str);
                type = "float";
            }

            if (type == "") return false;

            if (Tokens.match(ref str, allTokens, false) is not IExpressionToken right)
            {
                Console.WriteLine("Error: Expected Expression");
                return false;
            }

            result = new PrimitiveCastOperatorToken(type, right);
            if (add) allTokens.Add(result);
            return true;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}Cast to {Type.name} Token");
            Right.print(indentation + "    ");
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            StringBuilder code = new StringBuilder().Append(Type.name);
            string rightValue;
            if (Right is IValueToken value)
            {
                var pair = value.getVariabele(2);
                code.Append(pair.Key);
                rightValue = pair.Value;
            }
            else
            {
                Right.writeAss(file, destination);
                BinaryOperatorToken.decodeDestination(destination, code, out rightValue, 1);
            }
            BinaryOperatorToken.decodeDestination(destination, code, out string targetValue);

            file.Append(code).Append(' ').Append(rightValue).Append(" null ").Append(targetValue).Append('\n');
        }
    }
}
