// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.Internal;

using Pidgin;
using Parser = Pidgin.Parser;

public static class Lexers
{
  public static Parser<char, LabelSelectorToken> Whitespace { get; } = Parser.Whitespace.ThenReturn(LabelSelectorToken.Whitespace);

  public static Parser<char, LabelSelectorToken> Open { get; } = Parser.String("(").ThenReturn(LabelSelectorToken.Open);

  public static Parser<char, LabelSelectorToken> Close { get; } = Parser.String(")").ThenReturn(LabelSelectorToken.Close);

  public static Parser<char, LabelSelectorToken> NotEquals { get; } = Parser.Try(Parser.String("!=").ThenReturn(LabelSelectorToken.NotEquals));

  public static Parser<char, LabelSelectorToken> Equals_ { get; } = Parser.String("=").ThenReturn(LabelSelectorToken.Equals_);

  public static Parser<char, LabelSelectorToken> DoubleEquals { get; } = Parser.Try(Parser.String("==").ThenReturn(LabelSelectorToken.Equals_));

  public static Parser<char, LabelSelectorToken> Not { get; } = Parser.String("!").ThenReturn(LabelSelectorToken.Not);

  public static Parser<char, LabelSelectorToken> In { get; } = Parser.Try(Parser.String("in").ThenReturn(LabelSelectorToken.In));

  public static Parser<char, LabelSelectorToken> NotIn { get; } = Parser.Try(Parser.String("notin").ThenReturn(LabelSelectorToken.NotIn));

  public static Parser<char, LabelSelectorToken> Comma { get; } = Parser.String(",").ThenReturn(LabelSelectorToken.Comma);

  public static Parser<char, LabelSelectorToken> Label { get; } = Parser.AnyCharExcept(" ()!=,").AtLeastOnceString().Select(LabelSelectorToken.Label);

  public static Parser<char, IEnumerable<LabelSelectorToken>> Lexer { get; } = Parser.OneOf(Whitespace,
      Open,
      Close,
      NotEquals,
      DoubleEquals,
      Equals_,
      Not,
      In,
      NotIn,
      Comma,
      Label)
    .Many()
    .Select(tokens => tokens.Where(t => t != LabelSelectorToken.Whitespace))
    .Before(Parser<char>.End);
}
