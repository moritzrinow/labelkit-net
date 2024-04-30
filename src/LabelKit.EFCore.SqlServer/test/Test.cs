// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.EFCore.SqlServer.Test;

using DotNet.Testcontainers;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Testcontainers.MsSql;
using Xunit.Abstractions;

public class Test(ITestOutputHelper output) : IAsyncLifetime
{
  private readonly MsSqlContainer sqlserver = new MsSqlBuilder()
    .WithLogger(ConsoleLogger.Instance)
    .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
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
  public async Task Collection()
  {
    var connectionString = this.sqlserver.GetConnectionString();

    await using var dbContext = new TestContext(connectionString);

    dbContext.AddRange(this.data);

    await dbContext.SaveChangesAsync();

    var expressionBuilder = LabelSelectorExpressionBuilders.Collection<string[]>();

    var query = dbContext.Set<TestEntity>()
      .AsNoTracking()
      .MatchLabels(this.selector, expressionBuilder);

    output.WriteLine(query.ToQueryString());

    var entities = await query.ToListAsync();

    entities.ShouldHaveSingleItem();
    entities.ShouldAllBe(e => e.Id == 1);
  }

  public async Task InitializeAsync()
  {
    await this.sqlserver.StartAsync();

    var connectionString = this.sqlserver.GetConnectionString();

    await using var dbContext = new TestContext(connectionString);

    await dbContext.Database.EnsureCreatedAsync();
  }

  public async Task DisposeAsync()
  {
    await this.sqlserver.DisposeAsync();
  }
}
