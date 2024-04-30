// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

using System.Linq.Expressions;
using LinqKit;

/// <summary>
/// <see cref="ILabelSelectorExpressionBuilder{TLabels}"/> building expressions targeting labels represented by a collection of strings.
/// <remarks>You can use this builder on EFCore primitive-collections (>= EFCore 8).</remarks>
/// </summary>
/// <param name="delimiter">Delimiter used to separate label names and values.</param>
/// <typeparam name="T">Type of labels.</typeparam>
public class CollectionLabelSelectorExpressionBuilder<T>(string delimiter = ":") : ILabelSelectorExpressionBuilder<T>
  where T : IEnumerable<string>
{
  public Expression<Func<T, bool>> Build(ILabelSelector selector)
  {
    var expression = PredicateBuilder.New<T>(true);

    var exactMatches = selector.GetExactMatches().Select(s => $"{s.name}{delimiter}{s.value}").ToArray();

    if (exactMatches is { Length: > 0 })
    {
      expression = expression.And(e => exactMatches.All(l => e.Contains(l)));
    }

    foreach (var selectorExpression in selector)
    {
      switch (selectorExpression)
      {
        case { Operator: LabelSelectorOperator.In, Values.Length: 1 }:
          continue;
        case { Operator: LabelSelectorOperator.In, Values.Length: > 0 }:
          expression = expression.And(selectorExpression.Values.Aggregate(PredicateBuilder.New<T>(false), (current, value) => current.Or(e => e.Contains($"{selectorExpression.Name}:{value}"))));
          break;
        case { Operator: LabelSelectorOperator.NotIn, Values.Length: > 0 }:
          expression = expression.And(selectorExpression.Values.Aggregate(PredicateBuilder.New<T>(true), (current, value) => current.And(e => !e.Contains($"{selectorExpression.Name}:{value}"))));
          break;
        case { Operator: LabelSelectorOperator.Exists }:
          expression = expression.And(e => e.Any(l => l.StartsWith(selectorExpression.Name)));
          break;
        case { Operator: LabelSelectorOperator.NotExists }:
          expression = expression.And(e => !e.Any(l => l.StartsWith(selectorExpression.Name)));
          break;
      }
    }

    return expression;
  }
}
