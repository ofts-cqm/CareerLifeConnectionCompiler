using System.Text;

namespace CLCC.tokens
{
    public class AssignOperatorToken : BinaryOperatorToken
    {
        public AssignOperatorToken() : base("=", -1) { }

        public AssignOperatorToken(IValueToken variable, IExpressionToken value) : base("=", -1, variable, value) { }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!Content.Match("=")) return false;

            IValueToken? variable = null;
            if (allTokens.Last() is LocalVariableToken local)
            {
                variable = local;
            }
            else if (allTokens.Last() is GlobalVariableToken global)
            {
                variable = global;
            }
            
            if (variable is not null){
                IExpressionToken? token = Tokens.match(allTokens, false) as IExpressionToken;

                if(token is null)
                {
                    Content.LogError("Expected Expression");
                    return false;
                }

                result = new AssignOperatorToken(variable, token);

                if (result is AssignOperatorToken assign && assign.Left.Type == DataType.NULL)
                {
                    assign.Left.Type = assign.Right.Type;
                }

                allTokens.RemoveAt(allTokens.Count - 1);
                if (add) allTokens.Add(result);
                return true;
            }
            Content.LogError("Expected A Variable before = sign");
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
