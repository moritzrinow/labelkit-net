// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

public static class LabelSelectorExpressionExtensions
{
  /// <summary>
  /// Whether the <see cref="LabelSelectorExpression"/> represents an exact label match.
  /// </summary>
  /// <param name="expression">The expression.</param>
  /// <returns></returns>
  public static bool IsExactMatch(this LabelSelectorExpression expression)
    => expression is { Operator: LabelSelectorOperator.In, Values.Length: 1 };
}
