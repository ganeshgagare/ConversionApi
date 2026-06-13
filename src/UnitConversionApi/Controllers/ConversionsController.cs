using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

/// <summary>
/// Provides endpoints for unit-of-measurement conversion.
/// </summary>
[ApiController]
[Route("api/conversions")]
[Produces("application/json")]
public sealed class ConversionsController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public ConversionsController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    /// <summary>
    /// Converts a numerical value from one unit to another.
    /// </summary>
    /// <param name="request">The conversion request.</param>
    /// <returns>The converted value along with metadata about the conversion.</returns>
    /// <response code="200">Conversion was successful.</response>
    /// <response code="400">The request is invalid (unknown unit, incompatible categories, etc.).</response>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<ConversionResponse> Convert([FromBody] ConversionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = _conversionService.Convert(request);
        return Ok(result);
    }

    /// <summary>
    /// Returns all supported units, optionally filtered by category.
    /// </summary>
    /// <param name="category">Optional category to filter by (e.g., "temperature", "length", "weight").</param>
    /// <returns>A list of all supported units grouped by measurement category.</returns>
    /// <response code="200">Returns the list of units.</response>
    [HttpGet("units")]
    [ProducesResponseType(typeof(IEnumerable<UnitInfo>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<UnitInfo>> GetUnits([FromQuery] string? category = null)
    {
        var units = _conversionService.GetSupportedUnits(category);
        return Ok(units);
    }

    /// <summary>
    /// Returns all available measurement categories.
    /// </summary>
    /// <returns>A list of category names (e.g., "length", "temperature", "weight").</returns>
    /// <response code="200">Returns the list of categories.</response>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<string>> GetCategories()
    {
        var categories = _conversionService.GetCategories();
        return Ok(categories);
    }
}
