using CLCC.codeblock;
using System.Text;

namespace CLCC.tokens
{
    public interface IToken
    {
        bool match(List<IToken> allTokens, out IToken? result, bool add = true);
        void print(string indentation);
        void writeAss(StringBuilder file, Destination destination);
    }

    public class Tokens
    {
        public static int registerUsed { get;
            set; } = 0;

        public static IToken[] registeredTokens = new IToken[]
        {
            new EndOfFileToken(),
            new EndOfStatementToken(),
            new EndOfItemToken(),
            new PostfixUnaryOperatorToken(),
            new StructDotExpression(),
            new ArraySubscriptOperator(),
            new ConstantValueToken(),
            new GlobalVolatileArray(),
            new ReturnToken(null),
            new IfBlock(),
            new WhileBlock(null),
            new ForBlock(),
            new StructToken(DataType.NULL),
            new LoopBreakContinueToken(false, null),
            new FunctionCallToken("", null, DataType.NULL),
            new FunctionBlock("", DataType.NULL),
            new BinaryOperatorToken("==", 0),
            new BinaryOperatorToken("!=", 0),
            new AssignOperatorToken(),
            new BinaryOperatorToken("*", 4),
            new BinaryOperatorToken("/", 4),
            new BinaryOperatorToken("+", 3),
            new BinaryOperatorToken("-", 3),
            new BinaryOperatorToken(">>", 2),
            new BinaryOperatorToken("<<", 2),
            new BinaryOperatorToken(">=", 0),
            new BinaryOperatorToken("<=", 0),
            new BinaryOperatorToken(">", 0),
            new BinaryOperatorToken("<", 0),
            new BinaryOperatorToken("&&", -1),
            new BinaryOperatorToken("||", -1),
            new BinaryOperatorToken("&", 1),
            new BinaryOperatorToken("|", 1),
            new BinaryOperatorToken("^", 1),
            new FloatIntCastOperatorToken(),
            new NoEffectVariableCastToken(),
            new ExpressionParenthesisToken(),
            new CodeBlock(),
            new NumberToken(),
            new LocalVariableToken(),
            new NewVariableToken(),
        };

        public static string matchName()
        {
            string name = "";
            while(
                (Content.CurrentChar >= 'a' && Content.CurrentChar <= 'z') || 
                (Content.CurrentChar >= 'A' && Content.CurrentChar <= 'Z') ||
                (Content.CurrentChar >= '0' && Content.CurrentChar <= '9'))
            {
                name += Content.CurrentChar;
                Content.Advance();
            }
            Content.Fix();
            return name;
        }

        public static IToken match(List<IToken> allTokens, bool add = true)
        {
            Pos pos = Content.GetPos();
            foreach (IToken token in registeredTokens)
            {
                if (token.match(allTokens, out IToken? result, add))
                {
                    if (result is not null) return result;
                    Content.LogWarn("Read null token");
                }   
            }

            Content.LogError("Failed to read token", pos);
            Content.AdvanceRow();
            return new EndOfFileToken();
        }

        [Obsolete]
        public static void fixString(ref string str)
        {
            while (str.Length != 0 && (str[0] == ' ' || str[0] == '\n' || str[0] == '\r' || str[0] == '\t'))
            {
                str = str.Remove(0, 1);
            }
        }

        public static DataType getAdjustedDataType(BinaryOperatorToken token)
        {
            DataType left = token.Left.Type, right = token.Right.Type;
            if (left == right)
            {
                return left;
            }

            if (!right.isPrimitive) return left;

            // type inference
            if (left == DataType.NULL) return right;

            // auto cast (avoid strong type)
            if (token is not AssignOperatorToken)
            {
                if (left == DataType.INT && right == DataType.FLOAT)
                {
                    token.Left = new FloatIntCastOperatorToken("float", token.Left);
                    return DataType.FLOAT;
                }
            }

            if (left == DataType.FLOAT && right == DataType.INT)
            {
                token.Right = new FloatIntCastOperatorToken("float", token.Right);
                return DataType.FLOAT;
            }

            Console.WriteLine("Error: operands does not have the same data type");
            return DataType.NULL;
        }
    }
}
