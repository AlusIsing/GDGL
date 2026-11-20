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

using Superpower.Model;
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
        
    }
}