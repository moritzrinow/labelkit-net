// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

/// <summary>
/// Marks an entity containing labels.
/// </summary>
/// <remarks>Allows comfortable usage of queryable extension methods.</remarks>
/// <typeparam name="TLabels">Type of the labels.</typeparam>
public interface ILabelledEntity<out TLabels>
{
  /// <summary>
  /// Property holding the entity labels.
  /// </summary>
  TLabels ArrayLabels { get; }
}
