// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.EFCore.PostgreSQL.Test;

using System.Data.Common;
using Microsoft.EntityFrameworkCore;

public class TestContext(DbDataSource dataSource)
  : DbContext
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseNpgsql(dataSource, npgsql =>
    {
    });

    optionsBuilder.EnableDetailedErrors();
    optionsBuilder.EnableSensitiveDataLogging();
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<TestEntity>(entity =>
    {
      entity.Property(e => e.DictLabels).HasColumnType("jsonb");
    });
  }
}
