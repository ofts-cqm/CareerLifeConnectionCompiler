using clcc;
using System.Text;

namespace CLCC.tokens
{
    public class NewVariableToken : IToken
    {
        public IValueToken Variable { get; set; }

        public static NewVariableToken Instance = new();
        public static bool IsCreatingNewVar = false;
        public static DataType? CreatingType;

        public NewVariableToken(IValueToken variable)
        {
            Variable = variable;
        }

        public NewVariableToken() { }

        public bool match(DataType type, string name, Pos namePos, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            IsCreatingNewVar = true;
            CreatingType = type;

            if (Lexer.Current is not null)
            {
                Variable = new LocalVariableToken(Lexer.Current.LocalCount, name, type);
                if (!Lexer.Current.LocalValue.TryAdd(name, (LocalVariableToken)Variable))
                {
                    Content.LogWarn("Repetitive Variable Declaration", namePos);
                }
            }
            else
            {
                Variable = new GlobalVariableToken(type, name, --Lexer.CurrentOffset);
                if (!Lexer.GlobalVariables.TryAdd(name, (GlobalVariableToken)Variable))
                {
                    Content.LogWarn("Repetitive Variable Declaration", namePos);
                }
            }

            result = new NewVariableToken(Variable);
            if (add) allTokens.Add(Variable);
            return true;
        }

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!IsCreatingNewVar || CreatingType is null) return false;
            Pos namePos = Content.GetPos();
            Content.Match("struct ");
            bool isArr = Content.Match("*");
            if (isArr) CreatingType.isArray = true;
            bool matched = match(CreatingType, Tokens.matchName(), namePos, allTokens, out result, add);
            if (isArr) CreatingType.isArray = false;
            return matched;
        }

        public void print(string indentation)
        {
            Console.WriteLine(indentation + "New Variable Token");
            Variable.print(indentation + "    ");
        }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
