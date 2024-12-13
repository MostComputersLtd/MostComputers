using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.PDF.Utils;
internal static class CurrencyToWordsConversionUtils
{
    private readonly static string[] _unitsMap = {
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

    internal static string ConvertPriceToWordsInLeva(double amount)
    {
        int leva = (int)amount;
        int stotinki = (int)Math.Round((amount - leva) * 100, 2, MidpointRounding.AwayFromZero);

        string levaPart = ConvertNumberToWordsInLeva(leva, PriceConvertScaleEnum.Leva, _unitsMap, _teensMap, _tensMap, _hundredsMap);

        if (stotinki == 0)
        {
            return $"{levaPart} лева";
        }

        string stotinkiPart = ConvertNumberToWordsInLeva(stotinki, PriceConvertScaleEnum.Stotinki, _unitsMap, _teensMap, _tensMap, _hundredsMap);

        return $"{levaPart} лева и {stotinkiPart} стотинки";
    }

    private static string ConvertNumberToWordsInLeva(int number, PriceConvertScaleEnum priceConvertScale, string[] unitsMap, string[] teensMap, string[] tensMap, string[] hundredsMap)
    {
        if (number == 0) return "нула";
        if (number < 0) return "минус " + ConvertNumberToWordsInLeva(Math.Abs(number), priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap);

        string words = "";

        int billionsInNumber = number / 1_000_000_000;

        if (billionsInNumber > 0)
        {
            if (billionsInNumber == 1)
            {
                words += " един милиард ";
            }
            else
            {
                words += ConvertNumberToWordsInLeva(number / 1_000_000_000, priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap) + " милиарда ";
            }

            number %= 1_000_000_000;
        }

        int millionsInNumber = number / 1000000;

        if (millionsInNumber > 0)
        {
            if (millionsInNumber == 1)
            {
                words += " един милион ";
            }
            else
            {
                words += ConvertNumberToWordsInLeva(number / 1000000, priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap) + " милиона ";
            }

            number %= 1000000;
        }

        int thousandsInNumber = number / 1000;

        if (thousandsInNumber > 0)
        {
            if (thousandsInNumber == 1)
            {
                words += " хиляда ";
            }
            else
            {
                words += ConvertNumberToWordsInLeva(number / 1000, priceConvertScale, unitsMap, teensMap, tensMap, hundredsMap) + " хиляди ";
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

                if (priceConvertScale == PriceConvertScaleEnum.Stotinki
                   && number < _stotinkiUnitsMap.Length)
                {
                    wordForTenInNumber = _stotinkiUnitsMap[number];
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

                int tensInNumber = (number % 10);

                if (tensInNumber > 0)
                {
                    string wordForTenInNumber = unitsMap[tensInNumber];

                    if (priceConvertScale == PriceConvertScaleEnum.Stotinki
                       && tensInNumber < _stotinkiUnitsMap.Length)
                    {
                        wordForTenInNumber = _stotinkiUnitsMap[tensInNumber];
                    }

                    words += " и " + wordForTenInNumber;
                }
            }
        }

        return words.Trim();
    }
}

internal enum PriceConvertScaleEnum
{
    Stotinki = 0,
    Leva = 1,
}