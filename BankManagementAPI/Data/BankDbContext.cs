using Microsoft.EntityFrameworkCore;
using BankManagementAPI.Models;

namespace BankManagementAPI.Data;

public class BankDbContext : DbContext
{
	public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }

	public DbSet<User> Users { get; set; }
	public DbSet<Account> Accounts { get; set; }
	public DbSet<Transaction> Transactions { get; set; }
	public DbSet<Loan> Loans { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Account>()
		.HasOne(a => a.User)
		.WithMany(u => u.Accounts)
		.HasForeignKey(a => a.UserId)
		.OnDelete(DeleteBehavior.Restrict);

		// Account and Transaction relationship
		modelBuilder.Entity<Transaction>()
			.HasOne(t => t.Account)
			.WithMany(a => a.Transactions)
			.HasForeignKey(t => t.AccountId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Transaction>()
			.HasOne(t => t.Account)
			.WithMany(a => a.Transactions)
			.HasForeignKey(t => t.AccountId);

		base.OnModelCreating(modelBuilder);
	}
}
