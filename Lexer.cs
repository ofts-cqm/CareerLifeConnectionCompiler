using CLCC;
using CLCC.tokens;
using System.Text;

namespace clcc
{
    class Lexer
    {
        public static Dictionary<string, LocalVariableToken> LocalVariables = new();
        public static List<IToken> tokens = new();

        public static void Lex(string fileContent)
        {
            IToken? token;

            do
            {
                token = Tokens.match(ref fileContent, tokens);
            }
            while (token is not EndOfFileToken);

            foreach (IToken token1 in tokens)
            {
                token1.print("");
            }
        }

        public static void Main(string[] args)
        {
            string fileContent;

            if (args.Length == 0)
            {
                while (true)
                {
                    Console.Write(">");
                    fileContent = Console.ReadLine() ?? "";
                    if (fileContent == ".quit") return;
                    if (fileContent == ".compile")
                    {
                        StringBuilder builder = new();
                        Destination destination = new Destination() { Type = Destination.CLOSE};
                        foreach(IToken token in tokens)
                        {
                            token.writeAss(builder, destination);
                        }
                        Console.WriteLine(builder);
                        continue;
                    }
                    Lex(fileContent);
                }
            }
            else if (args.Length == 1)
            {
                try
                {
                    fileContent = File.ReadAllText(args[0]);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Cannot read file {args[0]}");
                    Console.WriteLine(ex.ToString());
                    return;
                }
                Lex(fileContent);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Usage: ./CLCC <file>");
                return;
            }
        }
    }
}