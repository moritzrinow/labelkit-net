// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.EFCore.Pomelo.MySql.Test;

using System.Data.Common;
using Microsoft.EntityFrameworkCore;

public class TestContext(DbDataSource dataSource, string version)
  : DbContext
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseMySql(dataSource, ServerVersion.Parse(version), mysql =>
    {
      mysql.EnablePrimitiveCollectionsSupport();

      mysql.UseNewtonsoftJson();
    });

    optionsBuilder.EnableDetailedErrors();
    optionsBuilder.EnableSensitiveDataLogging();
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<TestEntity>(entity =>
    {
      entity.Property(e => e.DictLabels).HasColumnType("json");
    });
  }
}
