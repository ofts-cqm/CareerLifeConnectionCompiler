﻿using System.Text;

namespace CLCC.tokens
{
    public class PostfixUnaryOperatorToken : IExpressionToken
    {
        public IExpressionToken Left;
        public byte Operator;

        public const byte PLUS = 0, MINUS = 1;

        public PostfixUnaryOperatorToken(IExpressionToken left, byte @operator) : base(DataType.INT)
        {
            Left = left;
            Operator = @operator;
        }

        public PostfixUnaryOperatorToken() : base(DataType.NULL) { }

        public override bool match(List<IToken> allTokens, out IToken? result, bool add = true)
        {
            result = null;
            IValueToken? lastToken = findLast(allTokens, out IExpressionToken? parent);
            if (lastToken == null) return false;

            if (Content.Match("++"))
            {
                result = new PostfixUnaryOperatorToken((IExpressionToken)lastToken, PLUS);
                replaceChild(allTokens, parent, (IExpressionToken)result);
                if (add) allTokens.Add(lastToken);
                return true;
            }
            
            if(Content.Match("--"))
            {
                result = new PostfixUnaryOperatorToken((IExpressionToken)lastToken, MINUS);
                replaceChild(allTokens, parent, (IExpressionToken)result);
                if (add) allTokens.Add(lastToken);
                return true;
            }
            return false;
        }

        public static void replaceChild(List<IToken> allTokens, IExpressionToken? parent, IExpressionToken child)
        {
            if (parent is null)
                allTokens.RemoveAt(allTokens.Count - 1);
            else if(parent is BinaryOperatorToken bin)
                bin.Right = child;
            else if(parent is ExpressionParenthesisToken exp)
                exp.insideTokens[0] = child;
        }

        public static IValueToken? findLast(List<IToken> allTokens, out IExpressionToken? parent)
        {
            parent = null;
            if (allTokens.Count == 0) return null;
            IToken current = allTokens.Last();

            while (true)
            {
                if (current is BinaryOperatorToken bin)
                {
                    parent = bin;
                    current = bin.Right;
                }
                else if (current is IValueToken val) return val;
                else if (current is ExpressionParenthesisToken exp)
                {
                    current = exp.insideTokens[0];
                    parent = exp;
                }
                else return null;
            }
        }

        public override void print(string indentation)
        {
            Console.WriteLine($"{indentation} Postfix Unary Operator {(Operator == PLUS ? "++" : "--")} evaluating {Type}");
        }

        public override void writeAss(StringBuilder file, Destination destination)
        {
            if (Left is not IValueToken val) return;

            KeyValuePair<string, string> before = val.getVariabele(1), after = val.getVariabele(3);
            file.Append(Operator == PLUS ? "add|imm2" : "sub|imm2").Append(before.Key).Append(after.Key)
                .Append(' ').Append(before.Value).Append(" 1 ").AppendLine(after.Value);

            if (destination.Type != Destination.CLOSE)
            {
                file.Append("mov").Append(before.Key);
                decodeDestination(destination, file, out string offset);
                file.Append(' ').Append(before.Value).Append(" null ").AppendLine(offset);
            }
        }
    }
}
