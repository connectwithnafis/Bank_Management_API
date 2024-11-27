namespace BankManagementAPI.Models;

public class Account
{
	public int Id { get; set; }
	public int UserId { get; set; }
	public string AccountType { get; set; } // Savings, Checking, etc.
	public decimal Balance { get; set; }

	public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
	public ICollection<Loan> Loans { get; set; } = new List<Loan>();
	public User User { get; set; }
}
