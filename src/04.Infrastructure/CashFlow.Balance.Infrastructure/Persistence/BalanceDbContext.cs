using CashFlow.Balance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Balance.Infrastructure.Persistence;

public sealed class BalanceDbContext(DbContextOptions<BalanceDbContext> options) : DbContext(options)
{
    public DbSet<DailyBalance> DailyBalances => Set<DailyBalance>();

    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BalanceDbContext).Assembly);
    }
}
