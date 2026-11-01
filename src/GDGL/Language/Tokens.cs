namespace GDGL.Language;

public enum TokenType
{
    Identifier,
    String,
    Integer,
    Float,

    Colon, // :
    Comma, // ,
    LeftParen, // (
    RightParen, // )
    
    NL, // new line
    EOF, // end of file
}

public record Token
(
    TokenType Type,
    string Lexeme,
    object? Literal,
    int Line,
    int Column
);