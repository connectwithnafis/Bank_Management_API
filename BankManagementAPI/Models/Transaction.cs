namespace BankManagementAPI.Models;

public class Transaction
{
	public int Id { get; set; }
	public int AccountId { get; set; }
	public DateTime Date { get; set; }
	public string TransactionType { get; set; } // Deposit, Withdrawal, Transfer
	public decimal Amount { get; set; }

	public Account Account { get; set; }
}
