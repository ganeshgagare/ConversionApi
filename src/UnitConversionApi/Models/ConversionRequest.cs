namespace UnitConversionApi.Models;

/// <summary>
/// Represents a request to convert a value from one unit to another.
/// </summary>
public sealed class ConversionRequest
{
    /// <summary>
    /// The numeric value to convert.
    /// </summary>
    /// <example>100</example>
    public double Value { get; init; }

    /// <summary>
    /// The unit to convert from (e.g., "celsius", "meters", "kilograms").
    /// </summary>
    /// <example>celsius</example>
    public string FromUnit { get; init; } = string.Empty;

    /// <summary>
    /// The unit to convert to (e.g., "fahrenheit", "feet", "pounds").
    /// </summary>
    /// <example>fahrenheit</example>
    public string ToUnit { get; init; } = string.Empty;
}
