namespace GDGL.Language;

using System.Globalization;

using Superpower;
using Superpower.Model;
using Superpower.Parsers;

using GDGL.Language.AST;

using GDGL.Runtime;


public class Parser
{
    readonly TokenListParser<TokenType, ArithmeticOperator> Add =
        Token.EqualTo(TokenType.Plus).Value(ArithmeticOperator.Add);

    readonly TokenListParser<TokenType, ArithmeticOperator> Subtract =
        Token.EqualTo(TokenType.Minus).Value(ArithmeticOperator.Subtract);

    readonly TokenListParser<TokenType, ArithmeticOperator> Multiply =
        Token.EqualTo(TokenType.Multiply).Value(ArithmeticOperator.Multiply);

    readonly TokenListParser<TokenType, ArithmeticOperator> Divide =
        Token.EqualTo(TokenType.Divide).Value(ArithmeticOperator.Divide);

    // 數字
    readonly TokenListParser<TokenType, ASTNode> IntegerNumber =
        Token.EqualTo(TokenType.Integer)
            .Apply(Numerics.IntegerInt32)
            .Select(n => (ASTNode)new NumberNode(n));

    // 括號或數字
    readonly TokenListParser<TokenType, ASTNode> Factor;

    // 乘除
    readonly TokenListParser<TokenType, ASTNode> Term;

    // 加減
    readonly TokenListParser<TokenType, ASTNode> Expr;

    readonly TokenListParser<TokenType, ASTNode> Stmts;

    // 完整解析器
    public readonly TokenListParser<TokenType, ASTNode> Parse;

    public Parser()
    {
        Factor =
            (from left in Token.EqualTo(TokenType.LeftParen)
             from expr in Superpower.Parse.Ref(() => Expr!)
             from right in Token.EqualTo(TokenType.RightParen)
             select expr)
            .Or(IntegerNumber);

        Term = Superpower.Parse.Chain(
            Multiply.Or(Divide),
            Factor,
            (op, left, right) => new BinaryNode(left, op, right)
        );

        Expr = Superpower.Parse.Chain(
            Add.Or(Subtract),
            Term,
            (op, left, right) => new BinaryNode(left, op, right)
        );

        Stmts =
            Expr.AtLeastOnceDelimitedBy(Token.EqualTo(TokenType.NL))
                .Select(statements => (ASTNode)new StmtsNode(statements));

        Parse = Stmts.AtEnd();
    }
}
