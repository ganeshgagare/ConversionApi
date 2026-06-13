using UnitConversionApi.Models;

namespace UnitConversionApi.Data;

/// <summary>
/// Represents a unit definition within a linear conversion category.
/// For linear units, the value is first converted to the base unit (factor * value + offset),
/// then from the base unit to the target (inverse).
///
/// Formula (to base): baseValue = (inputValue + preOffset) * factor
/// Formula (from base): result = (baseValue / factor) - preOffset
///
/// For most units: preOffset = 0, and factor is simply the ratio to the base.
/// Temperature uses offsets to handle non-linear zero points.
/// </summary>
internal sealed record UnitDefinition(
    string Key,
    string DisplayName,
    string Category,
    double Factor,       // Multiply by this to get the base unit value
    double PreOffset = 0 // Add this BEFORE multiplying (used for temperature offsets)
);

/// <summary>
/// Statically-defined registry of all supported units of measurement.
///
/// Design notes:
/// - Each category has a designated "base unit" to which all others are converted first.
/// - New categories or units can be added by extending the _units collection.
/// - In a future version, this data could be sourced from a database or configuration file
///   to support dynamic unit management at scale.
/// </summary>
public static class UnitRegistry
{
    // ──────────────────────────────────────────────────────────────────────────────
    // BASE UNITS PER CATEGORY
    //   Length      → meter
    //   Temperature → celsius  (special: offset-based conversion)
    //   Weight/Mass → kilogram
    //   Volume      → liter
    //   Speed       → meter_per_second
    //   Area        → square_meter
    //   Pressure    → pascal
    // ──────────────────────────────────────────────────────────────────────────────

