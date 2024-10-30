using System.Text;

namespace CLCC.tokens
{
    public class BinaryOperatorToken : IToken
    {
        public static readonly Dictionary<string, string> codeName = new()
        {
            { "+", "add" },
            { "-", "sub" },
            { "*", "mul" },
            { "/", "div" },
            { "<<", "shl" },
            { ">>", "shr" },
            { "&&", "band" },
            { "||", "bor" },
            { "&", "and" },
            { "|", "or" },
            { "^", "xor" }
        };

        public static Destination FirstDestination = new() { OffSet = 4, Type = Destination.REGISTER};
        public static Destination SecondDestination = new() { OffSet = 5, Type = Destination.REGISTER };

        public string Operator { get; set; }
        public int Precedence { get; set; }
        public string CodeName => codeName[Operator];
        public IToken Left { get; set; }
        public IToken Right { get; set; }

        public virtual bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!str.StartsWith(Operator)) return false;

            str = str[Operator.Length..];
            Tokens.fixString(ref str);
            IToken right = Tokens.match(ref str, allTokens, false);

            if (allTokens.Count > 0 && allTokens.Last() is BinaryOperatorToken opToken
                && opToken.Precedence < Precedence)
            {
                opToken.Right = new BinaryOperatorToken(Operator, Precedence, opToken.Right, right);
                result = opToken;
            }
            else
            {
                result = new BinaryOperatorToken(Operator, Precedence, allTokens.Last(), right);
                allTokens.RemoveAt(allTokens.Count - 1);
                if (add) allTokens.Add(result);
            }

            return true;
        }

        public BinaryOperatorToken(string @operator, int precedence, IToken left, IToken right)
        {
            Operator = @operator;
            Precedence = precedence;
            Left = left;
            Right = right;
        }

        public BinaryOperatorToken(string @operator, int precedence)
        {
            Operator = @operator;
            Precedence = precedence;
        }

        public void print(string indentation)
        {
            Console.WriteLine(indentation + "Binary Operator " + Operator);
            Left.print(indentation + "    ");
            Right.print(indentation + "    ");
        }

        public void decodeDestination(Destination destination, StringBuilder code, out string value)
        {
            switch (destination.Type)
            {
                case Destination.REGISTER:
                    {
                        value = Destination.RegisterName[destination.OffSet];
                        break;
                    }
                case Destination.STACK:
                    {
                        code.Append("|imm3|mem3|sta3");
                        value = destination.OffSet.ToString();
                        break;
                    }
                case Destination.HEAP:
                    {
                        code.Append("|imm3|mem3");
                        value = destination.OffSet.ToString();
                        break;
                    }
                default:
                    {
                        value = "";
                        break;
                    }
            }
        }

        public virtual void writeAss(StringBuilder file, Destination destination)
        {
            StringBuilder code = new StringBuilder().Append(CodeName);
            string leftValue = "", rightValue = "", endValue = "";
            bool shoudPop = false, usedRegister = false;

            {
                if (Left is IValueToken value)
                {
                    var pair = value.getVariabele(1);
                    code.Append(pair.Key);
                    leftValue = pair.Value;
                }
                else if (Left is BinaryOperatorToken || Left is ParenthesisToken)
                {
                    if (Tokens.registerUsed < 4)
                    {
                        Left.writeAss(file, new Destination() 
                        { Type = Destination.REGISTER, OffSet = Tokens.registerUsed });
                        leftValue = Destination.RegisterName[Tokens.registerUsed++];
                        usedRegister = true;
                    }
                    else
                    {
                        shoudPop = true;
                        Left.writeAss(file, FirstDestination);
                        file.Append("push edi null null\n");
                        leftValue = "edi";
                    }
                    
                }
            }

            {
                if (Right is IValueToken value)
                {
                    var pair = value.getVariabele(2);
                    code.Append(pair.Key);
                    rightValue = pair.Value;
                }
                else if (Right is BinaryOperatorToken || Right is ParenthesisToken)
                {
                    Right.writeAss(file, SecondDestination);
                    rightValue = "esi";
                }
            }

            if (usedRegister)
            {
                Tokens.registerUsed--;
            }

            if (shoudPop)
            {
                file.Append("pop null null edi");
            }

            decodeDestination(destination, code, out endValue);
            
            file.Append(code).Append(' ')
                .Append(leftValue).Append(' ')
                .Append(rightValue).Append(' ')
                .Append(endValue).Append('\n');
        }
    }
}
