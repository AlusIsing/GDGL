namespace GDGL.Language;

using System.Globalization;

using Superpower;
using Superpower.Model;
using Superpower.Parsers;

using GDGL.Language.AST;


public static class Parser
{
    static readonly TokenListParser<TokenType, ArithmeticOperator> Add =
        Token.EqualTo(TokenType.Plus).Value(ArithmeticOperator.Add);

    static readonly TokenListParser<TokenType, ArithmeticOperator> Subtract =
        Token.EqualTo(TokenType.Minus).Value(ArithmeticOperator.Subtract);

    static readonly TokenListParser<TokenType, ArithmeticOperator> Multiply =
        Token.EqualTo(TokenType.Multiply).Value(ArithmeticOperator.Multiply);

    static readonly TokenListParser<TokenType, ArithmeticOperator> Divide =
        Token.EqualTo(TokenType.Divide).Value(ArithmeticOperator.Divide);

    // 數字
    static readonly TokenListParser<TokenType, ASTNode> IntegerNumber =
        Token.EqualTo(TokenType.Integer)
            .Apply(Numerics.IntegerInt32)
            .Select(n => (ASTNode)new NumberNode(n));

    // 括號或數字
    static readonly TokenListParser<TokenType, ASTNode> Factor =
        (from left in Token.EqualTo(TokenType.LeftParen)
         from expr in Superpower.Parse.Ref(() => Expr)
         from right in Token.EqualTo(TokenType.RightParen)
         select expr)
        .Or(IntegerNumber);

    // 乘除
    static readonly TokenListParser<TokenType, ASTNode> Term =
        Superpower.Parse.Chain(
            Multiply.Or(Divide),
            Factor,
            (op, left, right) => new BinaryNode(left, op, right)
        );

    // 加減
    static readonly TokenListParser<TokenType, ASTNode> Expr =
        Superpower.Parse.Chain(
            Add.Or(Subtract),
            Term,
            (op, left, right) => new BinaryNode(left, op, right)
        );

    static readonly TokenListParser<TokenType, ASTNode> Stmts =
        Expr.AtLeastOnceDelimitedBy(Token.EqualTo(TokenType.NL))
            .Select(statements => (ASTNode)new StmtsNode(statements));

    // 完整解析器
    public static readonly TokenListParser<TokenType, ASTNode> Parse =
        Stmts.AtEnd();
}