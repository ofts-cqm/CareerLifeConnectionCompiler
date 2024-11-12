using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class ParameterBlock : IToken
    {
        public LocalVariableToken[] parameters;
        public static ParameterBlock Instance = new(null);

        public ParameterBlock(LocalVariableToken[] parameters)
        {
            this.parameters = parameters;
        }

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            Content.Push();
            result = null;
            if (Content.Match("("))
            {
                List<IToken> generalTokens = new();
                IToken? matched = Tokens.match(generalTokens);
                while (!(matched is ExpressionParenthesisToken parenthesis && parenthesis.right))
                {
                    matched = Tokens.match(generalTokens);
                }

                LocalVariableToken[] locals = new LocalVariableToken[generalTokens.Count];
                for (int i = 0; i < generalTokens.Count; i++)
                {
                    if (generalTokens[i] is LocalVariableToken local)
                    {
                        locals[i] = local;
                    }
                    else
                    {
                        Content.LogError("Not a parameter block");
                        Content.Pop();
                        return false;
                    }
                }

                result = new ParameterBlock(locals);
                Content.Ignore();
                return true;
            }
            else if (Content.Match(")"))
            {
                result = new ExpressionParenthesisToken() { right = true };
                Content.Ignore();
                return true;
            }
            Content.Pop();
            return false;
        }

        public void print(string indentation)
        {
            Console.Write(this);
        }

        public override string ToString()
        {
            StringBuilder str = new("(");
            foreach (LocalVariableToken token in parameters)
            {
                str.Append(token.Type.ToString() + ", ");
            }
            str.Append(')');
            return str.ToString();
        }

        public string assName()
        {
            StringBuilder str = new("_");
            foreach (LocalVariableToken token in parameters)
            {
                str.Append(token.Type.ToString() + "_");
            }
            return str.ToString();
        }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
