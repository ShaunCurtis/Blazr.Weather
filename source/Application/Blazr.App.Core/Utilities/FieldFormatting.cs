/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Globalization;
using System.Text.RegularExpressions;

namespace Blazr.App.Core;

public static class FieldFormatting
{
    public static string AsDateFormat(DateTime Date)
    {
        return Date.ToString("dd-MMM-yyyy");
    }

    public static string AsPercentage(decimal value)
    {
        return string.Format("{0}%", value.ToString("#0.##"));
    }

    public static string AsMoney(decimal value)
    {
        return string.Format("£{0}", value.ToString("#0.00"));
    }

    public static string AsWeight(decimal value)
    {
        return value.ToString("#0.000", CultureInfo.CurrentCulture);
    }

    public static string AsYesNo(bool value)
    {
        return value ? "Yes" : "No";
    }

    public static string AsSizedString(string value, bool dotting, int size = 50)
    {
        if (value != null)
        {
            if (value.Length > size - 3 && dotting) return string.Concat(value.Substring(0, size - 3), "...");
            else if (value.Length > size) return value.Substring(0, size);
            else return value;
        }
        return string.Empty;
    }

    public static string AsSeparatedString(string value)
    {
        return Regex.Replace(value, @"\B[A-Z]", " $0");
    }

    public static string TextToHtmlNewLines(string text)
    {

        var t = text.Replace(System.Environment.NewLine, "<br />");
        return t;
    }
}
