using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.PDF.Utils;

internal static class CurrencyToWordsConversionUtils
{
    internal enum PriceConvertScaleEnum
    {
        Stotinki = 0,
        Leva = 1,
    }

    private readonly static string[] _levaUnitsMap = {
        "",
        "един",
        "два",
        "три",
        "четири",
        "пет",
        "шест",
        "седем",
        "осем",
        "девет"
    };

    private readonly static string[] _euroUnitsMap = {
        "",
        "едно",
        "две",
        "три",
        "четири",
        "пет",
        "шест",
        "седем",
        "осем",
        "девет"
    };

    private readonly static string[] _usdUnitsMap = {
        "",
        "един",
        "два",
        "три",
        "четири",
        "пет",
        "шест",
        "седем",
        "осем",
        "девет"
    };

    private readonly static string[] _stotinkiUnitsMap = {
        "",
        "една",
        "две",
        "три",
        "четири",
        "пет",
        "шест",
        "седем",
        "осем",
        "девет"
    };

    private readonly static string[] _euroAndUsdCentUnitsMap = {
        "",
        "един",
        "два",
        "три",
        "четири",
        "пет",
        "шест",
        "седем",
        "осем",
        "девет"
    };

    private readonly static string[] _teensMap = {
        "десет",
        "единадесет",
        "дванадесет",
        "тринадесет",
        "четиринадесет",
        "петнадесет",
        "шестнадесет",
        "седемнадесет",
        "осемнадесет",
        "деветнадесет"
    };

    private readonly static string[] _tensMap = {
        "",
        "",
        "двадесет",
        "тридесет",
        "четиридесет",
        "петдесет",
        "шестдесет",
        "седемдесет",
        "осемдесет",
        "деветдесет"
    };

    private readonly static string[] _hundredsMap = {
        "",
        "сто",
        "двеста",
        "триста",
        "четиристотин",
        "петстотин",
        "шестстотин",
        "седемстотин",
        "осемстотин",
        "деветстотин"
    };

    public static string GetCurrencyString(Currency currency)
    {
        return currency switch
        {
            Currency.EUR => "€",
            Currency.BGN => "лв",
            Currency.USD => "$",
            _ => throw new NotImplementedException("Currency is not supported"),
        };
    }

    public static string GetCurrencyNameString(Currency currency, PriceConvertScaleEnum priceConvertScaleEnum, long number)
    {
        return (priceConvertScaleEnum, currency) switch
        {
            (PriceConvertScaleEnum.Leva, Currency.EUR) => "евро",
            (PriceConvertScaleEnum.Stotinki, Currency.EUR) => number == 1 ? "цент" : "цента",

            (PriceConvertScaleEnum.Leva, Currency.BGN) => number == 1 ? "лев" : "лева",
            (PriceConvertScaleEnum.Stotinki, Currency.BGN) => number == 1 ? "стотинка" : "стотинки",

            (PriceConvertScaleEnum.Leva, Currency.USD) => number == 1 ? "долар" : "долара",
            (PriceConvertScaleEnum.Stotinki, Currency.USD) => number == 1 ? "цент" : "цента",

            _ => throw new NotImplementedException("Currency is not supported"),
        };
    }

    internal static string ConvertPriceToWords(decimal amount, Currency currency)
    {
        long leva = (long)amount;
        int stotinki = (int)Math.Round((amount - leva) * 100, 2, MidpointRounding.AwayFromZero);

        string[] unitsMap = GetUnitsMap(currency);
        string[] centUnitsMap = GetCentUnitsMap(currency);

        string levaPart = ConvertNumberToWordsInBulgarian(leva, PriceConvertScaleEnum.Leva, unitsMap, _teensMap, _tensMap, _hundredsMap, centUnitsMap);

        string levaCurrencyWord = GetCurrencyNameString(currency, PriceConvertScaleEnum.Leva, leva);

        if (stotinki == 0)
        {
            return $"{levaPart} {levaCurrencyWord}";
        }

        string stotinkiCurrencyWord = GetCurrencyNameString(currency, PriceConvertScaleEnum.Stotinki, stotinki);

        string stotinkiPart = ConvertNumberToWordsInBulgarian(stotinki, PriceConvertScaleEnum.Stotinki, _levaUnitsMap, _teensMap, _tensMap, _hundredsMap, centUnitsMap);

        return $"{levaPart} {levaCurrencyWord} и {stotinkiPart} {stotinkiCurrencyWord}";
    }

    private static string[] GetCentUnitsMap(Currency currency)
    {
        return currency switch
        {
            Currency.EUR => _euroAndUsdCentUnitsMap,
            Currency.USD => _euroAndUsdCentUnitsMap,
            Currency.BGN => _stotinkiUnitsMap,
            _ => throw new NotImplementedException($"Currency is not supported")
        };
    }

    private static string[] GetUnitsMap(Currency currency)
    {
        return currency switch
        {
            Currency.EUR => _euroUnitsMap,
            Currency.USD => _usdUnitsMap,
            Currency.BGN => _levaUnitsMap,
            _ => throw new NotImplementedException($"Currency is not supported")
        };
    }

    private static string ConvertNumberToWordsInBulgarian(
        long number,
        PriceConvertScaleEnum priceConvertScale,
        string[] unitsMap,
        string[] teensMap,
        string[] tensMap,
        string[] hundredsMap,
        string[] centUnitsMap)
    {
        if (number == 0) return "нула";
        if (number < 0) return "минус " + ConvertNumberToWordsInBulgarian(Math.Abs(number), priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap, centUnitsMap);

        string words = "";

        long billionsInNumber = number / 1_000_000_000;

        if (billionsInNumber > 0)
        {
            if (billionsInNumber == 1)
            {
                words += " един милиард ";
            }
            else
            {
                words += ConvertNumberToWordsInBulgarian(number / 1_000_000_000, priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap, centUnitsMap) + " милиарда ";
            }

            number %= 1_000_000_000;
        }

        long millionsInNumber = number / 1000000;

        if (millionsInNumber > 0)
        {
            if (millionsInNumber == 1)
            {
                words += " един милион ";
            }
            else
            {
                words += ConvertNumberToWordsInBulgarian(number / 1000000, priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap, centUnitsMap) + " милиона ";
            }

            number %= 1000000;
        }

        long thousandsInNumber = number / 1000;

        if (thousandsInNumber > 0)
        {
            if (thousandsInNumber == 1)
            {
                words += " хиляда ";
            }
            else
            {
                words += ConvertNumberToWordsInBulgarian(number / 1000, priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap, centUnitsMap) + " хиляди ";
            }

            number %= 1000;
        }
        if (number / 100 > 0)
        {
            words += hundredsMap[number / 100] + " ";

            number %= 100;
        }
        if (number > 0)
        {
            if (number < 10)
            {
                string wordForTenInNumber = unitsMap[number];

                if (priceConvertScale == PriceConvertScaleEnum.Stotinki)
                {
                    wordForTenInNumber = centUnitsMap[number];
                }

                words += (words == "" ? "" : "и ") + wordForTenInNumber;
            }
            else if (number < 20)
            {
                words += (words == "" ? "" : "и ") + teensMap[number - 10];
            }
            else
            {
                words += tensMap[number / 10];

                long tensInNumber = (number % 10);

                if (tensInNumber > 0)
                {
                    string wordForTenInNumber = unitsMap[tensInNumber];

                    if (priceConvertScale == PriceConvertScaleEnum.Stotinki)
                    {
                        wordForTenInNumber = centUnitsMap[tensInNumber];
                    }

                    words += " и " + wordForTenInNumber;
                }
            }
        }

        return words.Trim();
    }
}
