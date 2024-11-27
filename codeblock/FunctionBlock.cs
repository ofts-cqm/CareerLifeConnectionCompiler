using clcc;
using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public class FunctionBlock : IBlockToken
    {
        public override string Type => "Function";

        public DataType ReturnType = DataType.NULL;
        public ParameterBlock parameters;
        public bool finished = false;
        public bool returned = false;
        public string ID => "func_" + Name + parameters.assName();

        public FunctionBlock(string name, DataType returnType) : base(name) { ReturnType = returnType; }

        public override bool TryReturn(IExpressionToken? expression)
        {
            if ((expression?.Type ?? DataType.NULL) != ReturnType) return false;
            returned = true;
            return true;
        }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;

            CLCC.Content.Push();
            if(!DataType.TryParseDataType(out DataType type))
            {
                CLCC.Content.Pop();
                return false;
            }

            Pos namePos = CLCC.Content.GetPos();
            string name = Tokens.matchName();
            CLCC.Content.Push();

            FunctionBlock block = new(name, type)
            {
                Parent = Lexer.Current
            };
            Lexer.Context.Push(block);

            if (ParameterBlock.Instance.match(new(), out IToken? para, false) && para is ParameterBlock paraBlock)
            {
                block.parameters = paraBlock;
                if (Lexer.Functions.TryGetValue(name + paraBlock.ToString(), out FunctionBlock func))
                {
                    block = func;
                    Lexer.Context.Pop();
                    Lexer.Context.Push(func);
                    add = false;
                }
                CLCC.Content.Ignore();
            }
            else
            {
                Lexer.Context.Pop();
                CLCC.Content.Pop();
                return NewVariableToken.Instance.match(type, name, namePos, allTokens, out result, add);
            }

            if(!Lexer.Functions.TryAdd(block.ID, block) && Lexer.Functions.TryGetValue(block.ID, out FunctionBlock existingBlock) && existingBlock.Content != null)
            {
                CLCC.Content.Ignore();
                CLCC.Content.LogError("Duplicate Function Declaration", namePos);
                return true;
            }
            Lexer.RawFunctions.Add(name);

            CLCC.Content.Ignore();
            CLCC.Content.Push();
            IToken token = Tokens.match(new(), false);

            if (token is not CodeBlock code)
            {
                CLCC.Content.Pop();
            }
            else
            {
                block.Content = code;
                block.finished = true;
                CLCC.Content.Ignore();
                if (type != DataType.NULL && !block.returned)
                {
                    CLCC.Content.LogError("Not all code retruned a value");
                }
            }

            Lexer.Context.Pop();
            if(add) allTokens.Add(block);
            result = block;
            return true;
        }

        public override void print(string indentation)
        {
            Console.Write($"Function {Name}{parameters}::{ReturnType} ");
            Content.print(indentation);
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            file.Append("label ").Append(ID).Append('\n');
            file.Append("var|imm1 ").Append(LocalValue.Count + SubVariableCount).Append(" null null\n");
            Content.writeAss(file, new Destination() { Type = Destination.CLOSE});
            if (!returned) file.Append("ret null null null\n");
        }
    }
}
