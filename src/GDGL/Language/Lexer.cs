namespace GDGL.Language;

using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

public static class Lexer
{
    public static Tokenizer<TokenType> Build()
    {
        return new TokenizerBuilder<TokenType>()
            // ignore white space and Tab but keep new line
            .Ignore(
                Character.EqualTo(' ')
                    .Or(Character.EqualTo('\t'))
                    .AtLeastOnce()
            )
            .Match(QuotedString.CStyle, TokenType.String) // String
            .Match( // Float must define before Integer
                Span.MatchedBy(
                    Numerics.Natural.Then(
                        _ => Character.EqualTo('.')
                            .IgnoreThen(Numerics.Natural)
                    )
                ),
                TokenType.Float
            )
            .Match(Numerics.Natural, TokenType.Integer)
            .Match(Identifier.CStyle, TokenType.Identifier)
            .Match(Character.EqualTo('+'), TokenType.Plus)
            .Match(Character.EqualTo('-'), TokenType.Minus)
            .Match(Character.EqualTo('*'), TokenType.Multiply)
            .Match(Character.EqualTo('/'), TokenType.Divide)
            .Match(Character.EqualTo(':'), TokenType.Colon)
            .Match(Character.EqualTo('='), TokenType.Equal)
            .Match(Character.EqualTo(','), TokenType.Comma)
            .Match(Character.EqualTo('('), TokenType.LeftParen)
            .Match(Character.EqualTo(')'), TokenType.RightParen)
            .Match( // New Line (support for LF and CRLF)
                Span.EqualTo("\r\n")
                    .Or(Span.EqualTo("\n"))
                    .Or(Span.EqualTo("\r")),
                TokenType.NL
            )
            .Build();
    }
}
