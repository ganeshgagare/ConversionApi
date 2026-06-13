namespace UnitConversionApi.Exceptions;

/// <summary>
/// Thrown when a requested conversion cannot be performed.
/// </summary>
public sealed class ConversionException : Exception
{
    public ConversionException(string message) : base(message) { }

    public ConversionException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Thrown when a requested unit is not found in the registry.
/// </summary>
public sealed class UnitNotFoundException : Exception
{
    public string UnitKey { get; }

    public UnitNotFoundException(string unitKey)
        : base($"Unit '{unitKey}' is not supported. Use GET /api/conversions/units to list all available units.")
    {
        UnitKey = unitKey;
    }
}

/// <summary>
/// Thrown when two valid units belong to different categories and cannot be compared.
/// </summary>
public sealed class IncompatibleUnitsException : Exception
{
    public IncompatibleUnitsException(string fromUnit, string toUnit, string fromCategory, string toCategory)
        : base($"Cannot convert '{fromUnit}' ({fromCategory}) to '{toUnit}' ({toCategory}): units belong to different categories.") { }
}
