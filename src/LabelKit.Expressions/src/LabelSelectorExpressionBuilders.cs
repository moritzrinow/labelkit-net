// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

public static partial class LabelSelectorExpressionBuilders
{
  /// <inheritdoc cref="CollectionLabelSelectorExpressionBuilder{T}"/>
  public static ILabelSelectorExpressionBuilder<TLabels> Collection<TLabels>(string delimiter = ":")
    where TLabels : IEnumerable<string>
    => new CollectionLabelSelectorExpressionBuilder<TLabels>(delimiter);
}
