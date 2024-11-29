using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public abstract class ILoopToken : IConditionalBlock
    {
        public List<LoopBreakContinueToken> breakTokens = new();

        protected ILoopToken(string name) : base(name)
        {
        }

        public override bool TryReturn(IExpressionToken? expression)
        {
            if (IsConditionAlwaysTrue(true) && breakTokens.Count == 0)
            {
                return Parent?.TryReturn(expression) ?? false;
            }
            return (expression?.Type ?? DataType.NULL) == ExpectedReturnType;
        }

        public void BreakLoop(StringBuilder file) => file.Append("jmp|imm3 null null ").Append(Name).AppendLine("_End");
        public void ContinueLoop(StringBuilder file) => file.Append("jmp|imm3 null null ").Append(Name).AppendLine("_Start");
    }
}
