using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class StructToken : IToken
    {
        public List<KeyValuePair<string, DataType>> variables;

        public static Dictionary<DataType, StructToken> Structs = new();

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            Content.Push();
            if (!Content.Match("struct ")) return false;
            string name = Tokens.matchName();
            if (Lexer.Structures.Contains(name))
            {
                Content.Pop();
                return false;
            }
            Content.Ignore();
            Lexer.Structures.Add(name);

            Structs.Add(new(name, false), Lexer.CurrentStruct = new());
            if (!Content.Match("{"))
            {
                Content.LogWarn("Structure Delcaration should start with '{'");
            }
            
            return true;
        }

        public void print(string indentation)
        {
            throw new NotImplementedException();
        }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
