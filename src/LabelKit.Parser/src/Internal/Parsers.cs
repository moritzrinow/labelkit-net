// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.Internal;

using Pidgin;
using Parser = Pidgin.Parser;

public static class Parsers
{
  public static Parser<LabelSelectorToken, LabelSelectorToken> Open { get; } = Parser<LabelSelectorToken>.Token(LabelSelectorToken.Open);

  public static Parser<LabelSelectorToken, LabelSelectorToken> Close { get; } = Parser<LabelSelectorToken>.Token(LabelSelectorToken.Close);

  public static Parser<LabelSelectorToken, LabelSelectorToken> NotEquals { get; } = Parser<LabelSelectorToken>.Token(LabelSelectorToken.NotEquals);

  public static Parser<LabelSelectorToken, LabelSelectorToken> Equals_ { get; } = Parser<LabelSelectorToken>.Token(LabelSelectorToken.Equals_);

  public static Parser<LabelSelectorToken, LabelSelectorToken> Not { get; } = Parser<LabelSelectorToken>.Token(LabelSelectorToken.Not);

  public static Parser<LabelSelectorToken, LabelSelectorToken> In { get; } = Parser<LabelSelectorToken>.Token(LabelSelectorToken.In);

  public static Parser<LabelSelectorToken, LabelSelectorToken> NotIn { get; } = Parser<LabelSelectorToken>.Token(LabelSelectorToken.NotIn);

  public static Parser<LabelSelectorToken, LabelSelectorToken> Comma { get; } = Parser<LabelSelectorToken>.Token(LabelSelectorToken.Comma);

  public static Parser<LabelSelectorToken, LabelSelectorToken> Label { get; } = Parser<LabelSelectorToken>.Token(t => t.Type == LabelSelectorToken.TokenType.Label);

  public static Parser<LabelSelectorToken, IEnumerable<LabelSelectorToken>> Values { get; } = Label.Separated(Comma).Between(Open, Close);

  public static Parser<LabelSelectorToken, LabelSelectorExpression> InExpression { get; } = Label.Then(In, (name, _) => name).Then(Values, (name, values) => new LabelSelectorExpression
  {
    Name = name.Value!,
    Operator = LabelSelectorOperator.In,
    Values = values.Select(v => v.Value).ToArray()!
  });

  public static Parser<LabelSelectorToken, LabelSelectorExpression> NotInExpression { get; } = Label.Then(NotIn, (name, _) => name).Then(Values, (name, values) => new LabelSelectorExpression
  {
    Name = name.Value!,
    Operator = LabelSelectorOperator.NotIn,
    Values = values.Select(v => v.Value).ToArray()!
  });

  public static Parser<LabelSelectorToken, LabelSelectorExpression> EqualsExpression { get; } = Label.Then(Equals_, (name, _) => name).Then(Label, (name, value) => new LabelSelectorExpression
  {
    Name = name.Value!,
    Operator = LabelSelectorOperator.In,
    Values = [value.Value!]
  });

  public static Parser<LabelSelectorToken, LabelSelectorExpression> NotEqualsExpression { get; } = Label.Then(NotEquals, (name, _) => name).Then(Label, (name, value) => new LabelSelectorExpression
  {
    Name = name.Value!,
    Operator = LabelSelectorOperator.NotIn,
    Values = [value.Value!]
  });

  public static Parser<LabelSelectorToken, LabelSelectorExpression> ExistsExpression { get; } = Label.Select(name => new LabelSelectorExpression
  {
    Name = name.Value!,
    Operator = LabelSelectorOperator.Exists
  });

  public static Parser<LabelSelectorToken, LabelSelectorExpression> NotExistsExpression { get; } = Not.Then(Label).Select(name => new LabelSelectorExpression
  {
    Name = name.Value!,
    Operator = LabelSelectorOperator.NotExists
  });

  public static Parser<LabelSelectorToken, IEnumerable<LabelSelectorExpression>> Expressions { get; } = Parser.OneOf(Parser.Try(InExpression),
      Parser.Try(NotInExpression),
      Parser.Try(EqualsExpression),
      Parser.Try(NotEqualsExpression),
      Parser.Try(NotExistsExpression),
      Parser.Try(ExistsExpression))
    .Separated(Comma);

  public static Parser<char, Result<LabelSelectorToken, IEnumerable<LabelSelectorExpression>>> LabelSelectorExpressions { get; } = Lexers.Lexer.Select(token => Expressions.Parse(token));
}
