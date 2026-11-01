namespace GDGL.Language;

using System.Globalization;

public sealed class Lexer
{
    private readonly string _source;

    private readonly List<Token> _tokens = [];

    private int _start;
    private int _current = 0;

    private int _line = 1; // line amount of source
    private int _column = 1; // char amount of current line

    private int _startLine = 1;
    private int _startColumn = 1;

    public Lexer(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        _source = source;
    }

    public IReadOnlyList<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            _startLine = _line;
            _startColumn = _column;

            ScanToken();
        }

        _tokens.Add(new Token(
            TokenType.EOF,
            string.Empty,
            null,
            _line,
            _column
        ));

        return _tokens;
    }

    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    private char GetNext()
    {
        char c = _source[_current++];
        
        _column++;

        return c;
    }

    private char CheckCurrent()
    {
        if (IsAtEnd())
            return '\0';

        return _source[_current];
    }

    private char CheckNext()
    {
        if (_current + 1 >= _source.Length)
            return '\0';

        return _source[_current + 1];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd() || _source[_current] != expected)
            return false;

        GetNext();

        return true;
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        string lexeme = _source[_start.._current];

        _tokens.Add(new Token(
            type,
            lexeme,
            literal,
            _startLine,
            _startColumn
        ));
    }

    private void AddNewLine()
    {
        AddToken(TokenType.NL);

        _line++;
        _column = 1;
    }

    private void SkipComment()
    {
        while (!IsAtEnd() &&
               CheckCurrent() != '\n' &&
               CheckCurrent() != '\r')
        {
            GetNext();
        }
    }

    private void ScanToken()
    {
        char c = GetNext();

        switch (c)
        {
            case '(':
                AddToken(TokenType.LeftParen);
                break;

            case ')':
                AddToken(TokenType.RightParen);
                break;

            case ',':
                AddToken(TokenType.Comma);
                break;

            case '\r':
                // Windows use \r\n for new line
                if (Match('\n'))
                {
                    AddNewLine();
                }
                break;

            case '\n':
                AddNewLine();
                break;

            case ' ':
            case '\t':
                // ignore space
                break;

            case '#':
                SkipComment();
                break;

            case '"':
                ScanString();
                break;

            default:
                if (char.IsDigit(c))
                {
                    ScanNumber();
                }
                else if (IsIdentifierStart(c))
                {
                    ScanIdentifier();
                }
                else
                {
                    throw CreateError(
                        $"無法辨識的字元：'{c}'"
                    );
                }

                break;
        }
    }

    private void ScanString()
    {
        var builder = new System.Text.StringBuilder();

        while (!IsAtEnd() && CheckCurrent() != '"')
        {
            char c = GetNext();

            if (c == '\r' || c == '\n')
            {
                throw CreateError("字串不可直接跨行");
            }

            if (c == '\\')
            {
                if (IsAtEnd())
                    throw CreateError("字串結尾缺少跳脫字元");

                char escaped = GetNext();

                c = escaped switch
                {
                    '"' => '"',
                    '\\' => '\\',
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    _ => throw CreateError(
                        $"不支援的跳脫字元：\\{escaped}"
                    )
                };
            }

            builder.Append(c);
        }

        if (IsAtEnd())
        {
            throw CreateError("字串缺少結尾引號");
        }

        // 消耗結尾的 "
        GetNext();

        AddToken(
            TokenType.String,
            builder.ToString()
        );
    }

    private void ScanNumber()
    {
        while (char.IsDigit(CheckCurrent()))
        {
            GetNext();
        }

        bool is_float = false;

        // 判斷是否為小數
        if (CheckCurrent() == '.' &&
            char.IsDigit(CheckNext()))
        {
            is_float = true;

            GetNext();

            while (char.IsDigit(CheckCurrent()))
            {
                GetNext();
            }
        }

        string lexeme = _source[_start.._current];

        if (!double.TryParse(
            lexeme,
            NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out double value))
        {
            throw CreateError($"無效的數字：{lexeme}");
        }

        if (is_float)
        {
            AddToken(TokenType.Float, value);
        }
        else
        {
            AddToken(TokenType.Integer, value);
        }
    }

    private void ScanIdentifier()
    {
        while (IsIdentifierPart(CheckCurrent()))
        {
            GetNext();
        }

        AddToken(TokenType.Identifier);
    }

    private static bool IsIdentifierStart(char c)
    {
        return c == '_' || char.IsLetter(c);
    }

    private static bool IsIdentifierPart(char c)
    {
        return c == '_' || char.IsLetterOrDigit(c);
    }

    private Exception CreateError(string message)
    {
        return new InvalidOperationException(
            $"Lexer Error at {_startLine}:{_startColumn} - {message}"
        );
    }

}