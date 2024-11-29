using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public abstract class IConditionalBlock : IBlockToken
    {
        public IExpressionToken? Condition;

        protected IConditionalBlock(string name) : base(name)
        {
        }

        public bool IsConditionAlwaysTrue(bool true_if_blank)
        {
            if (Condition is null) return true_if_blank;
            return Condition is NumberToken num && num.number == 1;
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation}If Condition:");
            Condition?.print(indentation + "    ");
            Console.WriteLine(indentation + "End Condition");
            base.print(indentation);
        }

        public void writeCondition(StringBuilder file, string label, bool jump_if_true, bool default_true = false)
        {
            string dest = "eax", postfix = "";
            if (Condition is IValueToken value)
            {
                var variable = value.getVariabele(1);
                dest = variable.Value;
                postfix = variable.Key;
            }
            else
            {
                if (Condition is null)
                {
                    if (!(jump_if_true ^ default_true))
                    {
                        file.Append($"jmp|imm3 null null {label}\n");
                    }
                    return;
                }
                Condition.writeAss(file, new Destination() { OffSet = 0, Type = Destination.REGISTER });
            }
            if (!jump_if_true) file.Append("n|");
            file.Append($"je|imm3{postfix} {dest} null {label}\n");
        }
    }
}
