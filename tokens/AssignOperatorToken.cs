using clcc;
using System.Text;

namespace CLCC.tokens
{
    public class AssignOperatorToken : BinaryOperatorToken
    {
        public AssignOperatorToken() : base("=", -1) { }

        public AssignOperatorToken(IValueToken variable, IExpressionToken value) : base("=", -1, false, variable, value) { }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            Pos varPos = Content.GetPos();
            if (!Content.Match("=")) return false;
            Pos pos = Content.GetPos();
            bool register = false;

            IValueToken? variable = null;
            if (allTokens.Last() is LocalVariableToken local)
            {
                variable = local;
            }
            else if (allTokens.Last() is GlobalVariableToken global)
            {
                variable = global;
                if (Lexer.Current == null)
                {
                    if (global.Initialized)
                    {
                        Content.LogWarn("Global Variable Initialized Multiple Times", pos);
                        Lexer.initTokens.Remove(global);
                    }
                    add = false;
                    global.Initialized = true;
                    register = true;
                }
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

                if (result is AssignOperatorToken assign1 && assign1.Right.Type == DataType.NULLPTR)
                {
                    assign1.Right.Type = assign1.Left.Type;
                }

                allTokens.RemoveAt(allTokens.Count - 1);
                if (register) Lexer.initTokens.Add((GlobalVariableToken)variable, (AssignOperatorToken)result);
                if (add) allTokens.Add(result);
                return true;
            }
            Content.LogError("Expected A Variable before = sign", varPos);
            return false;
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            Destination operationDestination = (Left as IValueToken)?.GetDestination() ?? destination;
            Right.writeAss(file, operationDestination);

            if (Right is IValueToken value)
            {
                var variable = value.getVariabele(1);
                file.Append("mov").Append(variable.Key);
                decodeDestination(operationDestination, file, out string offset);
                file.Append(' ').Append(variable.Value).Append(" null ").Append(offset).Append('\n');
            }

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
