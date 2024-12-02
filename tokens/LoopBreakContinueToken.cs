using clcc;
using CLCC.codeblock;
using System.Text;

namespace CLCC.tokens
{
    public class LoopBreakContinueToken: IToken
    {
        public bool breakToken;
        public ILoopToken loopToken;

        public LoopBreakContinueToken(bool breakToken, ILoopToken loopToken)
        {
            this.breakToken = breakToken;
            this.loopToken = loopToken;
        }

        public bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            if (!ILoopToken.InLoop.TryPeek(out ILoopToken loop)) return false;
            if (Content.Match("break"))
            {
                result = new LoopBreakContinueToken(true, loop);
                loop.breakTokens.Add((LoopBreakContinueToken)result);
                if (add) allTokens.Add(result);
                return true;
            }
            else if (Content.Match("continue"))
            {
                result = new LoopBreakContinueToken(false, loop);
                if (add) allTokens.Add(result);
                return true;
            }
            return false;
        }

        public void print(string indentation)
        {
            Console.WriteLine(indentation + (breakToken ? "Break Loop" : "Continue Loop"));
        }

        public void writeAss(StringBuilder file, Destination destination)
        {
            if (breakToken) loopToken.BreakLoop(file);
            else loopToken.ContinueLoop(file);
        }
    }
}
