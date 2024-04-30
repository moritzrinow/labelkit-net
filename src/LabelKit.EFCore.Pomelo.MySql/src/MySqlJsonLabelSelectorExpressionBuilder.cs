// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

using System.Linq.Expressions;
using System.Text;
using LinqKit;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// <see cref="ILabelSelectorExpressionBuilder{TLabels}"/> for EFCore-MySql building expressions targeting labels represented by JSON columns.
/// <remarks>
/// Use this builder if your labels are key-value pairs stored as JSON.
/// </remarks>
/// </summary>
/// <typeparam name="T">Type of labels.</typeparam>
public class MySqlJsonLabelSelectorExpressionBuilder<T> : ILabelSelectorExpressionBuilder<T>
{
  public Expression<Func<T, bool>> Build(ILabelSelector selector)
  {
    var expression = PredicateBuilder.New<T>(true);

    var exactMatches = selector.GetExactMatches().ToArray();

    if (exactMatches is { Length: > 0 })
    {
      var jsonBuilder = new StringBuilder();

      jsonBuilder.Append('{');

      var jsonParts = exactMatches.Select(e => $"\"{e.name}\":\"{e.value}\"");

      jsonBuilder.Append(string.Join(",", jsonParts));

      jsonBuilder.Append('}');

      var json = jsonBuilder.ToString();

      expression = expression.And(e => EF.Functions.JsonContains(e!, json));
    }

    foreach (var selectorExpression in selector)
    {
      switch (selectorExpression)
      {
        case { Operator: LabelSelectorOperator.In, Values.Length: 1 }:
          continue;
        case { Operator: LabelSelectorOperator.In, Values.Length: > 0 }:
          expression = expression.And(selectorExpression.Values.Aggregate(PredicateBuilder.New<T>(false), (current, value) => current.Or(e => EF.Functions.JsonContains(e!, $"{{\"{selectorExpression.Name}\":\"{value}\"}}"))));
          break;
        case { Operator: LabelSelectorOperator.NotIn, Values.Length: > 0 }:
          expression = expression.And(selectorExpression.Values.Aggregate(PredicateBuilder.New<T>(true), (current, value) => current.And(e => !EF.Functions.JsonContains(e!, $"{{\"{selectorExpression.Name}\":\"{value}\"}}"))));
          break;
        case { Operator: LabelSelectorOperator.Exists }:
          expression = expression.And(e => EF.Functions.JsonContainsPath(e!, $"$.{selectorExpression.Name}"));
          break;
        case { Operator: LabelSelectorOperator.NotExists }:
          expression = expression.And(e => !EF.Functions.JsonContainsPath(e!, $"$.{selectorExpression.Name}"));
          break;
      }
    }

    return expression;
  }
}
