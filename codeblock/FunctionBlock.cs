using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class FunctionBlock : IBlockToken
    {
        public override string Type => "Function";

        public FunctionBlock(string name) : base(name) { }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!CLCC.Content.Match("func ")) return false;

            string name = Tokens.matchName();

            FunctionBlock block = new(name)
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
