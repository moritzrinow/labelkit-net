// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

public static class LabelSelectorExpressionBuilderExtensions
{
  /// <summary>
  /// Adds a <see cref="LabelSelectorExpression"/> with the operator <see cref="LabelSelectorOperator.In"/> that matches a single value.
  /// </summary>
  /// <param name="value">Value to match.</param>
  /// <returns></returns>
  public static LabelSelector Exact(this LabelSelectorExpressionBuilder builder, string value)
    => builder.In(value);

  /// <summary>
  /// Adds a <see cref="LabelSelectorExpression"/> with the operator <see cref="LabelSelectorOperator.NotIn"/> that matches a single value.
  /// </summary>
  /// <param name="value">Value to match.</param>
  /// <returns></returns>
  public static LabelSelector Not(this LabelSelectorExpressionBuilder builder, string value)
    => builder.NotIn(value);

  /// <inheritdoc cref="LabelSelectorExpressionBuilder.In(IEnumerable{string})"/>
  public static LabelSelector In(this LabelSelectorExpressionBuilder builder, params string[] values)
    => builder.In(values);

  /// <inheritdoc cref="LabelSelectorExpressionBuilder.NotIn(IEnumerable{string})"/>
  public static LabelSelector NotIn(this LabelSelectorExpressionBuilder builder, params string[] values)
    => builder.NotIn(values);
}
