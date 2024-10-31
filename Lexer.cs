using CLCC;
using CLCC.codeblock;
using CLCC.tokens;
using System.Text;

namespace clcc
{
    class Lexer
    {
        //public static Dictionary<string, LocalVariableToken> LocalVariables { get; set; } = new();
        public static List<IToken> tokens = new();
        public static Stack<IBlockToken> Context = new();
        public static IBlockToken? Current => Context.Count > 0 ? Context.Peek() : null;

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

            Console.WriteLine("Lexing Finished");
        }

        public static void Main(string[] args)
        {
            

            if (args.Length == 0)
            {
                while (true)
                {
                    string fileContent = "";
                    while (true)
                    {
                        Console.Write(">");
                        string readValue = Console.ReadLine() ?? "";

                        fileContent += readValue + "\n";
                        if (readValue.StartsWith('.'))
                        {
                            break;
                        }
                    }
                    
                    if (fileContent == ".quit\n") return;

                    if (fileContent == ".clear\n")
                    {
                        tokens.Clear();
                        continue;
                    }
                    
                    if (fileContent == ".compile\n")
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
                string fileContent;
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