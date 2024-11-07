using System.Text;

namespace CLCC.tokens
{
    public class FloatIntCastOperatorToken : IExpressionToken
    {
        public IExpressionToken Right { get; set; }

        public FloatIntCastOperatorToken() : base(DataType.NULL) { }

        public FloatIntCastOperatorToken(string target, IExpressionToken right):base(new(target))
        {
            Right = right;
        }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            Content.Push();
            if (!Content.Match("("))
            {
                Content.Pop();
                return false;
            }

            string type = "";
            if (Content.Match("int)")) type = "int";
            else if (Content.Match("float)")) type = "float";

            if (type == "")
            {
                Content.Pop();
                return false;
            }
            else Content.Ignore();

            if (Tokens.match(allTokens, false) is not IExpressionToken right)
            {
                Content.LogError("Expected Expression");
                Content.Pop();
                return false;
            }

            result = new FloatIntCastOperatorToken(type, right);
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
