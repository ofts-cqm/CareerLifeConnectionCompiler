using CLCC.codeblock;
using System.Text;

namespace CLCC.tokens
{
    public class FunctionArgumentsToken : IToken
    {
        public List<IToken> insideTokens = new();
        public FunctionBlock? currentContext;

        public FunctionArgumentsToken(List<IToken> insideTokens, FunctionBlock? functionBlock)
        {
            this.insideTokens = insideTokens;
            this.currentContext = functionBlock;
        }

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            throw new NotImplementedException();
        }

        public void print(string indentation)
        {
            Console.WriteLine(indentation + "ParenthesisToken");
            foreach (IToken token in insideTokens)
            {
                token.print(indentation + "    ");
            }
        }

        public string getSigniture()
        {
            if (insideTokens.Count == 0)
            {
                return "_noPara";
            }

            StringBuilder sb = new StringBuilder();
            foreach (IToken token in insideTokens)
            {
                if (token is IExpressionToken exp)
                {
                    sb.Append('_').Append(exp.Type.ToString());
                }
                else
                {
                    Content.LogWarn("Token is not expression");
                }
            }
            return sb.ToString();
        }

        public void writeAss(StringBuilder file, Destination destination)
        {
            for (int i = 0; i < Tokens.registerUsed - 1; i++)
            {
                file.Append($"push {Destination.RegisterName[i]} null null\n");
            }
            int offset = currentContext != null ? currentContext.LocalCount + currentContext.SubVariableCount + 2 : 2;
            for (int i = 0; i < insideTokens.Count; i++)
            {
                insideTokens[i].writeAss(file, new Destination() 
                { 
                    Type = Destination.STACK, 
                    OffSet = offset + i
                });
                if (insideTokens[i] is IValueToken value)
                {
                    var variable = value.getVariabele(1);
                    file.Append("mov|imm3|mem3|sta3").Append(variable.Key).Append(' ')
                        .Append(variable.Value).Append(" null ").Append(offset + i).Append('\n');
                }
            }
        }
    }
}
