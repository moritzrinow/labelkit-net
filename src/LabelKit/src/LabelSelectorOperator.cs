// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

public enum LabelSelectorOperator
{
  /// <summary>
  /// Match label against a set of values where any of the values should match.
  /// </summary>
  In,
  /// <summary>
  /// Match label against a set of values where all the values should not match.
  /// </summary>
  NotIn,
  /// <summary>
  /// Match label for existence without considering the value.
  /// </summary>
  Exists,
  /// <summary>
  /// Match label for non-existence without considering the value.
  /// </summary>
  NotExists
}
