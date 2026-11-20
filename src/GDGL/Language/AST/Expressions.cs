namespace GDGL.Language.AST;

class NumberNode : ASTNode
{
    public int Value { get; }

    public NumberNode(int value)
    {
        Value = value;
    }

    public override int Evaluate() => Value;
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

    public override int Evaluate()
    {
        return Operator switch
        {
            ArithmeticOperator.Add => Left.Evaluate() + Right.Evaluate(),
            ArithmeticOperator.Subtract => Left.Evaluate() - Right.Evaluate(),
            ArithmeticOperator.Multiply => Left.Evaluate() * Right.Evaluate(),
            ArithmeticOperator.Divide => Left.Evaluate() / Right.Evaluate(),
            _ => throw new Exception("Unknown operator")
        };
    }
}

enum ArithmeticOperator
{
    Add,
    Subtract,
    Multiply,
    Divide
}
