using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class FunctionBlock : IBlockToken, IExpressionBaseToken
    {
        public override string Type => "Function";

        public DataType ReturnType = DataType.NULL;

        DataType IExpressionBaseToken.Type
        {
            get { return ReturnType; }
            set { ReturnType = value; }
        }

        public FunctionBlock(string name, DataType returnType) : base(name) { ReturnType = returnType; }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!CLCC.Content.Match("func ")) return false;

            string name = Tokens.matchName();

            if(!DataType.TryParseDataType(out DataType type))
            {
                type = DataType.NULL;
                CLCC.Content.LogWarn("Unknown Datatype");
            }
            FunctionBlock block = new(name, type)
            {
                Parent = Lexer.Current
            };
            Lexer.Context.Push(block);

            IToken token = Tokens.match(new(), false);

            if (token is not CodeBlock code)
            {
                CLCC.Content.LogError("Expected Code Block, Found " + token.GetType());
                return false;
            }

            block.Content = code;
            Lexer.Context.Pop();
            if(add) allTokens.Add(block);
            result = block;
            return true;
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            file.Append("label ").Append(Name).Append('\n');
            file.Append("var|imm1 ").Append(LocalValue.Count + SubVariableCount).Append(" null null\n");
            Content.writeAss(file, new Destination() { Type = Destination.CLOSE});
            file.Append("ret null null null\n");
        }
    }
}
