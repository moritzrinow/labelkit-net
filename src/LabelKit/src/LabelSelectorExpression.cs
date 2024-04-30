// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

using System.Text;

/// <summary>
/// Expression for matching a label.
/// </summary>
public class LabelSelectorExpression : IEquatable<LabelSelectorExpression>, ICloneable
{
  /// <summary>
  /// Name of the label.
  /// </summary>
  public string Name { get; set; } = null!;

  /// <summary>
  /// Operator with which to match.
  /// </summary>
  public LabelSelectorOperator Operator { get; set; }

  /// <summary>
  /// Values to match, if any.
  /// </summary>
  public string[]? Values { get; set; }

  /// <summary>
  /// Renders the expression.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public override string ToString()
  {
    var str = new StringBuilder();

    str.Append(this.Name);

    switch (this.Operator)
    {
      case LabelSelectorOperator.In when this.Values is { Length: 1 } values:
        str.Append($" = {values[0]}");
        break;
      case LabelSelectorOperator.In:
        str.Append($" in ({string.Join(", ", this.Values ?? Enumerable.Empty<string>())})");
        break;
      case LabelSelectorOperator.NotIn when this.Values is { Length: 1 } values:
        str.Append($" != {values[0]}");
        break;
      case LabelSelectorOperator.NotIn:
        str.Append($" notin ({string.Join(", ", this.Values ?? Enumerable.Empty<string>())})");
        break;
      case LabelSelectorOperator.NotExists:
        str.Insert(0, '!');
        break;
      case LabelSelectorOperator.Exists:
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }

    return str.ToString();
  }

  public bool Equals(LabelSelectorExpression? other)
  {
    if (ReferenceEquals(null, other))
    {
      return false;
    }

    if (ReferenceEquals(this, other))
    {
      return true;
    }

    return this.Name == other.Name
           && this.Operator == other.Operator
           && (this.Values ?? Enumerable.Empty<string>()).SequenceEqual(other.Values ?? Enumerable.Empty<string>());
  }

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

    return obj.GetType() == this.GetType() && this.Equals((LabelSelectorExpression) obj);
  }

  public override int GetHashCode()
  {
    unchecked
    {
      var hashCode = this.Name.GetHashCode();
      hashCode = (hashCode * 397) ^ (int) this.Operator;
      hashCode = (hashCode * 397) ^ (this.Values != null ? this.Values.GetHashCode() : 0);
      return hashCode;
    }
  }

  public object Clone()
  {
    return new LabelSelectorExpression
    {
      Name = this.Name,
      Operator = this.Operator,
      Values = this.Values?.ToArray()
    };
  }
}
