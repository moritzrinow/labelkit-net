// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

public static partial class MySqlLabelSelectorExpressionBuilders
{
  internal static class Builders<TLabels>
  {
    public static ILabelSelectorExpressionBuilder<TLabels> Json
      => new MySqlJsonLabelSelectorExpressionBuilder<TLabels>();
  }

  /// <inheritdoc cref="MySqlJsonLabelSelectorExpressionBuilder{T}"/>
  public static ILabelSelectorExpressionBuilder<TLabels> Json<TLabels>()
    => Builders<TLabels>.Json;
}
