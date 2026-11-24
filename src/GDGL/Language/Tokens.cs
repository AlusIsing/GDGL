namespace GDGL.Language;

using Superpower.Display;

public enum TokenType
{
    // reserved words
    CHARACTER,
    IMAGE,
    SET,
    SAY,
    SHOW,
    HIDE,

    Identifier,
    String,
    Integer,
    Float,
    
    [Token(Category = "operator", Example = "+")]
    Plus,
    Minus,
    Multiply,
    Divide,

    Colon, // :
    Comma, // ,
    LeftParen, // (
    RightParen, // )
    Equal, // =
    
    NL, // new line
    EOF, // end of file
}
