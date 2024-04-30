// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.EFCore.Pomelo.MySql.Test;

public class TestEntity : ILabelledEntity<string[]>
{
  public int Id { get; set; }

  public string[] ArrayLabels { get; set; } = null!;

  public Dictionary<string, string> DictLabels { get; set; } = null!;
}
