// Copyright (c) 2024 Moritz Rinow. All rights reserved.

namespace LabelKit.EFCore.Sqlite.Test;

using Microsoft.EntityFrameworkCore;

public class TestContext(string connectionString)
  : DbContext
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlite(connectionString, sqlite =>
    {
    });

    optionsBuilder.EnableDetailedErrors();
    optionsBuilder.EnableSensitiveDataLogging();
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<TestEntity>(entity =>
    {
    });
  }
}
