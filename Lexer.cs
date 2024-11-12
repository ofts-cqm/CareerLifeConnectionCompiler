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
        public static Dictionary<string, FunctionBlock> Functions = new();
        public static Stack<IBlockToken> Context = new();
        public static Dictionary<string, GlobalVariableToken> GlobalVariables = new();
        public static IBlockToken? Current => Context.Count > 0 ? Context.Peek() : null;
        public static int CurrentOffset = 1024*1024;

        public static bool GetVariable(string name, out GlobalVariableToken variableToken) => GlobalVariables.TryGetValue(name, out variableToken); 

        public static void Reset()
        {
            tokens.Clear();
            Context.Clear();
            GlobalVariables.Clear();
            Functions.Clear();
            CurrentOffset = 1024 * 1024;
        }

        public static void Lex(bool print)
        {
            IToken? token;

            do
            {
                token = Tokens.match(tokens);
            }
            while (token is not EndOfFileToken);

            if (print)
            {
                Console.WriteLine("Tokens:\n");
                foreach (IToken token1 in tokens)
                {
                    if (token1 is not FunctionBlock) token1.print("");
                }
                Console.WriteLine("Functions:\n");
                foreach(FunctionBlock fb in Functions.Values)
                {
                    fb.print("");
                }
            }

            Console.WriteLine("Lexing Finished");
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                while (true)
                {
                    bool compile = Content.Analyze();
                    Lex(!compile);
                    if (compile)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (IToken token in tokens)
                        {
                            token.writeAss(sb, new() { Type = Destination.CLOSE });
                        }
                        Console.WriteLine(sb.ToString());
                    }
                }
            }
            else if (args.Length == 1)
            {
                Content.Analyze(args[0]);
                Lex(true);
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