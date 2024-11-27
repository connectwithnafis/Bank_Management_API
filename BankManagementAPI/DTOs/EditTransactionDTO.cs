namespace BankManagementAPI.DTOs
{
	public class EditTransactionDTO
	{
		public int Id { get; set; }
		public string? TransactionType { get; set; }
		public decimal? Amount { get; set; }
		public DateTime? Date { get; set; }
	}
}
