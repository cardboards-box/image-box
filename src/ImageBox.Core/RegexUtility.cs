namespace TradingCardMaker.Core;

/// <summary>
/// A utility class for working with regular expressions
/// </summary>
public static partial class RegexUtility
{
    /// <summary>
    /// Strips all non-alphanumeric characters from a string (excluding percentages, periods, and hyphens)
    /// </summary>
    /// <param name="input">The string to strip</param>
    /// <returns>The stripped string</returns>
    public static string UnitFilter(string input)
    {
        return UnitFilterRegex().Replace(input, "");
    }

    /// <summary>
    /// Gets the value and unit of a unit
    /// </summary>
    /// <param name="input">The given unit</param>
    /// <returns>The parsed value and unit</returns>
    public static (double value, string unit) UnitParse(string input)
    {
        var regex = UnitParserRegex();
        var match = regex.Match(input);
        if (!match.Success) throw UnitParserException.RegexFailed();
        if (match.Groups.Count <= 1) throw UnitParserException.NoMatches();

        var strValue = match.Groups[1].Value;
        if (string.IsNullOrEmpty(strValue)) throw UnitParserException.NoValue();

        if (!double.TryParse(strValue, out var value)) throw UnitParserException.InvalidValue();

        if (match.Groups.Count == 1) return (value, string.Empty);

        var unit = match.Groups[2].Value;
        return (value, unit);
    }

    [GeneratedRegex(@"[^a-zA-Z0-9\.%-]")]
    private static partial Regex UnitFilterRegex();

    [GeneratedRegex(@"(-?[0-9]{0,}\.?[0-9]{0,}?)([a-z%]{1,})?$")]
    private static partial Regex UnitParserRegex();
}

/// <summary>
/// Exception thrown when a unit parser fails
/// </summary>
/// <param name="message">The error that occurred</param>
public class UnitParserException(string message) : Exception(message) 
{
    internal static UnitParserException RegexFailed() => new("Input string is not a valid unit (Failed validation)");

    internal static UnitParserException NoMatches() => new("Input string is not a valid unit (Failed validation - no matches)");

    internal static UnitParserException NoValue() => new("Input string is not a valid unit (No value)");

    internal static UnitParserException InvalidValue() => new("Input string is not a valid unit (Unit value was not a valid number)");
}