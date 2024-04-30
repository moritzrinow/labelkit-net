// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.EFCore.Sqlite.Test;

using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit.Abstractions;

public class Test(ITestOutputHelper output)
{
  private readonly LabelSelector selector = LabelSelector.New()
    .Match("label1").Exact("value")
    .Match("label2").Exact("value")
    .Match("label3").In("value1", "value2")
    .Match("label4").Not("value")
    .Match("label5").NotIn("value1", "value2")
    .Match("label6").Exists()
    .Match("label7").NotExists();

  private readonly IEnumerable<TestEntity> data = new[]
  {
    // Matches selector
    new TestEntity
    {
      Id = 1,
      ArrayLabels =
      [
        "label1:value",
        "label2:value",
        "label3:value1",
        "label4:value1",
        "label5:value",
        "label6:value"
      ]
    },
    // Does not match selector
    new TestEntity
    {
      Id = 2,
      ArrayLabels =
      [
        "label1:value",
        "label2:value",
        "label3:value1",
        "label4:value1",
        "label5:value",
        "label6:value",
        "label7:value"
      ]
    }
  };

  [Fact]
  public void Collection()
  {
    var file = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

    using var dbContext = new TestContext($"Data Source={file}");

    dbContext.Database.EnsureCreated();

    dbContext.AddRange(this.data);

    dbContext.SaveChanges();

    var expressionBuilder = LabelSelectorExpressionBuilders.Collection<string[]>();

    var query = dbContext.Set<TestEntity>()
      .AsNoTracking()
      .MatchLabels(this.selector, expressionBuilder);

    output.WriteLine(query.ToQueryString());

    var entities = query.ToList();

    entities.ShouldHaveSingleItem();
    entities.ShouldAllBe(e => e.Id == 1);
  }
}
