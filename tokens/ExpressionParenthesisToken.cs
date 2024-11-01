﻿using System.Text;

namespace CLCC.tokens
{
    public class ExpressionParenthesisToken : IExpressionToken
    {
        public List<IToken> insideTokens = new();
        public bool right = false;

        public ExpressionParenthesisToken() : base(DataType.NULL) { }

        public override bool match(ref string str, List<IToken> allTokens, out IToken? result, bool add = true)
        {
            if (str.StartsWith('('))
            {
                str = str[1..];
                Tokens.fixString(ref str);
                ExpressionParenthesisToken result1 = new ExpressionParenthesisToken();
                IToken? matched = Tokens.match(ref str, result1.insideTokens);

                while (!(matched is ExpressionParenthesisToken parenthesis && parenthesis.right))
                {
                    //insideTokens.Add(matched);
                    matched = Tokens.match(ref str, result1.insideTokens);
                }

                if (result1.insideTokens.Count > 1)
                {
                    Console.WriteLine("Error: ExpressionParenthesisToken is expected to contain only one token");
                    result = null;
                    return false;
                }

                if (result1.insideTokens[0] is not IExpressionToken expression)
                {
                    Console.WriteLine("Error: ExpressionParenthesisToken is expected to contain an expression");
                    result = null;
                    return false;
                }
                
                result1.Type = expression.Type;
                result = result1;
                if (add) allTokens.Add(result);
                return true;
            }
            else if (str.StartsWith(')'))
            {
                str = str[1..];
                Tokens.fixString(ref str);
                result = new ExpressionParenthesisToken() { right = true };
                return true;
            }
            result = null;
            return false;
        }

        public override void print(string indentation)
        {
            Console.WriteLine(indentation + "ParenthesisToken");
            foreach (IToken token in insideTokens) 
            { 
                token.print(indentation + "    ");
            }
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            foreach (IToken token in insideTokens)
            {
                token.writeAss(file, destination);
            }
        }
    }
}
