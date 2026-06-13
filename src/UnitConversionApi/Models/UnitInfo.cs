namespace UnitConversionApi.Models;

/// <summary>
/// Describes a supported unit of measurement.
/// </summary>
public sealed class UnitInfo
{
    /// <summary>The canonical unit key used in API calls (e.g., "celsius").</summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>Human-readable display name (e.g., "Celsius (°C)").</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>The measurement category this unit belongs to.</summary>
    public string Category { get; init; } = string.Empty;
}