    private static readonly IReadOnlyList<UnitDefinition> Units = new List<UnitDefinition>
    {
        // ── Length (base: meter) ──────────────────────────────────────────────────
        new("meter",       "Meter (m)",          "length", 1.0),
        new("kilometer",   "Kilometer (km)",     "length", 1000.0),
        new("centimeter",  "Centimeter (cm)",    "length", 0.01),
        new("millimeter",  "Millimeter (mm)",    "length", 0.001),
        new("inch",        "Inch (in)",          "length", 0.0254),
        new("foot",        "Foot (ft)",          "length", 0.3048),
        new("yard",        "Yard (yd)",          "length", 0.9144),
        new("mile",        "Mile (mi)",          "length", 1609.344),
        new("nautical_mile","Nautical Mile (nmi)","length",1852.0),

        // ── Temperature (base: celsius) ───────────────────────────────────────────
        // Formula to Celsius:  celsius = (input + preOffset) * factor
        // celsius → celsius:   (x + 0) * 1         = x
        // fahrenheit → celsius:(x - 32) * (5/9)    = (x + -32) * 0.5556
        // kelvin → celsius:    (x - 273.15) * 1    = (x + -273.15) * 1
        new("celsius",    "Celsius (°C)",       "temperature", 1.0,      0.0),
        new("fahrenheit", "Fahrenheit (°F)",    "temperature", 5.0/9.0, -32.0),
        new("kelvin",     "Kelvin (K)",         "temperature", 1.0,     -273.15),

        // ── Weight / Mass (base: kilogram) ────────────────────────────────────────
        new("kilogram",   "Kilogram (kg)",      "weight", 1.0),
        new("gram",       "Gram (g)",           "weight", 0.001),
        new("milligram",  "Milligram (mg)",     "weight", 0.000001),
        new("pound",      "Pound (lb)",         "weight", 0.45359237),
        new("ounce",      "Ounce (oz)",         "weight", 0.028349523125),
        new("metric_ton", "Metric Ton (t)",     "weight", 1000.0),
        new("short_ton",  "Short Ton (US ton)", "weight", 907.18474),
        new("stone",      "Stone (st)",         "weight", 6.35029318),

        // ── Volume (base: liter) ─────────────────────────────────────────────────
        new("liter",          "Liter (L)",           "volume", 1.0),
        new("milliliter",     "Milliliter (mL)",      "volume", 0.001),
        new("cubic_meter",    "Cubic Meter (m³)",     "volume", 1000.0),
        new("cubic_centimeter","Cubic Centimeter (cm³)","volume", 0.001),
        new("us_gallon",      "US Gallon (gal)",      "volume", 3.785411784),
        new("uk_gallon",      "UK Gallon (gal)",      "volume", 4.54609),
        new("us_fluid_ounce", "US Fluid Ounce (fl oz)","volume", 0.0295735296875),
        new("us_cup",         "US Cup",               "volume", 0.2365882365),
        new("us_pint",        "US Pint (pt)",         "volume", 0.473176473),
        new("us_quart",       "US Quart (qt)",        "volume", 0.946352946),

        // ── Speed (base: meter_per_second) ───────────────────────────────────────
        new("meter_per_second",    "Meter per Second (m/s)",       "speed", 1.0),
        new("kilometer_per_hour",  "Kilometer per Hour (km/h)",    "speed", 1.0/3.6),
        new("mile_per_hour",       "Mile per Hour (mph)",          "speed", 0.44704),
        new("knot",                "Knot (kn)",                    "speed", 0.514444),
        new("foot_per_second",     "Foot per Second (ft/s)",       "speed", 0.3048),

        // ── Area (base: square_meter) ─────────────────────────────────────────────
        new("square_meter",     "Square Meter (m²)",     "area", 1.0),
        new("square_kilometer", "Square Kilometer (km²)","area", 1_000_000.0),
        new("square_centimeter","Square Centimeter (cm²)","area", 0.0001),
        new("square_inch",      "Square Inch (in²)",     "area", 0.00064516),
        new("square_foot",      "Square Foot (ft²)",     "area", 0.09290304),
        new("square_yard",      "Square Yard (yd²)",     "area", 0.83612736),
        new("acre",             "Acre (ac)",              "area", 4046.8564224),
        new("hectare",          "Hectare (ha)",           "area", 10_000.0),
        new("square_mile",      "Square Mile (mi²)",     "area", 2_589_988.110336),

        // ── Pressure (base: pascal) ───────────────────────────────────────────────
        new("pascal",       "Pascal (Pa)",            "pressure", 1.0),
        new("kilopascal",   "Kilopascal (kPa)",       "pressure", 1000.0),
        new("megapascal",   "Megapascal (MPa)",       "pressure", 1_000_000.0),
        new("bar",          "Bar",                    "pressure", 100_000.0),
        new("millibar",     "Millibar (mbar)",        "pressure", 100.0),
        new("atmosphere",   "Atmosphere (atm)",       "pressure", 101_325.0),
        new("psi",          "Pound per Square Inch (psi)", "pressure", 6894.757293168),
        new("torr",         "Torr (mmHg)",            "pressure", 133.3223684211),
    };

    // ── Lookup dictionary (case-insensitive) ──────────────────────────────────────

    private static readonly IReadOnlyDictionary<string, UnitDefinition> UnitsByKey =
        Units.ToDictionary(u => u.Key, u => u, StringComparer.OrdinalIgnoreCase);

    // ── Public API ────────────────────────────────────────────────────────────────

    /// <summary>Tries to find a unit definition by its key.</summary>
    internal static bool TryGetUnit(string key, out UnitDefinition? unit)
    {
        unit = null;
        if (string.IsNullOrWhiteSpace(key)) return false;
        return UnitsByKey.TryGetValue(key.Trim(), out unit);
    }

    /// <summary>Returns all unit definitions, optionally filtered by category.</summary>
    internal static IEnumerable<UnitDefinition> GetAll(string? category = null) =>
        string.IsNullOrWhiteSpace(category)
            ? Units
            : Units.Where(u => u.Category.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase));

    /// <summary>Returns all distinct category names.</summary>
    public static IEnumerable<string> GetCategories() =>
        Units.Select(u => u.Category).Distinct(StringComparer.OrdinalIgnoreCase);

    // Make the internal record accessible to the service layer.
    internal static UnitDefinition GetUnitOrThrow(string key)
    {
        if (!UnitsByKey.TryGetValue(key.Trim(), out var unit))
            throw new Exceptions.UnitNotFoundException(key);
        return unit;
    }
}
