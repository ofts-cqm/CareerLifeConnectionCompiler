using clcc;
using System.Text;

namespace CLCC.tokens
{
    public class NewVariableToken : IToken
    {
        public LocalVariableToken Variable { get; set; }

        public NewVariableToken(LocalVariableToken variable)
        {
            Variable = variable;
        }

        public NewVariableToken() { }

        public bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!str.StartsWith("var")) return false;

            str = str[3..];
            Tokens.fixString(ref str);
            string name = "";
            for (int i = 0; i < str.Length && ((str[0] >= 'a' && str[0] <= 'z') || (str[0] >= 'A' && str[0] <= 'Z')); i++)
            {
                name += str[i];
                str = str[1..];
            }

            Variable = new(Lexer.LocalVariables.Count, name);
            Lexer.LocalVariables.Add(name, Variable);
            result = new NewVariableToken(Variable);
            Tokens.fixString(ref str);
            if (add) allTokens.Add(Variable);
            return true;
        }

        public void print(string indentation)
        {
            Console.WriteLine(indentation + "New Variable Token");
            Variable.print(indentation + "    ");
        }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
