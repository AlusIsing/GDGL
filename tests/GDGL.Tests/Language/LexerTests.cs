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

using Xunit.Abstractions;

public class LexerTests
{
    private readonly ITestOutputHelper Logger;

    public LexerTests(ITestOutputHelper output)
    {
        Logger = output;
    }

    [Fact]
    public void OneLineSTMTTest()
    {
        string stmt = "func(a, b, c)";
        Lexer lexer = new Lexer(stmt);

        IReadOnlyList<Token> tokens = lexer.ScanTokens();
        
        string tokens_str = "";

        foreach (Token token in tokens)
        {
            string one_line_str = String.Format("{0}: {1} line {2} column {3}\n",
                token.Type.ToString(),
                token.Lexeme,
                token.Line,
                token.Column
            );

            tokens_str = tokens_str + one_line_str;
            
            Logger.WriteLine(one_line_str);
        }

        string ans = "Identifier: func line 1 column 1\n"
            + "LeftParen: ( line 1 column 5\n"
            + "Identifier: a line 1 column 6\n"
            + "Comma: , line 1 column 7\n"
            + "Identifier: b line 1 column 9\n"
            + "Comma: , line 1 column 10\n"
            + "Identifier: c line 1 column 12\n"
            + "RightParen: ) line 1 column 13\n"
            + "EOF:  line 1 column 14\n";

        Assert.Equal(ans, tokens_str);
    }
}