using System.Globalization;

namespace x99AssessmentByTva.Application.Common.Helpers;

internal static class PeriodFormatter
{
    public static string FormatMonthYear(int month, int year)
        => $"{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month)} {year}";
}