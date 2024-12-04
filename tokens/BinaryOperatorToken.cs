using System.Text;

namespace CLCC.tokens
{
    public class BinaryOperatorToken : IExpressionToken
    {
        public static readonly Dictionary<string, string> codeName = new()
        {
            { "+", "add" },
            { "-", "sub" },
            { "*", "mul" },
            { "/", "div" },
            { "<<", "shl" },
            { ">>", "shr" },
            { ">", "gtr"},
            { "<", "lsr"},
            { ">=", "geq"},
            { "<=", "leq"},
            { "&&", "band" },
            { "||", "bor" },
            { "&", "and" },
            { "|", "or" },
            { "^", "xor" },
            { "==", "equ"}
        };

        public static Destination SecondDestination = new() { OffSet = 4, Type = Destination.REGISTER };

        public string Operator { get; set; }
        public int Precedence { get; set; }
        public string CodeName => (Type == DataType.FLOAT ? "f": "") + codeName[Operator];
        public IExpressionToken Left { get; set; }
        public IExpressionToken Right { get; set; }

        public bool recursive = false;

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!Content.Match(Operator)) return false;

            bool IsRecursive = Content.Match("=");

            if (Tokens.match(allTokens, false) is not IExpressionToken right)
            {
                Content.LogError("Expected Expression");
                return false;
            }
            
            if (allTokens.Count > 0 && allTokens.Last() is BinaryOperatorToken opToken
                && opToken.Precedence < Precedence)
            {
                IExpressionToken target = opToken;
                BinaryOperatorToken last = opToken;
                while (target is BinaryOperatorToken binOp && binOp.Precedence < Precedence) 
                {
                    last = binOp;
                    target = binOp.Right;
                }

                last.Right = new BinaryOperatorToken(Operator, Precedence, IsRecursive, last.Right, right);
                result = opToken;
            }
            else
            {
                if (allTokens.Last() is not IExpressionToken left)
                {
                    Content.LogError("Expected Expression, found " + allTokens.Last().GetType());
                    return false;
                }

                result = new BinaryOperatorToken(Operator, Precedence, IsRecursive, left, right);
                allTokens.RemoveAt(allTokens.Count - 1);
                if (add) allTokens.Add(result);
            }

            if (Operator == "==" || Operator == "&&" || Operator == "||")
            {
                ((BinaryOperatorToken)result).Type = DataType.BOOL;
            }
            return true;
        }

        public BinaryOperatorToken(string @operator, int precedence, bool recursive, IExpressionToken left, IExpressionToken right): base(DataType.NULL)
        {
            Operator = @operator;
            Precedence = precedence;
            Left = left;
            Right = right;
            this.recursive = recursive;
            Type = Tokens.getAdjustedDataType(this);
        }

        public BinaryOperatorToken(string @operator, int precedence): base(DataType.NULL)
        {
            Operator = @operator;
            Precedence = precedence;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}Binary Operator {Operator} Evaluating {Type}");
            Left.print(indentation + "    ");
            Right.print(indentation + "    ");
        }

        

        public override void writeAss(StringBuilder file, Destination destination)
        {
            if (destination.Type == Destination.CLOSE && !recursive)
            {
                Left.writeAss(file, destination);
                Right.writeAss(file, destination);
                return;
            }
            else if (destination.Type == Destination.CLOSE && recursive && Left is IValueToken LValue)
            {
                recursive = false;
                destination = LValue.GetDestination();
            }

            StringBuilder code = new StringBuilder().Append(CodeName);
            bool shoudPop = false;

            string leftValue;
            {
                if (Left is IValueToken value)
                {
                    var pair = value.getVariabele(1);
                    code.Append(pair.Key);
                    leftValue = pair.Value;
                }
                else
                {
                    Left.writeAss(file, destination);
                    if (destination.Type == Destination.REGISTER)
                    {
                        leftValue = Destination.RegisterName[destination.OffSet];
                    }
                    else
                    {
                        leftValue = destination.OffSet.ToString();
                    }
                }
            }

            string rightValue;
            {
                if (Right is IValueToken value)
                {
                    var pair = value.getVariabele(2);
                    code.Append(pair.Key);
                    rightValue = pair.Value;
                }
                else
                {
                    if (Tokens.registerUsed == 5)
                    {
                        Right.writeAss(file, SecondDestination);
                        rightValue = "esi";
                        file.Append("push edi null null");
                        shoudPop = true;
                    }
                    else
                    {
                        Right.writeAss(file, new Destination() { OffSet = Tokens.registerUsed++, Type = Destination.REGISTER });
                        rightValue = Destination.RegisterName[--Tokens.registerUsed];
                    }
                }
            }

            if (shoudPop)
            {
                file.Append("pop null null esi");
            }

            decodeDestination(destination, code, out string endValue);
            
            file.Append(code).Append(' ')
                .Append(leftValue).Append(' ')
                .Append(rightValue).Append(' ')
                .Append(endValue).Append('\n');
        }
    }
}
