// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.EFCore.PostgreSQL.Test;

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using LabelKit;
using Npgsql;
using Shouldly;
using Xunit.Abstractions;

public class Test(ITestOutputHelper output) : IAsyncLifetime
{
  private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder()
    .WithDatabase("test")
    .Build();

  private DbDataSource dataSource = null!;

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
      ],
      DictLabels = new Dictionary<string, string>
      {
        ["label1"] = "value",
        ["label2"] = "value",
        ["label3"] = "value1",
        ["label4"] = "value1",
        ["label5"] = "value",
        ["label6"] = "value"
      }
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
      ],
      DictLabels = new Dictionary<string, string>
      {
        ["label1"] = "value",
        ["label2"] = "value",
        ["label3"] = "value1",
        ["label4"] = "value1",
        ["label5"] = "value",
        ["label6"] = "value",
        ["label7"] = "value"
      }
    }
  };

  [Fact]
  public async Task Jsonb()
  {
    await using var dbContext = new TestContext(this.dataSource);

    var expressionBuilder = NpgsqlLabelSelectorExpressionBuilders.Jsonb<Dictionary<string, string>>();

    var query = dbContext.Set<TestEntity>()
      .AsNoTracking().MatchLabels(e => e.DictLabels, this.selector, expressionBuilder);

    output.WriteLine(query.ToQueryString());

    var entities = await query.ToListAsync();

    entities.ShouldHaveSingleItem();
    entities.ShouldAllBe(e => e.Id == 1);
  }

  [Fact]
  public async Task Collection()
  {
    await using var dbContext = new TestContext(this.dataSource);

    var expressionBuilder = LabelSelectorExpressionBuilders.Collection<string[]>();

    var query = dbContext.Set<TestEntity>()
      .AsNoTracking().MatchLabels(e => e.ArrayLabels, this.selector, expressionBuilder);

    output.WriteLine(query.ToQueryString());

    var entities = await query.ToListAsync();

    entities.ShouldHaveSingleItem();
    entities.ShouldAllBe(e => e.Id == 1);
  }

  public async Task InitializeAsync()
  {
    await this.postgres.StartAsync();

    var connectionString = this.postgres.GetConnectionString();

    this.dataSource = new NpgsqlDataSourceBuilder(connectionString)
      .EnableDynamicJson()
      .Build();

    await using var dbContext = new TestContext(this.dataSource);

    await dbContext.Database.EnsureCreatedAsync();

    dbContext.AddRange(this.data);

    await dbContext.SaveChangesAsync();
  }

  public async Task DisposeAsync()
  {
    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    if (this.dataSource is not null)
    {
      await this.dataSource.DisposeAsync();
    }

    await this.postgres.DisposeAsync();
  }
}
