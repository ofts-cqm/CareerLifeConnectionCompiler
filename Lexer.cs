﻿using CLCC;
using CLCC.codeblock;
using CLCC.tokens;
using System.Text;

namespace clcc
{
    class Lexer
    {
        //public static Dictionary<string, LocalVariableToken> LocalVariables { get; set; } = new();
        public static List<IToken> tokens = new();
        public static Dictionary<GlobalVariableToken, AssignOperatorToken> initTokens = new();
        public static Dictionary<string, FunctionBlock> Functions = new();
        public static HashSet<string> Structures = new();
        public static HashSet<string> RawFunctions = new();
        public static HashSet<ConstantValueToken> SegmentData = new();
        public static Stack<IBlockToken> Context = new();
        public static Dictionary<string, GlobalVariableToken> GlobalVariables = new();
        public static IBlockToken? Current => Context.Count > 0 ? Context.Peek() : null;
        public static StructToken? CurrentStruct = null;
        public static int CurrentOffset = 1024*1024;

        public static bool GetVariable(string name, out GlobalVariableToken variableToken) => GlobalVariables.TryGetValue(name, out variableToken); 

        public static void Reset()
        {
            tokens.Clear();
            Context.Clear();
            GlobalVariables.Clear();
            Functions.Clear();
            RawFunctions.Clear();
            initTokens.Clear();
            Structures.Clear();
            SegmentData.Clear();
            ConstantValueToken.names.Clear();
            DataType.Init();
            StructToken.Structs.Clear();
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
                Console.WriteLine("Globals:\n");
                foreach(AssignOperatorToken assign in initTokens.Values)
                {
                    assign.print("");
                }
                Console.WriteLine("Functions:\n");
                foreach(FunctionBlock fb in Functions.Values)
                {
                    fb.print("");
                }
                Console.WriteLine("Out of Context Tokens:\n");
                foreach (IToken token1 in tokens)
                {
                    if (token1 is not FunctionBlock) token1.print("");
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
                        StringBuilder sb = new("label segment_init\n");
                        foreach (AssignOperatorToken token in initTokens.Values)
                        {
                            token.writeAss(sb, new() { Type = Destination.CLOSE });
                        }
                        sb.Append("call|imm1 func_main_noPara null null\n");

                        if (SegmentData.Count > 0)
                        {
                            sb.AppendLine("label segment_data");
                            foreach (ConstantValueToken constant in SegmentData)
                                constant.writeData(sb);
                        }

                        sb.AppendLine("label segment_code");
                        foreach (IToken token in tokens)
                        {
                            if (token is AssignOperatorToken) continue;
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

/*
int fibonachi(int value){
    if (value < 3) return 1;
    return fibonachi(value - 1) + fibonachi(value - 2);
}

void main(){
    fibonachi(3);
    fibonachi(5);
    fibonachi(6);
}
.end
 */