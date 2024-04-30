// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

using System.Collections;

/// <summary>
/// Represents a materialized collection of <see cref="LabelSelectorExpression"/>s that may be modified.
/// </summary>
public class LabelSelector : ILabelSelector, ICloneable, IEquatable<ILabelSelector>
{
  /// <summary>
  /// Collection of <see cref="LabelSelectorExpression"/>s this selector contains.
  /// </summary>
  public IList<LabelSelectorExpression>? Expressions { get; set; }

  public IEnumerator<LabelSelectorExpression> GetEnumerator()
    => this.Expressions?.GetEnumerator() ?? Enumerable.Empty<LabelSelectorExpression>().GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

  /// <summary>
  /// Create a new <see cref="LabelSelector"/> optionally from existing <see cref="LabelSelectorExpression"/>s.
  /// </summary>
  /// <param name="expressions">Source expressions.</param>
  /// <param name="copy">Whether to copy the expressions or reuse the original instances.</param>
  /// <returns></returns>
  public static LabelSelector New(IEnumerable<LabelSelectorExpression>? expressions = null, bool copy = true)
    => new()
    {
      Expressions = copy
        ? expressions?.Select(e => (LabelSelectorExpression) e.Clone()).ToList()
        : expressions?.ToList()
    };

  /// <summary>
  /// Combines an arbitrary number of <see cref="ILabelSelector"/>s into one (without materializing).
  /// </summary>
  /// <param name="selectors">Selectors to combine.</param>
  /// <returns></returns>
  public static ILabelSelector Combine(params ILabelSelector[] selectors)
    => Combine(selectors.Length is 0 ? (IEnumerable<ILabelSelector>?) null : selectors);

  /// <summary>
  /// Combines an arbitrary number of <see cref="ILabelSelector"/>s into one (without materializing).
  /// </summary>
  /// <param name="selectors">Selectors to combine.</param>
  /// <returns></returns>
  public static ILabelSelector Combine(IEnumerable<ILabelSelector>? selectors)
    => new CombinedLabelSelector(selectors);

  /// <summary>
  /// Merges an arbitrary number of <see cref="ILabelSelector"/>s into one.
  /// This materializes the expressions and removes duplicates.
  /// </summary>
  /// <param name="selectors">Selectors to merge.</param>
  /// <returns></returns>
  public static LabelSelector Merge(params ILabelSelector[] selectors)
    => Merge(selectors.Length is 0 ? (IEnumerable<ILabelSelector>?) null : selectors);

  /// <summary>
  /// Merges an arbitrary number of <see cref="ILabelSelector"/>s into one.
  /// This materializes the expressions and removes duplicates.
  /// </summary>
  /// <param name="selectors">Selectors to merge.</param>
  /// <returns></returns>
  public static LabelSelector Merge(IEnumerable<ILabelSelector>? selectors)
    => New(selectors?.SelectMany(s => s).Distinct());

  object ICloneable.Clone() => this.Clone();

  public LabelSelector Clone() => New(this);

  /// <summary>
  /// Renders this selector.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
    => string.Join(", ", this.Select(e => e.ToString()));

  protected bool Equals(LabelSelector other)
    => this.Equals((ILabelSelector) other);

  public bool Equals(ILabelSelector other)
    => this.SequenceEqual(other);

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj))
    {
      return false;
    }

    if (ReferenceEquals(this, obj))
    {
      return true;
    }

    return obj.GetType() == this.GetType() && this.Equals((LabelSelector) obj);
  }

  public override int GetHashCode() => this.Expressions != null ? this.Expressions.GetHashCode() : 0;
}
