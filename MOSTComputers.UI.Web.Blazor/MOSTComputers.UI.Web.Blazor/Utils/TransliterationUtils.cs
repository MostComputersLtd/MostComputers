using System.Text;

namespace MOSTComputers.UI.Web.Blazor.Utils;

public class TransliterationUtils
{
    public enum TransliterationType
    {
        CyrillicToLatin,
        LatinToCyrillic
    }

    private static readonly Dictionary<char, string> _cyrillicToLatinMap = new()
    {
        // Basic Cyrillic (Russian/Bulgarian)
        ['А'] = "A", ['а'] = "a",
        ['Б'] = "B", ['б'] = "b",
        ['В'] = "V", ['в'] = "v",
        ['Г'] = "G", ['г'] = "g",
        ['Д'] = "D", ['д'] = "d",
        ['Е'] = "E", ['е'] = "e",
        ['Ё'] = "Yo", ['ё'] = "yo",
        ['Ж'] = "Zh", ['ж'] = "zh",
        ['З'] = "Z", ['з'] = "z",
        ['И'] = "I", ['и'] = "i",
        ['Й'] = "Y", ['й'] = "y",
        ['К'] = "K", ['к'] = "k",
        ['Л'] = "L", ['л'] = "l",
        ['М'] = "M", ['м'] = "m",
        ['Н'] = "N", ['н'] = "n",
        ['О'] = "O", ['о'] = "o",
        ['П'] = "P", ['п'] = "p",
        ['Р'] = "R", ['р'] = "r",
        ['С'] = "S", ['с'] = "s",
        ['Т'] = "T", ['т'] = "t",
        ['У'] = "U", ['у'] = "u",
        ['Ф'] = "F", ['ф'] = "f",
        ['Х'] = "H", ['х'] = "h",
        ['Ц'] = "Ts", ['ц'] = "ts",
        ['Ч'] = "Ch", ['ч'] = "ch",
        ['Ш'] = "Sh", ['ш'] = "sh",
        ['Щ'] = "Sht", ['щ'] = "sht",
        ['Ъ'] = "A", ['ъ'] = "a",   // Bulgarian hard vowel
        ['Ы'] = "Y", ['ы'] = "y",
        ['Ь'] = "", ['ь'] = "",    // soft sign omitted
        ['Э'] = "E", ['э'] = "e",
        ['Ю'] = "Yu", ['ю'] = "yu",
        ['Я'] = "Ya", ['я'] = "ya",

        // Common additional letters
        // Ukrainian
        ['Ґ'] = "G̀", ['ґ'] = "g̀",
        ['Є'] = "Ê", ['є'] = "ê",
        ['І'] = "Ì", ['і'] = "ì",
        ['Ї'] = "Ï", ['ї'] = "ï",

        // Belarusian
        ['Ў'] = "Ŭ", ['ў'] = "ŭ",

        // Serbian/Macedonian
        ['Ј'] = "J", ['ј'] = "j",
        ['Љ'] = "Ľ", ['љ'] = "ľ",
        ['Њ'] = "Ň", ['њ'] = "ň",
        ['Ћ'] = "Ć", ['ћ'] = "ć",
        ['Ђ'] = "Đ", ['ђ'] = "đ",
        ['Ќ'] = "Ḱ", ['ќ'] = "ḱ",
        ['Ѓ'] = "Ǵ", ['ѓ'] = "ǵ",
        ['Ѕ'] = "Ẑ", ['ѕ'] = "ẑ",
    };

    public static string Transliterate(ReadOnlySpan<char> input, TransliterationType transliterationDirection)
    {
        return transliterationDirection switch
        {
            TransliterationType.CyrillicToLatin => TransliterateCyrillicToLatin(input),
            TransliterationType.LatinToCyrillic => TransliterateLatinToCyrillic(input),
            _ => input.ToString()
        };
    }

    private static string TransliterateCyrillicToLatin(ReadOnlySpan<char> cyrillicInput)
    {
        if (cyrillicInput.Length == 0) return string.Empty;

        StringBuilder stringBuilder = new();

        foreach (char character in cyrillicInput)
        {
            if (_cyrillicToLatinMap.TryGetValue(character, out var latin))
            {
                stringBuilder.Append(latin);
            }
            else
            {
                stringBuilder.Append(character);
            }
        }

        return stringBuilder.ToString();
    }

    private static string TransliterateLatinToCyrillic(ReadOnlySpan<char> latinInput)
    {
        if (latinInput.Length == 0) return string.Empty;

        StringBuilder stringBuilder = new();

        int searchIndex = 0;

        while (searchIndex < latinInput.Length)
        {
            char matchedCharacter = '\0';
            int matchedLength = 0;

            foreach (KeyValuePair<char, string> kvp in _cyrillicToLatinMap)
            {
                string value = kvp.Value;
                int length = value.Length;

                if (length <= matchedLength
                    || length <= 0
                    || searchIndex + length > latinInput.Length
                    || !latinInput.Slice(searchIndex, length).SequenceEqual(value))
                {
                    continue;
                }

                matchedCharacter = kvp.Key;
                matchedLength = length;
            }

            if (matchedLength > 0)
            {
                stringBuilder.Append(matchedCharacter);

                searchIndex += matchedLength;
            }
            else
            {
                stringBuilder.Append(latinInput[searchIndex]);

                searchIndex++;
            }
        }

        return stringBuilder.ToString();
    }

    public static bool IsCyrillicOnly(ReadOnlySpan<char> input)
    {
        if (input.Length == 0) return false;

        foreach (char character in input)
        {
            if (char.IsWhiteSpace(character) || char.IsPunctuation(character) || char.IsDigit(character)) continue;

            // Cyrillic block: U+0400–U+04FF,
            // Extended Cyrillic U+0500–U+052F
            if ((character >= '\u0400' && character <= '\u04FF') ||
                (character >= '\u0500' && character <= '\u052F'))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    public static bool IsLatinOnly(ReadOnlySpan<char> input)
    {
        if (input.Length == 0) return false;

        foreach (char ch in input)
        {
            if (char.IsWhiteSpace(ch) || char.IsPunctuation(ch) || char.IsDigit(ch)) continue;

            // Basic Latin: U+0041–U+007A (A–Z, a–z),
            // Extended Latin ranges: U+00C0–U+024F
            if ((ch >= '\u0041' && ch <= '\u007A') ||
                (ch >= '\u00C0' && ch <= '\u024F'))
            {
                continue;
            }

            return false;
        }

        return true;
    }
}