// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.EFCore.Pomelo.MySql.Test;

using System.Data.Common;
using DotNet.Testcontainers;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Shouldly;
using Testcontainers.MySql;
using Xunit.Abstractions;

public class Test(ITestOutputHelper output) : IAsyncLifetime
{
  private const string mysqlVersion = "8.0";

  private DbDataSource dataSource = null!;

  private readonly MySqlContainer mysql = new MySqlBuilder()
    .WithLogger(ConsoleLogger.Instance)
    .WithImage($"mysql:{mysqlVersion}")
    .WithDatabase("test")
    .Build();

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
  public async Task Collection()
  {
    await using var dbContext = new TestContext(this.dataSource, mysqlVersion);

    var expressionBuilder = LabelSelectorExpressionBuilders.Collection<string[]>();

    var query = dbContext.Set<TestEntity>()
      .AsNoTracking()
      .MatchLabels(this.selector, expressionBuilder);

    output.WriteLine(query.ToQueryString());

    var entities = await query.ToListAsync();

    entities.ShouldHaveSingleItem();
    entities.ShouldAllBe(e => e.Id == 1);
  }

  [Fact]
  public async Task Json()
  {
    await using var dbContext = new TestContext(this.dataSource, mysqlVersion);

    var expressionBuilder = MySqlLabelSelectorExpressionBuilders.Json<Dictionary<string, string>>();

    var query = dbContext.Set<TestEntity>()
      .AsNoTracking()
      .MatchLabels(e => e.DictLabels, this.selector, expressionBuilder);

    output.WriteLine(query.ToQueryString());

    var entities = await query.ToListAsync();

    entities.ShouldHaveSingleItem();
    entities.ShouldAllBe(e => e.Id == 1);
  }

  public async Task InitializeAsync()
  {
    await this.mysql.StartAsync();

    var connectionStringBuilder = new MySqlConnectionStringBuilder(this.mysql.GetConnectionString())
    {
      AllowUserVariables = true,
      UseAffectedRows = false
    };

    this.dataSource = new MySqlDataSourceBuilder(connectionStringBuilder.ConnectionString)
      .Build();

    await using var dbContext = new TestContext(this.dataSource, mysqlVersion);

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

    await this.mysql.DisposeAsync();
  }
}
