using CLCC.codeblock;
using System.Text;
using System.Xml.XPath;

namespace CLCC.tokens
{
    public interface IToken
    {
        bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true);
        void print(string indentation);
        void writeAss(StringBuilder file, Destination destination);
    }

    public class Tokens
    {
        public static int registerUsed = 0;

        public static IToken[] registeredTokens = new IToken[]
        {
            new FunctionBlock(""),
            new AssignOperatorToken(),
            new BinaryOperatorToken("*", 4),
            new BinaryOperatorToken("/", 4),
            new BinaryOperatorToken("+", 3),
            new BinaryOperatorToken("-", 3),
            new BinaryOperatorToken(">>", 2),
            new BinaryOperatorToken("<<", 2),
            new BinaryOperatorToken("&&", 0),
            new BinaryOperatorToken("||", 0),
            new BinaryOperatorToken("&", 1),
            new BinaryOperatorToken("|", 1),
            new BinaryOperatorToken("^", 1),
            new ExpressionParenthesisToken(),
            new CodeBlock(),
            new EndOfFileToken(), 
            new NumberToken(),
            new NewVariableToken(),
            new LocalVariableToken()
        };

        public static string matchName(ref string str)
        {
            string name = "";
            while(
                (str[0] >= 'a' && str[0] <= 'z') || 
                (str[0] >= 'A' && str[0] <= 'Z') ||
                (str[0] >= '0' && str[0] <= '9'))
            {
                name += str[0];
                str = str[1..];
            }
            return name;
        }

        public static IToken match(ref string str, List<IToken> allTokens, bool add = true)
        {
            foreach (IToken token in registeredTokens)
            {
                if (token.match(ref str, allTokens, out IToken? result, add) && result is not null) 
                    return result;
            }

            Console.WriteLine($"failed to read token at {(str.Length < 10 ? str : str[..10])}");
            return new EndOfFileToken();
        }

        public static void fixString(ref string str)
        {
            while (str.Length != 0 && (str[0] == ' ' || str[0] == '\n' || str[0] == '\r' || str[0] == '\t'))
            {
                str = str.Remove(0, 1);
            }
        }

        public static DataType getAdjustedDataType(IExpressionToken left, IExpressionToken right)
        {
            if (left.Type == right.Type)
            {
                return left.Type;
            }

            if (left.Type.isPrimitive && left.Type.name == "null") return right.Type;

            Console.WriteLine("Error: operands does not have the same data type");
            return new("null");
        }
    }
}
