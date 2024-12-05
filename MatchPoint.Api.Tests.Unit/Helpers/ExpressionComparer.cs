using System.Linq.Expressions;

namespace MatchPoint.Api.Tests.Unit.Helpers
{
    internal class ExpressionComparer
    {
        /// <summary>
        /// Compares two <see cref="Expression"/> for equality.
        /// This actually compares the internal of the two Expressions,
        /// it does not just check for reference equality.
        /// </summary>
        public static bool Compare(Expression? expr1, Expression? expr2)
        {
            if (expr1 == expr2)
                return true;

            if (expr1 == null || expr2 == null || expr1.NodeType != expr2.NodeType || expr1.Type != expr2.Type)
                return false;

            switch (expr1)
            {
                case LambdaExpression lambda1 when expr2 is LambdaExpression lambda2:
                    if (lambda1.Parameters.Count != lambda2.Parameters.Count) return false;
                    for (int i = 0; i < lambda1.Parameters.Count; i++)
                    {
                        if (!Compare(lambda1.Parameters[i], lambda2.Parameters[i]))
                        {
                            return false;
                        }
                    }
                    return Compare(lambda1.Body, lambda2.Body);
                case BinaryExpression binary1 when expr2 is BinaryExpression binary2:
                    return binary1.Method == binary2.Method
                        && Compare(binary1.Left, binary2.Left)
                        && Compare(binary1.Right, binary2.Right);
                case UnaryExpression unary1 when expr2 is UnaryExpression unary2:
                    return unary1.NodeType == unary2.NodeType
                        && unary1.Method == unary2.Method
                        && Compare(unary1.Operand, unary2.Operand);
                case MemberExpression member1 when expr2 is MemberExpression member2:
                    return member1.Member == member2.Member
                        && Compare(member1.Expression, member2.Expression);
                case ConstantExpression const1 when expr2 is ConstantExpression const2:
                    return Equals(const1.Value, const2.Value);
                case ParameterExpression param1 when expr2 is ParameterExpression param2:
                    return param1.Name == param2.Name && param1.Type == param2.Type;
                default:
                    return false;
            }
        }
    }
}
