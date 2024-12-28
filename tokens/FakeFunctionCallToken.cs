using clcc;
using System.Text;

namespace CLCC.tokens
{
    public delegate void FakeFunctionResolver(Destination dest, string[] args, StringBuilder file);

    public class FakeFunctionCallToken: IExpressionToken
    {
        public static Dictionary<string, FakeFunctionResolver> fakeFunctions = new()
        {
            { "getchar", getchar },
            { "init", (dest, args, file)=>{ } },
            { "refresh", refresh },
            { "setColor", setColor}
        };

        public static void getchar(Destination dest, string[] args, StringBuilder file)
        {
            if (dest.Type == Destination.CLOSE)
            {
                file.AppendLine("getchar null null null");
                return;
            }
            file.Append("getchar");
            decodeDestination(dest, file, out string val);
            file.Append(" null null ").AppendLine(val);
        }

        public static void refresh(Destination dest, string[] args, StringBuilder file)
        {
            file.AppendLine("refresh null null null\n");
        }

        public static void setColor(Destination dest, string[] args, StringBuilder file)
        {
            if (args.Length != 4)
            {
                Console.Error.WriteLine($"Expected 2 arguments, found {args.Length / 2.0}");
                return;
            }

            file.Append("color").Append(args[0]).Append(args[2]).Append(' ').Append(args[1]).Append(' ').Append(args[3]).AppendLine(" null\n");
        }

        public string Name { get; set; }

        public FunctionArgumentsToken Args { get; set; }

        public FakeFunctionCallToken(string name, FunctionArgumentsToken args) : base(DataType.INT)
        {
            this.Name = name;
            this.Args = args;
        }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            throw new NotImplementedException();
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}Fake Function Name {Name} with args:");
            Args.print(indentation + "    ");
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            List<string> args = new();
            int increasedReg = 0;
            for (int i = 0; i < Args.insideTokens.Count; i++)
            {
                IToken token = Args.insideTokens[i];
                if (token is IValueToken value)
                {
                    args.Add(value.getVariabele(i).Key);
                    args.Add(value.getVariabele(i).Value);
                }
                else
                {
                    token.writeAss(file, new Destination()
                    {
                        Type = Destination.REGISTER,
                        OffSet = Tokens.registerUsed++
                    });
                    increasedReg++;
                    args.Add("");
                    args.Add(Destination.RegisterName[Tokens.registerUsed - 1]);
                }
            }
            fakeFunctions[Name].Invoke(destination, args.ToArray(), file);
            Tokens.registerUsed -= increasedReg;
        }
    }
}
