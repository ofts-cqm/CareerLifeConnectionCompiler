using System.Text;

namespace CLCC.tokens
{
    public class AssignOperatorToken : BinaryOperatorToken
    {
        public AssignOperatorToken() : base("=", -1) { }

        public AssignOperatorToken(IValueToken variable, IExpressionToken value) : base("=", -1, variable, value) { }

        public override bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!str.StartsWith('=')) return false;
            str = str[1..];
            Tokens.fixString(ref str);

            if (allTokens.Last() is LocalVariableToken variable)
            {
                IExpressionToken? token = Tokens.match(ref str, allTokens, false) as IExpressionToken;

                if(token is null)
                {
                    Console.WriteLine("Error: Expected Expression");
                    return false;
                }

                result = new AssignOperatorToken(variable, token);

                if (result is AssignOperatorToken assign && assign.Left.Type.isPrimitive && assign.Left.Type.name == "null")
                {
                    assign.Left.Type = assign.Right.Type;
                }

                allTokens.RemoveAt(allTokens.Count - 1);
                if (add) allTokens.Add(result);
                return true;
            }
            Console.WriteLine("Error: Expected A Variable before = sign");
            return false;
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            Destination operationDestination = (Left as IValueToken)?.GetDestination() ?? destination;
            Right.writeAss(file, operationDestination);

            if (destination.Type != Destination.CLOSE)
            {
                StringBuilder code = new();
                decodeDestination(operationDestination, code, out string value1);
                decodeDestination(destination, code, out string value2);

                file.Append("mov").Append(code).Append(' ')
                    .Append(value1).Append(" null ").Append(value2).Append('\n');
            }
        }
    }
}
