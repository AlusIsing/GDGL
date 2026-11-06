namespace GDGL.Language;

using Superpower.Display;

public enum TokenType
{
    Identifier,
    String,
    Integer,
    Float,
    
    [Token(Category = "operator", Example = "+")]
    Plus,

    Colon, // :
    Comma, // ,
    LeftParen, // (
    RightParen, // )
    
    NL, // new line
    EOF, // end of file
}
