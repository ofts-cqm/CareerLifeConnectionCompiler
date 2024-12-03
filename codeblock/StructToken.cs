using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class StructToken : IToken
    {
        public List<KeyValuePair<string, DataType>> variables = new();

        public DataType Type;

        public static Dictionary<DataType, StructToken> Structs = new();

        public StructToken(DataType type)
        {
            Type = type;
        }

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
            StructToken currentStruct = new(new(name, false));
            Structs.Add(currentStruct.Type, Lexer.CurrentStruct = currentStruct);
            if (!Content.Match("{"))
            {
                Content.LogWarn("Structure Delcaration should start with '{'");
            }

            while (!Content.Match("}"))
            {
                Content.Match("struct ");
                if (!DataType.TryParseDataType(out DataType type))
                {
                    Content.LogError("Unknown Datatype");
                    Content.AdvanceRow();
                }

                while (true)
                {
                    currentStruct.variables.Add(new(Tokens.matchName(), type));
                    if (Content.Match(";")) break;
                    if (!Content.Match(","))
                    {
                        Content.LogWarn("Unknown Token");
                        break;
                    }
                }
            }
            result = currentStruct;
            if (add) allTokens.Add(result);
            return true;
        }

        public void print(string indentation)
        {
            Console.WriteLine($"{indentation}Struct {Type.name} with {variables.Count} variables:");
            foreach (KeyValuePair<string, DataType> @var in variables)
            {
                Console.WriteLine($"{indentation}    {@var.Value} {@var.Key}");
            }
            Console.WriteLine("End Struct");
        }

        public void writeAss(StringBuilder file, Destination destination) { }
    }
}
