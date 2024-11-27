namespace BankManagementAPI.DTOs;

public class TransactionDTO
{
	public int Id { get; set; }
	public decimal Amount { get; set; }
	public DateTime Date { get; set; }
	public string TransactionType { get; set; }
}
