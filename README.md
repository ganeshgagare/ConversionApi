# Unit Conversion API

A RESTful ASP.NET Core Web API for converting numerical values between different units of measurement. Supports **length, temperature, weight/mass, volume, speed, area, and pressure** — with a clean architecture that makes adding new units or categories straightforward.

---

## Table of Contents

- [Getting Started](#getting-started)
- [API Reference](#api-reference)
- [Supported Units](#supported-units)
- [Project Structure](#project-structure)
- [Design Decisions & Trade-offs](#design-decisions--trade-offs)
- [Running Tests](#running-tests)

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Run locally

```bash
# 1. Clone the repository
git clone <repository-url>
cd UnitConversionApi

# 2. Restore dependencies
dotnet restore

# 3. Run the API
dotnet run --project src/UnitConversionApi

# The API will start on:
#   http://localhost:5000
#   https://localhost:5001
#
# Swagger UI is available at the root:
#   http://localhost:5000
```

> **Tip:** In development mode, Swagger UI is served at the root URL (`/`). You can explore and test all endpoints interactively without any additional tooling.

---

## API Reference

### `POST /api/conversions/convert`

Converts a value from one unit to another.

**Request body:**

```json
{
  "value": 100,
  "fromUnit": "celsius",
  "toUnit": "fahrenheit"
}
```

**Response:**

```json
{
  "value": 100,
  "fromUnit": "celsius",
  "toUnit": "fahrenheit",
  "result": 212.0,
  "category": "temperature"
}
```

**Error response (RFC 7807 Problem Details):**

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Unit Not Found",
  "status": 400,
  "detail": "Unit 'blorp' is not supported. Use GET /api/conversions/units to list all available units.",
  "instance": "/api/conversions/convert"
}
```

---

### `GET /api/conversions/units`

Returns all supported units, optionally filtered by category.

**Query parameters:**

| Parameter  | Type   | Description                                 |
|-----------|--------|---------------------------------------------|
| `category` | string | Optional. Filter by category (e.g., `temperature`). |

**Example:**

```
GET /api/conversions/units?category=temperature
```

**Response:**

```json
[
  { "key": "celsius",    "displayName": "Celsius (°C)",    "category": "temperature" },
  { "key": "fahrenheit", "displayName": "Fahrenheit (°F)", "category": "temperature" },
  { "key": "kelvin",     "displayName": "Kelvin (K)",      "category": "temperature" }
]
```

---

### `GET /api/conversions/categories`

Returns all available measurement categories.

**Response:**

```json
["area", "length", "pressure", "speed", "temperature", "volume", "weight"]
```

---

## Supported Units

| Category    | Unit Keys                                                                                       |
|------------|------------------------------------------------------------------------------------------------|
| Length      | `meter`, `kilometer`, `centimeter`, `millimeter`, `inch`, `foot`, `yard`, `mile`, `nautical_mile` |
| Temperature | `celsius`, `fahrenheit`, `kelvin`                                                              |
| Weight/Mass | `kilogram`, `gram`, `milligram`, `pound`, `ounce`, `metric_ton`, `short_ton`, `stone`         |
| Volume      | `liter`, `milliliter`, `cubic_meter`, `cubic_centimeter`, `us_gallon`, `uk_gallon`, `us_fluid_ounce`, `us_cup`, `us_pint`, `us_quart` |
| Speed       | `meter_per_second`, `kilometer_per_hour`, `mile_per_hour`, `knot`, `foot_per_second`           |
| Area        | `square_meter`, `square_kilometer`, `square_centimeter`, `square_inch`, `square_foot`, `square_yard`, `acre`, `hectare`, `square_mile` |
| Pressure    | `pascal`, `kilopascal`, `megapascal`, `bar`, `millibar`, `atmosphere`, `psi`, `torr`          |

> Unit keys are **case-insensitive** in all API calls.

---

## Project Structure

```
UnitConversionApi/
├── src/
│   └── UnitConversionApi/
│       ├── Controllers/          # HTTP API layer
│       ├── Data/                 # UnitRegistry — unit definitions & conversion factors
│       ├── Exceptions/           # Domain-specific exception types
│       ├── Middleware/           # Global exception handling (RFC 7807 Problem Details)
│       ├── Models/               # DTOs (request / response / unit info)
│       ├── Services/             # Business logic (IConversionService / ConversionService)
│       ├── Program.cs
│       └── appsettings.json
├── tests/
│   └── UnitConversionApi.Tests/  # xUnit unit tests
├── .editorconfig                 # Consistent code style for the team
├── .gitignore
├── README.md
└── UnitConversionApi.sln
```

---

## Design Decisions & Trade-offs

### Two-step base-unit conversion algorithm

All units in a category are defined by their relationship to a single **base unit** (e.g., meters for length, Celsius for temperature). Any conversion is performed in two steps:

1. Convert the input value to the base unit.
2. Convert from the base unit to the target unit.

**Why?** This is O(1) per conversion and O(N) registry entries regardless of how many units exist. The alternative — a full N×N lookup table of direct conversion factors — would be impractical at scale.

### Temperature offsets

Temperature cannot use simple multiplication alone because Celsius, Fahrenheit, and Kelvin have different zero points. Each unit carries a `PreOffset` value that is applied before multiplication:

```
celsius = (input + preOffset) * factor
```

This keeps the same algorithm for all units while correctly handling non-linear temperature scales.

### Hardcoded unit registry (current version)

Unit data is stored in `UnitRegistry.cs` as compile-time constants. This is intentional for this version — it keeps the solution dependency-free and trivially runnable. The registry is designed so that migrating to a database or configuration-file source in the future requires only changing `UnitRegistry.cs` without touching the service or controller layer.

### RFC 7807 Problem Details error responses

All errors are returned as [RFC 7807 Problem Details](https://datatracker.ietf.org/doc/html/rfc7807) JSON, which is the standard format used by modern APIs and natively consumed by many HTTP clients.

### Extensibility

To add a new unit: add a single `UnitDefinition` entry to `UnitRegistry.cs`.  
To add a new category: add entries with a new `Category` value — no other code changes required.

---

## Running Tests

```bash
dotnet test
```

Tests cover:
- All major conversion categories (length, temperature, weight, volume, speed)
- Known exact values (e.g., 0 °C = 32 °F, 1 inch = 2.54 cm)
- Error cases (unknown unit, incompatible categories, null input)
- Case-insensitive unit key handling
- Unit listing and category filtering
