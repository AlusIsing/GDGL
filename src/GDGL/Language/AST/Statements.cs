namespace GDGL.Language.AST;

class StmtsNode : ASTNode
{
    public IReadOnlyList<ASTNode> Statements { get; }

    public StmtsNode(IEnumerable<ASTNode> statements)
    {
        Statements = statements.ToArray();
    }
}
