// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit;

/// <summary>
/// Represents a parsed token of raw label-selectors.
/// </summary>
public record LabelSelectorToken
{
  public enum TokenType
  {
    None,
    Whitespace,
    Open,
    Close,
    Equals,
    NotEquals,
    Not,
    In,
    NotIn,
    Comma,
    Label
  }

  /// <summary>
  /// Type of the token.
  /// </summary>
  public TokenType Type { get; set; }

  /// <summary>
  /// Value of the token if it represents a label name or value.
  /// </summary>
  public string? Value { get; set; }

  public static LabelSelectorToken Whitespace { get; } = new() { Type = TokenType.Whitespace };

  public static LabelSelectorToken Open { get; } = new() { Type = TokenType.Open };

  public static LabelSelectorToken Close { get; } = new() { Type = TokenType.Close };

  public static LabelSelectorToken Equals_ { get; } = new() { Type = TokenType.Equals };

  public static LabelSelectorToken NotEquals { get; } = new() { Type = TokenType.NotEquals };

  public static LabelSelectorToken Not { get; } = new() { Type = TokenType.Not };

  public static LabelSelectorToken In { get; } = new() { Type = TokenType.In };

  public static LabelSelectorToken NotIn { get; } = new() { Type = TokenType.NotIn };

  public static LabelSelectorToken Comma { get; } = new() { Type = TokenType.Comma };

  public static LabelSelectorToken Label(string value) => new() { Type = TokenType.Label, Value = value };
}
