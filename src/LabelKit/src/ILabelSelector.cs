// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

/// <summary>
/// Represents a collection of <see cref="LabelSelectorExpression"/>s.
/// </summary>
public interface ILabelSelector : IEnumerable<LabelSelectorExpression>
{
}
