using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

/// <summary>
/// Defines the contract for unit conversion operations.
/// </summary>
public interface IConversionService
{
    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    /// <param name="request">The conversion request containing value, source unit, and target unit.</param>
    /// <returns>A <see cref="ConversionResponse"/> containing the converted result.</returns>
    /// <exception cref="Exceptions.UnitNotFoundException">
    /// Thrown if either the source or target unit is not recognised.
    /// </exception>
    /// <exception cref="Exceptions.IncompatibleUnitsException">
    /// Thrown if the source and target units belong to different measurement categories.
    /// </exception>
    ConversionResponse Convert(ConversionRequest request);

    /// <summary>
    /// Returns all supported units, optionally filtered by category.
    /// </summary>
    /// <param name="category">Optional category filter (e.g., "temperature").</param>
    IEnumerable<UnitInfo> GetSupportedUnits(string? category = null);

    /// <summary>Returns all distinct measurement category names.</summary>
    IEnumerable<string> GetCategories();
}
