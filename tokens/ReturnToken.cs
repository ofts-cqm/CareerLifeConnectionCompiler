using clcc;
using System.Linq.Expressions;
using System.Text;

namespace CLCC.tokens
{
    public class ReturnToken : IToken
    {
        public IExpressionToken? value;

        public readonly Destination EAX = new Destination() { Type = Destination.REGISTER, OffSet = 0 };

        public ReturnToken(IExpressionToken? value)
        {
            this.value = value;
        }

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!Content.Match("return")) return false;
            List<IToken> tokens = new();
            IToken matched = null;
            Content.Push();
            while (matched is not EndOfStatementToken)
            {
                Content.Ignore();
                Content.Push();
                matched = Tokens.match(tokens, true);
            }
            Content.Pop();
            if (tokens.Count > 1)
            {
                Content.LogError("Multiple Return");
                return false;
            }
            if (tokens.Count == 0)
            {
                return varify(null, allTokens, ref result, add);
            }
            if (tokens[0] is not IExpressionToken expression)
            {
                Content.LogError("Return Value is not a expression");
                return false;
            }

            return varify(expression, allTokens, ref result, add);
        }

        public bool varify(IExpressionToken? expression, List<IToken> allTokens, ref IToken? result, bool add)
        {
            if (!Lexer.Current?.TryReturn(expression) ?? false)
            {
                Content.LogError("Return Value Type Mismatched");
                return false;
            }

            result = new ReturnToken(expression);
            if (add) allTokens.Add(result);
            return true;
        }

        public void print(string indentation)
        {
            if (value is null)
            {
                Console.WriteLine(indentation + "Return Void");
            }
            else if(value is IValueToken val)
            {
                Console.Write(indentation + "Return ");
                val.print("");
            }
            else
            {
                Console.Write(indentation + "Return Expression");
                value.print(indentation + "    ");
                Console.WriteLine("END");
            }
        }

        public void writeAss(StringBuilder file, Destination destination)
        {
            if (value is not null)
            {
                if (value is IValueToken val)
                {
                    KeyValuePair<string, string> fixes = val.getVariabele(1);
                    file.Append("mov").Append(fixes.Key).Append(' ').Append(fixes.Value).Append(" null eax\n");
                }
                else
                {
                    Tokens.registerUsed++;
                    value.writeAss(file, EAX);
                    Tokens.registerUsed--;
                }
            }
            file.Append("ret null null null\n");
        }
    }
}
