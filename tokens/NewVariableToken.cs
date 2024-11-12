using clcc;
using System.Text;

namespace CLCC.tokens
{
    public class NewVariableToken : IToken
    {
        public IValueToken Variable { get; set; }

        public static NewVariableToken Instance = new();

        public NewVariableToken(IValueToken variable)
        {
            Variable = variable;
        }

        public NewVariableToken() { }

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!DataType.TryParseDataType(out DataType type)) return false;
            string name = Tokens.matchName();

            if (Lexer.Current is not null)
            {
                Variable = new LocalVariableToken(Lexer.Current.LocalCount, name, type);
                if(!Lexer.Current.LocalValue.TryAdd(name, (LocalVariableToken)Variable))
                {
                    Content.LogWarn("Repetitive Variable Declaration");
                }
            }
            else
            {
                Variable = new GlobalVariableToken(type, name, --Lexer.CurrentOffset);
                if (!Lexer.GlobalVariables.TryAdd(name, (GlobalVariableToken)Variable))
                {
                    Content.LogWarn("Repetitive Variable Declaration");
                }
            }
            
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
