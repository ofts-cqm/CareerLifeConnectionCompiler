using CLCC.tokens;
using System.Text;

namespace CLCC.codeblock
{
    public abstract class IBlockToken : IToken
    {
        public string Name { get; set; }
        public abstract string Type { get; }
        public CodeBlock Content { get; set; }
        public IBlockToken? Parent { get; set; }
        public Dictionary<string, LocalVariableToken> LocalValue = new();
        private int _subVariableCount;
        public int SubVariableCount
        {
            get
            {
                return _subVariableCount;
            }
            set
            {
                _subVariableCount = Math.Max(value, _subVariableCount);
            }
        }

        public IBlockToken(string name)
        {
            Name = name;
            Content = new CodeBlock();
        }

        public abstract bool match(List<IToken> allTokens, out IToken? result, bool add = true);

        public virtual void print(string indentation)
        {
            Console.Write(indentation + Type + " " + Name + " ");
            Content.print(indentation);
        }

        public abstract void writeAss(StringBuilder file, Destination destination);

        public bool tryGetLocalVariable(string name, out LocalVariableToken token)
        {
            if (LocalValue.TryGetValue(name, out token)) return true;

            return Parent?.tryGetLocalVariable(name, out token) ?? false;
        }
    }
}
