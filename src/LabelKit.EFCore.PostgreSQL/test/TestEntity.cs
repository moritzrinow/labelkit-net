// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.EFCore.PostgreSQL.Test;

public class TestEntity
{
  public int Id { get; set; }

  public Dictionary<string, string> DictLabels { get; set; } = null!;

  public string[] ArrayLabels { get; set; } = null!;
}
