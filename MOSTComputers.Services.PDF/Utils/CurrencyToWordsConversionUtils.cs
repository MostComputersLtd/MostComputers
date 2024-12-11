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
        int stotinki = (int)((amount - leva) * 100);

        string levaPart = ConvertNumberToWordsInLeva(leva, _unitsMap, _teensMap, _tensMap, _hundredsMap);
        string stotinkiPart = ConvertNumberToWordsInLeva(stotinki, _unitsMap, _teensMap, _tensMap, _hundredsMap);

        return $"{levaPart} лева и {stotinkiPart} стотинки";
    }

    private static string ConvertNumberToWordsInLeva(int number, string[] unitsMap, string[] teensMap, string[] tensMap, string[] hundredsMap)
    {
        if (number == 0) return "нула";
        if (number < 0) return "минус " + ConvertNumberToWordsInLeva(Math.Abs(number), unitsMap, teensMap, tensMap, hundredsMap);

        string words = "";

        if (number / 1000 > 0)
        {
            words += ConvertNumberToWordsInLeva(number / 1000, unitsMap, teensMap, tensMap, hundredsMap) + " хиляди ";

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
                words += unitsMap[number];
            }
            else if (number < 20)
            {
                words += teensMap[number - 10];
            }
            else
            {
                words += tensMap[number / 10];
                
                if ((number % 10) > 0)
                {
                    words += " и " + unitsMap[number % 10];
                }
            }
        }

        return words.Trim();
    }
}
