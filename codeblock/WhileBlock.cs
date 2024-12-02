using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class WhileBlock : ILoopToken
    {
        public static int WhileCount = 0;

        public WhileBlock(IExpressionToken Condition) : base($"While_{WhileCount++}")
        {
            base.Condition = Condition;
        }

        public override string Type => "While";

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!CLCC.Content.Match("while")) return false;
            if (Lexer.Current == null)
            {
                CLCC.Content.LogError("While Statement Without Context");
                return false;
            }

            Pos pos = CLCC.Content.GetPos();
            IToken temp = Tokens.match(new(), false);

            if (temp is not ExpressionParenthesisToken expression)
            {
                CLCC.Content.LogError("Expected expression", pos);
                return false;
            }
            if (expression.Type != DataType.BOOL && expression.Type != DataType.INT)
            {
                CLCC.Content.LogWarn("Expression evaluate type is not int or bool", pos);
            }

            WhileBlock whileStatement = new((IExpressionToken)expression.insideTokens[0])
            {
                Parent = Lexer.Current
            };
            Lexer.Context.Push(whileStatement);

            InLoop.Push(whileStatement);
            pos = CLCC.Content.GetPos();
            List<IToken> parsed = new();
            IToken parsedToken = Tokens.match(parsed);
            while (parsedToken is not CodeBlock && parsedToken is not EndOfStatementToken && parsedToken is not IBlockToken)
            {
                parsedToken = Tokens.match(parsed);
            }

            CodeBlock parsedCode;
            if (parsedToken is CodeBlock block) parsedCode = block;
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
            Lexer.Current.SubVariableCount = whileStatement.LocalValue.Count;
            whileStatement.Content = parsedCode;
            result = whileStatement;
            if (add) allTokens.Add(whileStatement);
            InLoop.Pop();
            return true;
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            file.Append($"label {Name}_Start\n");
            writeCondition(file, Name + "_End", false);
            Content.writeAss(file, new Destination { Type = Destination.CLOSE });
            file.Append($"label {Name}_End\n");
        }
    }
}
