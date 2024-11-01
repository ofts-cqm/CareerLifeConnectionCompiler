using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class FunctionBlock : IBlockToken
    {
        public override string Type => "Function";

        public FunctionBlock(string name) : base(name) { }

        public override bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!str.StartsWith("func ")) return false;
            str = str[5..];

            string name = Tokens.matchName(ref str);
            Tokens.fixString(ref str);

            FunctionBlock block = new(name)
            {
                Parent = Lexer.Current
            };
            Lexer.Context.Push(block);

            IToken token = Tokens.match(ref str, new(), false);

            if (token is not CodeBlock code)
            {
                Console.WriteLine("Error: Expected Code Block, Found " + token.GetType());
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
