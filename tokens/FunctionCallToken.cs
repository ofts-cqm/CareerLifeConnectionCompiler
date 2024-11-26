using clcc;
using CLCC.codeblock;
using System.Text;

namespace CLCC.tokens
{
    public class FunctionCallToken : IExpressionToken
    {
        public string ID { get; set; } 
        public FunctionArgumentsToken Arguments { get; set; }

        public FunctionCallToken(string ID, FunctionArgumentsToken Arguments, DataType type) : base(type)
        {
            this.ID = ID;
            this.Arguments = Arguments;
        }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            Content.Push();
            string name = Tokens.matchName();
            if (!Lexer.RawFunctions.Contains(name))
            {
                //Console.WriteLine("does not contain name " + name);
                Content.Pop();
                return false;
            }
            IToken matched = Tokens.match(allTokens, false);
            if (matched is not FunctionArgumentsToken argument)
            {
                if (matched is ExpressionParenthesisToken expression)
                {
                    argument = new FunctionArgumentsToken(expression.insideTokens, Lexer.Current?.BaseFunction);
                }
                else
                {
                    Content.LogError("Function with this parameter not found!");
                    Content.Ignore();
                    return false;
                }
            }
            string id = $"func_{name}{argument.getSigniture()}";
            if (!Lexer.Functions.TryGetValue(id, out FunctionBlock? func) || func is null)
            {
                Content.LogError("Function with this parameter not found!");
                Content.Ignore();
                return false;
            }
            result = new FunctionCallToken(id, argument, func.ReturnType);
            if (add) allTokens.Add(result);
            return true;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}Call Function {ID} with Parameters:");
            foreach (IToken token in Arguments.insideTokens)
            {
                token.print(indentation + "    ");
            }
            Console.WriteLine(indentation + "END");
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            Arguments.writeAss(file, destination);
            file.Append("call|imm1 ").Append(ID).Append(" null null\n");
            
            if (destination.Type == Destination.REGISTER && destination.OffSet != 0)
            {
                file.Append("mov eax null ").Append(Destination.RegisterName[destination.OffSet]).Append('\n');
            }
            else if (destination.Type == Destination.STACK)
            {
                file.Append("mov|imm3|mem3|sta3 eax null ").Append(destination.OffSet).Append('\n');
            }
            else if(destination.Type == Destination.HEAP) 
            {
                file.Append("mov|imm3|mem3 eax null ").Append(destination.OffSet).Append('\n');
            }

            for (int i = Tokens.registerUsed - 2; i >= 0; i--)
            {
                file.Append($"pop null null {Destination.RegisterName[i]}\n");
            }
        }
    }
}
