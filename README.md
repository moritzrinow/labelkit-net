# LabelKit

LabelKit is a .NET toolkit for parsing [kubernetes-like label-selectors](https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/#label-selectors) and using them to build filters, including query-expressions for EFCore (PostgreSQL, MySql, SqlServer, Sqlite).
Labels are name/value pairs that can be represented as dictionaries (name->value) or simple string collections (name+delimiter+value).

- [Packages](#packages)
- [Examples](#examples)
  - [EFCore PostgreSQL](#efcore-postgresql)
  - [Label-Selectors](#label-selectors)
  - [Matching Labels Offline](#matching-labels-offline)
- [Expressions (EFCore)](#expressions-efcore)
  - [Supported EFCore Providers](#supported-efcore-providers)
  - [Provided Expression-Builders](#provided-expression-builders)
  - [Filter IQueryables](#filter-iqueryables)

## Packages

The majority of packages support netstandard2.0.

The following NuGet packages are provided:

- [LabelKit](https://www.nuget.org/packages/LabelKit/)
  - You need to reference structured label-selectors.
- [LabelKit.Parser](https://www.nuget.org/packages/LabelKit.Parser/)
  - You need to parse raw label-selectors.
- [LabelKit.Expressions](https://www.nuget.org/packages/LabelKit.Expressions/)
  - You need to build expressions and filter queries. See [here](#expressions-efcore).
- [LabelKit.EFCore.PostgreSQL](https://www.nuget.org/packages/LabelKit.EFCore.PostgreSQL/)
  - You need to filter EFCore-PostgreSQL queries. See [here](#expressions-efcore).
- [LabelKit.EFCore.Pomelo.MySql](https://www.nuget.org/packages/LabelKit.EFCore.Pomelo.MySql/)
  - You need to filter EFCore-MySql queries. See [here](#expressions-efcore).

## Examples

### EFCore PostgreSQL:

```csharp
public class Entity
{
  // Stored as JSONB
  public Dictionary<string, string> Labels { get; set; }
}
```

`dotnet add package LabelKit.Parser`

LabelKit.Parser offers a default parser built with [Pidgin](https://github.com/benjamin-hodgson/Pidgin).

The parser is able to parse raw label-selectors adhering to the [kubernetes syntax](https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/#label-selectors).

```csharp
using LabelKit;

var selector = LabelSelectorParser.Parse(
  "label1 = value, label2 = value, label3 in (value1, value2)");
```

`dotnet add package LabelKit.EFCore.PostgreSQL`

```csharp
var expressionBuilder = NpgsqlLabelSelectorExpressionBuilders.Jsonb<Dictionary<string, string>>();

var entities = await dbContext.Set<Entity>()
  .MatchLabels(e => e.Labels, selector, expressionBuilder)
  .ToListAsync();
```

Executed SQL:
```postgresql
-- @__json_1='{"label1":"value","label2":"value"}' (DbType = Object)
-- @__Format_2='{"label3":"value1"}' (DbType = Object)
-- @__Format_3='{"label3":"value2"}' (DbType = Object)
SELECT t."Id", t."Labels"
FROM "TestEntity" AS t
WHERE t."Labels" @> @__json_1 AND (t."Labels" @> @__Format_2 OR t."Labels" @> @__Format_3)
```

### Label-Selectors

Label-selectors can be easily created and extended...

```csharp
var selector = LabelSelector.New()
  .Match("label1").Exact("value")
  .Match("label2").Not("value")
  .Match("label3").In("value1", "value2")
  .Match("label4").NotIn("value1", "value2")
  .Match("label5").Exists()
  .Match("label6").NotExists();
```

They can be merged...

```csharp
var selector1 = LabelSelector.New()
  .Match("label1").Exact("value");

var selector2 = LabelSelector.New()
  .Match("label2").Exact("value");

// Contains a fully copy of all expressions
var merged = selector1.Merge(selector2);

// You can merge an arbitrary number of selectors

merged = LabelSelector.Merge(selector1, selector2, ...);

```

They can be rendered...

```csharp
// label1 = value, label2 = value
merged.ToString();
```

### Matching Labels Offline

You can also use label-selectors offline without any database interaction.

`dotnet add package LabelKit`

```csharp
var selector = LabelSelector.New()
  .Match("label1").Exact("value")
  .Match("label2").Exact("value");

string[] labels = [ "label1:value", "label2:value" ];

// Default delimiter is ':'
// -> true
var doesMatch = selector.Matches(labels);
```

The same can be done with dictionary-like labels:
```csharp
var selector = LabelSelector.New()
  .Match("label1").Exact("value")
  .Match("label2").Exact("value");

var labels = new Dictionary<string, string()
{
  ["label1"] = "value",
  ["label2"] = "value"
};

// -> true
var doesMatch = selector.Matches(labels);
```

> [!TIP]
> 
> Any component implementing **ILabelSelector** (meaning it can provide a collection of selector-expressions) can be used to match offline.

## Expressions (EFCore)

LabelKit supports infrastructure for building expression-trees that can be used to filter IQueryables.
Components implementing **ILabelSelectorExpressionBuilder** are responsible for creating **Expressions** from label-selectors.
Different expression-builders are needed for different scenarios.

Example: If your labels are stored as JSONB (PostgreSQL), the resulting SQL query has to be
vastly different from if they were stored as an array. Therefore, you need a different expression.

### Supported EFCore Providers

- [PostgreSQL](https://github.com/npgsql/efcore.pg) (>=5.0.5)
  - Supports labels stored as [JSONB](https://www.npgsql.org/efcore/mapping/json.html#traditional-poco-mapping-deprecated) ("name": "value")
  - Supports labels stored as [primitive-collection](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#primitive-collections) of strings ("name{separator}value").
- [MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) (>=3.2.0)
  - Supports labels stored as [JSON](https://dev.mysql.com/doc/refman/8.0/en/json.html) ("name": "value")
  - Supports labels stored as [primitive-collection](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#primitive-collections) of strings ("name{separator}value").
    - Disabled by default in the provider due to [this issue here](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/pull/1791).
- [SqlServer](https://github.com/dotnet/efcore) (>=8.0.0)
  - Supports labels stored as [primitive-collection](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#primitive-collections) of strings ("name{separator}value").
- [Sqlite](https://github.com/dotnet/efcore) (>=8.0.0)
  - Supports labels stored as [primitive-collection](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#primitive-collections) of strings ("name{separator}value").
- Any other EFCore provider supporting primitive-collections
  - Supports labels stored as [primitive-collection](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#primitive-collections) of strings ("name{separator}value").

### Provided Expression-Builders

- `NpgsqlJsonbLabelSelectorExpressionBuilder` (`NpgsqlLabelSelectorExpressionBuilders.Jsonb()`)
  - Builds expression specifically suited for PostgreSQL JSONB columns.
- `MySqlJsonLabelSelectorExpressionBuilder` (`MySqlLabelSelectorExpressionBuilders.Json()`)
  - Builds expression specifically suited for MySql JSON columns.
- `CollectionLabelSelectorExpressionBuilder` (`LabelSelectorExpressionBuilders.Collection()`)
  - Builds generic expression suitable for any collection of strings.
  - Supported by PostgreSQL, SqlServer, Sqlite (and MySql).
  - Available in package `LabelKit.Expressions`.

> [!IMPORTANT]
> 
> `CollectionLabelSelectorExpressionBuilder` produces expressions that should be translatable to SQL by EFCore
> providers supporting [primitive-collections](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/whatsnew#primitive-collections).
> However, you should also be able to use those expressions offline (compiled).

### Filter IQueryables

`dotnet add package LabelKit.Expressions`

```csharp
using LabelKit;

var expressionBuilder = LabelSelectorExpressionBuilders.Collection<string[]>();

// e => e.Labels is an expression representing the labels to match.
// You can also mark your entity as ILabelledEntity to avoid having to specify this every time.
var entities = await dbContext.Set<Entity>()
  .MatchLabels(e => e.Labels,
    selector => selector
        .Match("label1").Exact("value")
        .Match("label2").Exact("value")
    , expressionBuilder)
  .ToListAsync();
```
