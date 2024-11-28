using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class IfBlock : IBlockToken
    {
        public ElseBlock? ElseBlock { get; set; } = null;
        public IExpressionToken Condition { get; set; }
        public bool returned = false;
        public static int IfCount = 0;

        public IfBlock(IExpressionToken condition) : base($"If_{IfCount++}")
        {
            Condition = condition;
        }

        public IfBlock() : base("") { }

        public override string Type => "If";

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!CLCC.Content.Match("if")) return false;
            if (Lexer.Current == null)
            {
                CLCC.Content.LogError("If Statement Without Context");
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

            IfBlock ifStatement = new((IExpressionToken)expression.insideTokens[0])
            {
                Parent = Lexer.Current
            };
            Lexer.Context.Push(ifStatement);

            pos = CLCC.Content.GetPos();
            List<IToken> parsed = new();
            IToken parsedToken = Tokens.match(parsed);
            while(parsedToken is not CodeBlock && parsedToken is not EndOfStatementToken && parsedToken is not IBlockToken)
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
            Lexer.Current.SubVariableCount = ifStatement.LocalValue.Count;
            ifStatement.Content = parsedCode;
            result = ifStatement;

            ifStatement.ElseBlock = ElseBlock.match();
            if (ifStatement.returned && (ifStatement.ElseBlock?.returned ?? false))
            {
                ifStatement.Parent.TryReturn(new NumberToken(1, DataType.BOOL));
            }
            if (add) allTokens.Add(ifStatement);
            return true;
        }

        public override bool TryReturn(IExpressionToken? expression)
        {
            if ((expression?.Type ?? DataType.NULL) != ExpectedReturnType) return false;
            returned = true;
            return true;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}If Condition:");
            Condition.print(indentation + "    ");
            Console.WriteLine(indentation + "End Condition");
            base.print(indentation);
            ElseBlock?.print(indentation);
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            string dest = "eax", postfix = "";
            if (Condition is IValueToken value)
            {
                var variable = value.getVariabele(1);
                dest = variable.Value;
                postfix = variable.Key;
            }
            else
            {
                Condition.writeAss(file, new Destination() { OffSet = 0, Type = Destination.REGISTER});
            }
            file.Append($"n|je|imm3{postfix} {dest} null {Name}_Else\n");
            Content.writeAss(file, new Destination { Type = Destination.CLOSE });
            if (ElseBlock is not null)
            {
                file.Append($"jmp|imm1 {Name}_End\n");
            }
            file.Append($"label {Name}_Else\n");
            ElseBlock?.writeAss(file, destination);
        }
    }
}
