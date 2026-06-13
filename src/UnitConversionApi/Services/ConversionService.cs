using UnitConversionApi.Data;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

/// <summary>
/// Core implementation of <see cref="IConversionService"/>.
///
/// Conversion algorithm (two-step via base unit):
///   1. Convert the input value to the category's base unit:
///         baseValue = (inputValue + fromUnit.PreOffset) * fromUnit.Factor
///   2. Convert from the base unit to the target:
///         result = (baseValue / toUnit.Factor) - toUnit.PreOffset
///
/// This two-step approach means adding a new unit only ever requires
/// knowing its relationship to a single base unit, keeping the registry simple
/// and O(1) per conversion regardless of how many units are registered.
/// </summary>
public sealed class ConversionService : IConversionService
{
    private const int ResultDecimalPlaces = 10;

    /// <inheritdoc />
    public ConversionResponse Convert(ConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var fromUnit = UnitRegistry.GetUnitOrThrow(request.FromUnit);
        var toUnit   = UnitRegistry.GetUnitOrThrow(request.ToUnit);

        if (!fromUnit.Category.Equals(toUnit.Category, StringComparison.OrdinalIgnoreCase))
        {
            throw new IncompatibleUnitsException(
                request.FromUnit, request.ToUnit,
                fromUnit.Category, toUnit.Category);
        }

        double baseValue = (request.Value + fromUnit.PreOffset) * fromUnit.Factor;
        double result    = (baseValue / toUnit.Factor) - toUnit.PreOffset;

        // Round to avoid floating-point noise (e.g., 0.9999999999 instead of 1.0).
        result = Math.Round(result, ResultDecimalPlaces, MidpointRounding.AwayFromZero);

        return new ConversionResponse
        {
            Value    = request.Value,
            FromUnit = fromUnit.Key,
            ToUnit   = toUnit.Key,
            Result   = result,
            Category = fromUnit.Category,
        };
    }

    /// <inheritdoc />
    public IEnumerable<UnitInfo> GetSupportedUnits(string? category = null) =>
        UnitRegistry.GetAll(category)
            .Select(u => new UnitInfo
            {
                Key         = u.Key,
                DisplayName = u.DisplayName,
                Category    = u.Category,
            })
            .OrderBy(u => u.Category)
            .ThenBy(u => u.Key);

    /// <inheritdoc />
    public IEnumerable<string> GetCategories() =>
        UnitRegistry.GetCategories().OrderBy(c => c);
}
