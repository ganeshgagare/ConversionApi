namespace UnitConversionApi.Models;

/// <summary>
/// Represents the result of a unit conversion operation.
/// </summary>
public sealed class ConversionResponse
{
    /// <summary>The original value supplied by the caller.</summary>
    public double Value { get; init; }

    /// <summary>The source unit.</summary>
    public string FromUnit { get; init; } = string.Empty;

    /// <summary>The target unit.</summary>
    public string ToUnit { get; init; } = string.Empty;

    /// <summary>The converted result.</summary>
    public double Result { get; init; }

    /// <summary>The measurement category (e.g., "temperature", "length").</summary>
    public string Category { get; init; } = string.Empty;
}
