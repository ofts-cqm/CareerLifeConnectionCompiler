using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class ForBlock : ILoopToken
    {
        public static int ForCount = 0;

        public IToken? init;
        public IToken? callback;

        public ForBlock() : base($"For_{ForCount++}")
        {
            this.init = null;
            base.Condition = null;
            this.callback = null;
        }

        public void initExp(IToken? init, IExpressionToken? condition, IToken? callback)
        {
            this.init = init;
            Condition = condition;
            this.callback = callback;
        }

        public override string Type => "For";

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!CLCC.Content.Match("for")) return false;
            if (Lexer.Current == null)
            {
                CLCC.Content.LogError("For Statement Without Context");
                return false;
            }

            ForBlock forStatement = new()
            {
                Parent = Lexer.Current
            };
            Lexer.Context.Push(forStatement);

            Pos pos = CLCC.Content.GetPos();
            ExpressionParenthesisToken.match(new(), out IToken? temp, false, true);

            if (temp is not FunctionArgumentsToken arguments)
            {
                CLCC.Content.LogError("For Loop Without Arguments", pos);
                Lexer.Context.Pop();
                return false;
            }

            IToken? init = null, callback = null;
            IExpressionToken? expression = null;
            List<IToken> matched = arguments.insideTokens;

            if (matched.Count > 5)
            {
                CLCC.Content.LogWarn("Redundent Expressions");
                goto createStatement;
            }

            if (matched.Count < 2)
            {
                CLCC.Content.LogWarn("Missing Expressions");
                goto createStatement;
            }

            if (matched[0] is not EndOfStatementToken)
            {
                init = matched[0];
                matched.RemoveAt(0);
            }
            matched.RemoveAt(0);

            if (matched[0] is not EndOfStatementToken)
            {
                if (matched[0] is not IExpressionToken exp) CLCC.Content.LogWarn("For condition is not expression", pos);
                else
                {
                    if (exp.Type != DataType.BOOL && exp.Type != DataType.INT) CLCC.Content.LogWarn("For condition is not int or bool", pos);
                    expression = exp;
                }
                matched.RemoveAt(0);
            }
            matched.RemoveAt(0);

            if (matched.Count > 0)
            {
                callback = matched[0];
                matched.RemoveAt(0);
            }

        createStatement:
            forStatement.initExp(init, expression, callback);

            InLoop.Push(forStatement);
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
            Lexer.Current.SubVariableCount = forStatement.LocalValue.Count;
            forStatement.Content = parsedCode;
            result = forStatement;
            if (add) allTokens.Add(forStatement);
            InLoop.Pop();
            return true;
        }

        public override void print(string indentation)
        {
            Console.WriteLine(indentation + "For Init Statement:");
            init?.print(indentation + "    ");
            Console.WriteLine(indentation + "End For Init");
            base.print(indentation);
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            Destination close = new() { Type = Destination.CLOSE };
            init?.writeAss(file, close);
            file.Append($"label {Name}_Start\n");
            writeCondition(file, Name + "_End", false, true);
            Content.writeAss(file, new Destination { Type = Destination.CLOSE });
            callback?.writeAss(file, close);
            file.Append($"label {Name}_End\n");
        }

        public override void ContinueLoop(StringBuilder file)
        {
            callback?.writeAss(file, new() { Type = Destination.CLOSE });
            base.ContinueLoop(file);
        }
    }
}
