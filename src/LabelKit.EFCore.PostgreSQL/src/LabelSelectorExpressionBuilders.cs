// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

public static partial class NpgsqlLabelSelectorExpressionBuilders
{
  internal static class Builders<TLabels>
  {
    public static ILabelSelectorExpressionBuilder<TLabels> Jsonb
      => new NpgsqlJsonbLabelSelectorExpressionBuilder<TLabels>();
  }

  /// <inheritdoc cref="NpgsqlJsonbLabelSelectorExpressionBuilder{T}"/>
  public static ILabelSelectorExpressionBuilder<TLabels> Jsonb<TLabels>()
    => Builders<TLabels>.Jsonb;
}
