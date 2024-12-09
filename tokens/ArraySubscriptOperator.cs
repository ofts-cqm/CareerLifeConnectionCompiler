using System.Text;

namespace CLCC.tokens
{
    public class ArraySubscriptOperator : IExpressionToken, IAssignable
    {
        public IExpressionToken Left { get; set; }
        public IExpressionToken Offset { get; set; }

        public ArraySubscriptOperator(DataType type, IExpressionToken left, IExpressionToken offset) : base(type)
        {
            Offset = offset;
            Left = left;
        }

        public ArraySubscriptOperator() : base(DataType.NULL) { }

        public void DumpValue()
        {
            Tokens.registerUsed--;
        }

        public Destination GetDestination()
        {
            return new() { Type = Destination.DEREFERENCE, OffSet = Tokens.registerUsed - 1 };
        }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            IExpressionToken? lastToken = PostfixUnaryOperatorToken.findLast(allTokens, out IExpressionToken? parent);
            if (lastToken == null) return false;
            if (!Content.Match("[")) return false;
            if (!lastToken.Type.isArray) return false;

            Pos pos = Content.GetPos();
            List<IToken> tokens = new();
            IToken? matched = null;
            while (!Content.Match("]") && matched is not EndOfFileToken)
                matched = Tokens.match(tokens);

            if (matched is EndOfFileToken)
            {
                Content.LogError("Closing Array Subscript Not Found", pos);
                return false;
            }

            if (tokens.Count > 1)
            {
                Content.LogError("Multiple Explression Found", pos);
                return false;
            }

            if (tokens.Count == 0)
            {
                Content.LogError("Expected Expression Inside", pos);
                return false;
            }

            if (tokens[0] is not IExpressionToken exp)
            {
                Content.LogError($"Expected Expression, found {tokens[0]}", pos);
                return false;
            }

            result = new ArraySubscriptOperator(new(lastToken.Type) { isArray = false }, lastToken, exp);
            PostfixUnaryOperatorToken.replaceChild(allTokens, parent, (IExpressionToken)result, add);
            return true;
        }

        public void InitExpression(StringBuilder file, Destination destination)
        {
            string registerName = Destination.RegisterName[Tokens.registerUsed];

            Left.writeAss(file, new Destination() { OffSet = Tokens.registerUsed++, Type = Destination.REGISTER });
            if (Left is IValueToken value)
            {
                file.Append("mov");
                decodeDestination(destination, file, out string off);
                var variable = value.getVariabele(1);
                file.Append(variable.Key).Append(' ')
                    .Append(variable.Value).Append(" null ").Append(off).Append('\n');
            }

            string postFix = "", calculated;
            bool shouldPop = false;

            if (Offset is IValueToken Rvalue)
            {
                var pair = Rvalue.getVariabele(2);
                postFix = pair.Key;
                calculated = pair.Value;
            }
            else
            {
                if (Tokens.registerUsed == 5)
                {
                    Offset.writeAss(file, new() { OffSet = 4, Type = Destination.REGISTER });
                    calculated = "esi";
                    file.Append("push edi null null");
                    shouldPop = true;
                }
                else
                {
                    Offset.writeAss(file, new Destination() { OffSet = Tokens.registerUsed++, Type = Destination.REGISTER });
                    calculated = Destination.RegisterName[--Tokens.registerUsed];
                }
            }

            if (shouldPop)
            {
                file.Append("pop null null esi");
            }

            file.Append("add").Append(postFix);
            decodeDestination(destination, file, out string start, 1);
            file.Append($" {start} {calculated} {registerName}\n");
        }

        public void PrepareValue(StringBuilder file)
        {
            Destination destination = new() { Type = Destination.REGISTER, OffSet = Tokens.registerUsed };
            InitExpression(file, destination);
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}Array Subscript Token Typed {Left.Type}:\n{indentation}    Array:");
            Left.print(indentation + "    ");
            Console.WriteLine($"{indentation}    Offset:");
            Offset.print(indentation + "    ");
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            InitExpression(file, destination);
            file.Append("mov|mem1");
            decodeDestination(destination, file, out string end);
            file.Append($" {Destination.RegisterName[--Tokens.registerUsed]} null {end}\n");
        }
    }
}
