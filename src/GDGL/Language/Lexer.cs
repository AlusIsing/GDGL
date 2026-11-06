namespace GDGL.Language;

using System;
using System.Collections.Generic;

using Superpower;
using Superpower.Model;

public sealed class Lexer : Tokenizer<TokenType>
{
    public IEnumerable<Result<TokenType>> ScanTokens(TextSpan input)
    {
        return Tokenize(input);
    }
    protected override IEnumerable<Result<TokenType>> Tokenize(TextSpan input)
    {
        Result<char> next = input.ConsumeChar();

        while (next.HasValue)
        {
            TextSpan start = next.Location;
            TextSpan end = next.Remainder;

            switch (next.Value)
            {
                case ' ':
                    break;
                case '(':
                    yield return Result.Value(
                        TokenType.LeftParen,
                        start,
                        end
                    );
                    break;

                case ')':
                    yield return Result.Value(
                        TokenType.RightParen,
                        start,
                        end
                    );
                    break;

                case ',':
                    yield return Result.Value(
                        TokenType.Comma,
                        start,
                        end
                    );
                    break;

                case '"': // string scan
                    Result<TokenType> result = ScanString(end);

                    yield return result;

                    if (!result.HasValue)
                        yield break;

                    end = result.Remainder;
                    break;


                default:
                    if (char.IsDigit(next.Value)) // integer or float scan
                    {
                        var (numberEnd, isFloat) = ScanNumber(end);

                        yield return Result.Value(
                            isFloat ? TokenType.Float : TokenType.Integer,
                            start,
                            numberEnd
                        );

                        end = numberEnd;
                    }
                    else if (IsIdentifierStart(next.Value)) // identifier scan
                    {
                        while (true)
                        {
                            var c = end.ConsumeChar();

                            if (!c.HasValue || !IsIdentifierPart(c.Value))
                                break;

                            end = c.Remainder;
                        }

                        yield return Result.Value(
                            TokenType.Identifier,
                            start,
                            end
                        );

                        next = SkipTrivia(end);
                        continue;
                    }
                    else // uknow
                    {
                        yield return Result.Empty<TokenType>(
                            start,
                            $"無法辨識的字元：'{next.Value}'"
                        );

                        yield break;
                    }

                    break;
            }

            next = end.ConsumeChar();
        }
    }

    private static Result<char> SkipTrivia(TextSpan input)
    {
        var next = input.ConsumeChar();

        while (next.HasValue)
        {
            // ignore white space or Tab
            if (next.Value is ' ' or '\t')
            {
                next = next.Remainder.ConsumeChar();
                continue;
            }

            // ignore # but catch the new line
            if (next.Value == '#')
            {
                var remainder = next.Remainder;

                next = remainder.ConsumeChar();

                while (next.HasValue &&
                       next.Value is not '\n' and not '\r')
                {
                    next = next.Remainder.ConsumeChar();
                }

                continue;
            }

            return next;
        }

        return next;
    }

    private Result<TokenType> ScanString(TextSpan start)
    {
        TextSpan end = start;
        while (true)
        {
            var c = end.ConsumeChar();

            if (!c.HasValue)
            {
                return Result.Empty<TokenType>(
                    start,
                    "字串缺少結尾引號"
                );
            }

            if (c.Value == '"')
            {
                end = c.Remainder;
                break;
            }

            if (c.Value is '\r' or '\n')
            {
                return Result.Empty<TokenType>(
                    c.Location,
                    "字串不可直接跨行"
                );
            }

            if (c.Value == '\\')
            {
                var escaped = c.Remainder.ConsumeChar();

                if (!escaped.HasValue)
                {
                    return Result.Empty<TokenType>(
                        c.Location,
                        "字串結尾缺少跳脫字元"
                    );
                }

                if (escaped.Value is not ('"' or '\\' or 'n' or 'r' or 't'))
                {
                    return Result.Empty<TokenType>(
                        escaped.Location,
                        $"unspport char: \\{escaped.Value}"
                    );
                }

                end = escaped.Remainder;
            }
            else
            {
                end = c.Remainder;
            }
        }

        return Result.Value(
            TokenType.String,
            start,
            end
        );
    }

    private (TextSpan End, bool IsFloat) ScanNumber(
        TextSpan input)
    {
        TextSpan end = input;
        bool is_float = false;

        Result<char> current_char;

        while (true)
        {
            current_char = end.ConsumeChar();

            if (!current_char.HasValue)
            {
                break;
            }
            if (!char.IsDigit(current_char.Value))
            {
                break;
            }

            end = current_char.Remainder;
        }

        current_char = end.ConsumeChar();

        if (current_char.HasValue)
        {
            if (current_char.Value == '.')
            {
                is_float = true;

                current_char = current_char.Remainder.ConsumeChar();

                while (true)
                {
                    end = current_char.Remainder;
                    current_char = end.ConsumeChar();

                    if (!current_char.HasValue)
                    {
                        break;
                    }
                    if (!char.IsDigit(current_char.Value))
                    {
                        break;
                    }
                }
            }
        }

        return (end, is_float);
    }

    private static bool IsIdentifierStart(char c)
    {
        return c == '_' || char.IsLetter(c);
    }

    private static bool IsIdentifierPart(char c)
    {
        return c == '_' || char.IsLetterOrDigit(c);
    }
}
