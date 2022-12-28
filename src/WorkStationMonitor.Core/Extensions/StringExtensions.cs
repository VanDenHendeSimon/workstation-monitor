namespace WorkStationMonitor.Core.Extensions;

public static class StringExtensions
{
    public static string Capitalize(this string str) => str.Length switch
    {
        0 => string.Empty,
        1 => str.ToUpper(),
        _ => char.ToUpper(str[0]) + str[1..].ToLower(),
    };
}
