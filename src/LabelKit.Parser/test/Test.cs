// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.Parser.Test;

using Internal;
using Pidgin;
using Shouldly;

public class Test
{
  [Fact]
  public void Parse()
  {
    var selector = LabelSelector.New()
      .Match("label1").Exact("value")
      .Match("label2").In("value")
      .Match("label3").In("value1", "value2")
      .Match("label4").Not("value")
      .Match("label5").NotIn("value")
      .Match("label6").NotIn("value1", "value2")
      .Match("label7").Exists()
      .Match("label8").NotExists();

    const string raw = "label1 = value, label2 = value, label3 in (value1, value2), label4 != value, label5 != value, label6 notin (value1, value2), label7, !label8";

    var parsed = LabelSelectorParser.ParseOrThrow(raw);

    Assert.Equal(selector, parsed);

    Assert.Equal(selector.ToString(), parsed.ToString());

    Assert.Equal(raw, selector.ToString());

    Assert.Equal(raw, parsed.ToString());
  }

  [Fact]
  public void Lexer()
  {
    const string raw = "label1 = value, label2 == value, label3 in (value1, value2), label4 != value, label5 != value, label6 notin (value1, value2), label7, !label8";

    var tokens = Lexers.Lexer.ParseOrThrow(raw).ToArray();
  }

  [Fact]
  public void Match()
  {
    string[] arrayLabels1 =
    [
      "label1:value",
      "label2:value",
      "label3:value1",
      "label4:value1",
      "label5:value",
      "label6:value"
    ];

    var dictLabels1 = new Dictionary<string, string>
    {
      ["label1"] = "value",
      ["label2"] = "value",
      ["label3"] = "value1",
      ["label4"] = "value1",
      ["label5"] = "value",
      ["label6"] = "value"
    };

    string[] arrayLabels2 =
    [
      "label1:value",
      "label2:value",
      "label3:value1",
      "label4:value1",
      "label5:value",
      "label6:value",
      "label7:value"
    ];

    var dictLabels2 = new Dictionary<string, string>
    {
      ["label1"] = "value",
      ["label2"] = "value",
      ["label3"] = "value1",
      ["label4"] = "value1",
      ["label5"] = "value",
      ["label6"] = "value",
      ["label7"] = "value"
    };

    var selector = LabelSelector.New()
      .Match("label1").Exact("value")
      .Match("label2").Exact("value")
      .Match("label3").In("value1", "value2")
      .Match("label4").Not("value")
      .Match("label5").NotIn("value1", "value2")
      .Match("label6").Exists()
      .Match("label7").NotExists();

    selector.Matches(arrayLabels1).ShouldBeTrue();
    selector.Matches(dictLabels1).ShouldBeTrue();

    selector.Matches(arrayLabels2).ShouldBeFalse();
    selector.Matches(dictLabels2).ShouldBeFalse();
  }
}
