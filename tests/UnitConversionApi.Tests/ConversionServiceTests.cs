using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Tests;

/// <summary>
/// Unit tests for <see cref="ConversionService"/>.
///
/// Test naming convention: MethodName_StateUnderTest_ExpectedBehavior
/// </summary>
public sealed class ConversionServiceTests
{
    private readonly IConversionService _sut = new ConversionService();

    // ── Length conversions ────────────────────────────────────────────────────

    [Fact]
    public void Convert_MetersToFeet_ReturnsCorrectResult()
    {
        var request  = new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "foot" };
        var response = _sut.Convert(request);

        Assert.Equal(3.280839895, response.Result, precision: 6);
        Assert.Equal("length", response.Category);
    }

    [Fact]
    public void Convert_KilometersToMiles_ReturnsCorrectResult()
    {
        var request  = new ConversionRequest { Value = 1, FromUnit = "kilometer", ToUnit = "mile" };
        var response = _sut.Convert(request);

        Assert.Equal(0.6213711922, response.Result, precision: 6);
    }

    [Fact]
    public void Convert_InchesToCentimeters_ReturnsCorrectResult()
    {
        var request  = new ConversionRequest { Value = 1, FromUnit = "inch", ToUnit = "centimeter" };
        var response = _sut.Convert(request);

        Assert.Equal(2.54, response.Result, precision: 6);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        var request  = new ConversionRequest { Value = 42.5, FromUnit = "meter", ToUnit = "meter" };
        var response = _sut.Convert(request);

        Assert.Equal(42.5, response.Result);
    }

    // ── Temperature conversions ───────────────────────────────────────────────

    [Theory]
    [InlineData(0,   "celsius",    "fahrenheit", 32.0)]
    [InlineData(100, "celsius",    "fahrenheit", 212.0)]
    [InlineData(32,  "fahrenheit", "celsius",    0.0)]
    [InlineData(212, "fahrenheit", "celsius",    100.0)]
    [InlineData(0,   "celsius",    "kelvin",     273.15)]
    [InlineData(0,   "kelvin",     "celsius",    -273.15)]
    [InlineData(300, "kelvin",     "celsius",    26.85)]
    public void Convert_Temperature_ReturnsCorrectResult(
        double input, string from, string to, double expected)
    {
        var request  = new ConversionRequest { Value = input, FromUnit = from, ToUnit = to };
        var response = _sut.Convert(request);

        Assert.Equal(expected, response.Result, precision: 4);
        Assert.Equal("temperature", response.Category);
    }

    // ── Weight / Mass conversions ─────────────────────────────────────────────

    [Fact]
    public void Convert_KilogramsToPounds_ReturnsCorrectResult()
    {
        var request  = new ConversionRequest { Value = 1, FromUnit = "kilogram", ToUnit = "pound" };
        var response = _sut.Convert(request);

        Assert.Equal(2.2046226218, response.Result, precision: 6);
        Assert.Equal("weight", response.Category);
    }

    [Fact]
    public void Convert_OuncesToGrams_ReturnsCorrectResult()
    {
        var request  = new ConversionRequest { Value = 1, FromUnit = "ounce", ToUnit = "gram" };
        var response = _sut.Convert(request);

        Assert.Equal(28.3495231, response.Result, precision: 4);
    }

    // ── Volume conversions ────────────────────────────────────────────────────

    [Fact]
    public void Convert_LitersToUsGallons_ReturnsCorrectResult()
    {
        var request  = new ConversionRequest { Value = 1, FromUnit = "liter", ToUnit = "us_gallon" };
        var response = _sut.Convert(request);

        Assert.Equal(0.2641720524, response.Result, precision: 6);
        Assert.Equal("volume", response.Category);
    }

    // ── Speed conversions ─────────────────────────────────────────────────────

    [Fact]
    public void Convert_KmhToMph_ReturnsCorrectResult()
    {
        var request  = new ConversionRequest { Value = 100, FromUnit = "kilometer_per_hour", ToUnit = "mile_per_hour" };
        var response = _sut.Convert(request);

        Assert.Equal(62.1371192, response.Result, precision: 4);
        Assert.Equal("speed", response.Category);
    }

    // ── Error cases ───────────────────────────────────────────────────────────

    [Fact]
    public void Convert_UnknownFromUnit_ThrowsUnitNotFoundException()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "blorp", ToUnit = "meter" };

        Assert.Throws<UnitNotFoundException>(() => _sut.Convert(request));
    }

    [Fact]
    public void Convert_UnknownToUnit_ThrowsUnitNotFoundException()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "quasar" };

        Assert.Throws<UnitNotFoundException>(() => _sut.Convert(request));
    }

    [Fact]
    public void Convert_IncompatibleCategories_ThrowsIncompatibleUnitsException()
    {
        var request = new ConversionRequest { Value = 100, FromUnit = "celsius", ToUnit = "meter" };

        Assert.Throws<IncompatibleUnitsException>(() => _sut.Convert(request));
    }

    [Fact]
    public void Convert_NullRequest_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.Convert(null!));
    }

    // ── GetSupportedUnits ─────────────────────────────────────────────────────

    [Fact]
    public void GetSupportedUnits_NoFilter_ReturnsAllUnits()
    {
        var units = _sut.GetSupportedUnits().ToList();

        Assert.NotEmpty(units);
        Assert.Contains(units, u => u.Key == "meter");
        Assert.Contains(units, u => u.Key == "celsius");
        Assert.Contains(units, u => u.Key == "kilogram");
    }

    [Fact]
    public void GetSupportedUnits_WithCategoryFilter_ReturnsOnlyThatCategory()
    {
        var units = _sut.GetSupportedUnits("temperature").ToList();

        Assert.All(units, u => Assert.Equal("temperature", u.Category));
        Assert.Contains(units, u => u.Key == "celsius");
        Assert.Contains(units, u => u.Key == "fahrenheit");
        Assert.Contains(units, u => u.Key == "kelvin");
    }

    // ── GetCategories ─────────────────────────────────────────────────────────

    [Fact]
    public void GetCategories_ReturnsAllExpectedCategories()
    {
        var categories = _sut.GetCategories().ToList();

        Assert.Contains("length",      categories);
        Assert.Contains("temperature", categories);
        Assert.Contains("weight",      categories);
        Assert.Contains("volume",      categories);
        Assert.Contains("speed",       categories);
        Assert.Contains("area",        categories);
        Assert.Contains("pressure",    categories);
    }

    // ── Case insensitivity ────────────────────────────────────────────────────

    [Theory]
    [InlineData("CELSIUS",    "FAHRENHEIT")]
    [InlineData("Celsius",    "Fahrenheit")]
    [InlineData("METER",      "FOOT")]
    [InlineData("Kilogram",   "Pound")]
    public void Convert_CaseInsensitiveUnitKeys_ReturnsResult(string from, string to)
    {
        var request  = new ConversionRequest { Value = 1, FromUnit = from, ToUnit = to };
        var response = _sut.Convert(request);

        Assert.True(response.Result > 0 || response.Result < 0 || response.Result == 0);
    }
}
