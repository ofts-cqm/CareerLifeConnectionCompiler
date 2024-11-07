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

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!DataType.TryParseDataType(out DataType type)) return false;
            string name = Tokens.matchName();

            Variable = new(Lexer.Current?.LocalValue.Count ?? 0, name, type);//new(Lexer.LocalVariables.Count, name);
            Lexer.Current?.LocalValue.Add(name, Variable);//Lexer.LocalVariables.Add(name, Variable);
            result = new NewVariableToken(Variable);
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
