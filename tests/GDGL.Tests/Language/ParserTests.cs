// Author: Alus
//
// Descript:
// Test script for GDGL.Language.Lexer.cs
//
// Warnning:
// Don't use Console.Write
// Use Logger.WriteLine for print information
//

namespace GDGL.Tests.Language;

using GDGL.Language;
using GDGL.Language.AST;

using Superpower;
using Superpower.Model;
using Xunit.Abstractions;

public class ParserTests
{
    private readonly ITestOutputHelper Logger;

    public ParserTests(ITestOutputHelper output)
    {
        Logger = output;
    }

    [Fact]
    public void OneLineExpressionTest()
    {
        string input = "1 + 1";

        Tokenizer<TokenType> tokenizer = Lexer.Build();
        
        TokenList<TokenType> token_list = tokenizer.Tokenize(input);

        ASTNode ast = Parser.Script(token_list).Value;

        foreach (Token<TokenType> token in token_list)
        {
            Console.WriteLine(
                $"Kind: {token.Kind,-12} Token: {token}"
            );
        }

        Logger.WriteLine("\n========== RESULT ==========");
        Logger.WriteLine(ast.Evaluate().ToString());
    }
}