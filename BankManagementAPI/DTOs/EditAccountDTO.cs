namespace BankManagementAPI.DTOs
{
	public class EditAccountDTO
	{
		public int Id { get; set; }
		public string? AccountType { get; set; }
		public decimal? Balance { get; set; }
		public List<EditTransactionDTO>? Transactions { get; set; }
	}
}
