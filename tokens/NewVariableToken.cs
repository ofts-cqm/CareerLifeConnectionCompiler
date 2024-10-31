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
            if (!str.StartsWith("var ")) return false;

            str = str[4..];
            Tokens.fixString(ref str);
            string name = Tokens.matchName(ref str);

            Variable = new(Lexer.Current?.LocalValue.Count ?? 0, name);//new(Lexer.LocalVariables.Count, name);
            Lexer.Current?.LocalValue.Add(name, Variable);//Lexer.LocalVariables.Add(name, Variable);
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
