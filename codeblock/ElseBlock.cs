using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class ElseBlock : IBlockToken
    {
        public bool returned { get; set; } = false;

        public ElseBlock() : base($"If_{IfBlock.IfCount - 1}")
        {
        }

        public override string Type => "Else";

        public static ElseBlock? match()
        {
            if (!CLCC.Content.Match("else")) return null;
            if (Lexer.Current == null)
            {
                CLCC.Content.LogError("Else Statement Without Context");
                return null;
            }

            Pos pos = CLCC.Content.GetPos();

            ElseBlock elseStatement = new()
            {
                Parent = Lexer.Current
            };
            Lexer.Context.Push(elseStatement);

            pos = CLCC.Content.GetPos();
            List<IToken> parsed = new();
            IToken parsedToken = Tokens.match(parsed);
            while (parsedToken is not CodeBlock && parsedToken is not EndOfStatementToken && parsedToken is not IBlockToken)
            {
                parsedToken = Tokens.match(parsed);
            }

            CodeBlock parsedCode;
            if (parsedToken is CodeBlock codeblock) parsedCode = codeblock;
            else if (parsedToken is EndOfStatementToken)
            {
                if (parsed.Count != 1)
                {
                    CLCC.Content.LogWarn("Multiple Statements Found", pos);
                }
                parsedCode = new CodeBlock() { insideTokens = parsed };
            }
            else
            {
                parsedCode = new CodeBlock();
                parsedCode.insideTokens.Add((IBlockToken)parsedToken);
            }

            Lexer.Context.Pop();
            Lexer.Current.SubVariableCount = elseStatement.LocalValue.Count;
            elseStatement.Content = parsedCode;
            return elseStatement;
        }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true) 
        {
            result = null;
            return false;
        }

        public override bool TryReturn(IExpressionToken? expression)
        {
            if ((expression?.Type ?? DataType.NULL) != ExpectedReturnType) return false;
            returned = true;
            return true;
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            Content.writeAss(file, new Destination { Type = Destination.CLOSE });
            file.Append($"label {Name}_End\n");
        }
    }
}
