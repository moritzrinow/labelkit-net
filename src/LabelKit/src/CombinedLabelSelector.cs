// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

using System.Collections;

/// <summary>
/// Represents a non-materialized combination of multiple <see cref="ILabelSelector"/>s
/// (No <see cref="LabelSelectorExpression"/> instances are copied or moved around).
/// </summary>
/// <param name="selectors">Selectors to combine.</param>
public class CombinedLabelSelector(IEnumerable<ILabelSelector>? selectors) : ILabelSelector
{
  private readonly ILabelSelector[]? selectors = selectors?.ToArray();

  public IEnumerator<LabelSelectorExpression> GetEnumerator() => this.GetEnumeratorInternal();

  IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

  private IEnumerator<LabelSelectorExpression> GetEnumeratorInternal()
  {
    return (this.selectors ?? Enumerable.Empty<ILabelSelector>()).SelectMany(e => e).GetEnumerator();
  }
}
