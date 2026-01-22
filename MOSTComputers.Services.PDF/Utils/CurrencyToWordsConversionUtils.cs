using MOSTComputers.Models.Product.Models;

namespace MOSTComputers.Services.PDF.Utils;

internal static class CurrencyToWordsConversionUtils
{
    internal enum PriceConvertScaleEnum
    {
        Subunit = 0,
        Unit = 1,
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
            Currency.USD => "$",
            Currency.BGN => "лв",
            _ => throw new NotImplementedException("Currency is not supported"),
        };
    }

    public static string GetCurrencyNameString(Currency currency, PriceConvertScaleEnum priceConvertScaleEnum, long number)
    {
        return (priceConvertScaleEnum, currency) switch
        {
            (PriceConvertScaleEnum.Unit, Currency.EUR) => "евро",
            (PriceConvertScaleEnum.Subunit, Currency.EUR) => number == 1 ? "евро цент" : "евро цента",

            (PriceConvertScaleEnum.Unit, Currency.BGN) => number == 1 ? "лев" : "лева",
            (PriceConvertScaleEnum.Subunit, Currency.BGN) => number == 1 ? "стотинка" : "стотинки",

            (PriceConvertScaleEnum.Unit, Currency.USD) => number == 1 ? "долар" : "долара",
            (PriceConvertScaleEnum.Subunit, Currency.USD) => number == 1 ? "цент" : "цента",

            _ => throw new NotImplementedException("Currency is not supported"),
        };
    }

    internal static string ConvertNumberToWordsInBulgarian(decimal amount, Currency currency)
    {
        long unit = (long)amount;
        int subunit = (int)Math.Round((amount - unit) * 100, 2, MidpointRounding.AwayFromZero);

        if (subunit < 0)
        {
            subunit = -subunit;
        }    

        string[] unitsMap = GetUnitsMap(currency);
        string[] subunitsMap = GetSubunitsMap(currency);

        string unitPart = ConvertNumberToWordsInBulgarianInternal(unit, PriceConvertScaleEnum.Unit, unitsMap, _teensMap, _tensMap, _hundredsMap, subunitsMap);

        string unitCurrencyWord = GetCurrencyNameString(currency, PriceConvertScaleEnum.Unit, unit);

        if (subunit == 0)
        {
            return $"{unitPart} {unitCurrencyWord}";
        }

        string subunitCurrencyWord = GetCurrencyNameString(currency, PriceConvertScaleEnum.Subunit, subunit);

        string subunitPart = ConvertNumberToWordsInBulgarianInternal(subunit, PriceConvertScaleEnum.Subunit, unitsMap, _teensMap, _tensMap, _hundredsMap, subunitsMap);

        return $"{unitPart} {unitCurrencyWord} и {subunitPart} {subunitCurrencyWord}";
    }

    private static string[] GetSubunitsMap(Currency currency)
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

    private static string ConvertNumberToWordsInBulgarianInternal(
        long number,
        PriceConvertScaleEnum priceConvertScale,
        string[] unitsMap,
        string[] teensMap,
        string[] tensMap,
        string[] hundredsMap,
        string[] centUnitsMap)
    {
        if (number == 0) return "нула";
        if (number < 0) return "минус " + ConvertNumberToWordsInBulgarianInternal(Math.Abs(number), priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap, centUnitsMap);

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
                words += ConvertNumberToWordsInBulgarianInternal(number / 1_000_000_000, priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap, centUnitsMap) + " милиарда ";
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
                words += ConvertNumberToWordsInBulgarianInternal(number / 1000000, priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap, centUnitsMap) + " милиона ";
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
                words += ConvertNumberToWordsInBulgarianInternal(number / 1000, priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap, centUnitsMap) + " хиляди ";
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

                if (priceConvertScale == PriceConvertScaleEnum.Subunit)
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

                    if (priceConvertScale == PriceConvertScaleEnum.Subunit)
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
