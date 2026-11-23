namespace GDGL.Language.AST;

class NumberNode : ASTNode
{
    public int Value { get; }

    public NumberNode(int value)
    {
        Value = value;
    }
}

class BinaryNode : ASTNode
{
    public ASTNode Left { get; }
    public ASTNode Right { get; }
    public ArithmeticOperator Operator { get; }

    public BinaryNode(
        ASTNode left,
        ArithmeticOperator op,
        ASTNode right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
}

enum ArithmeticOperator
{
    Add,
    Subtract,
    Multiply,
    Divide
}
