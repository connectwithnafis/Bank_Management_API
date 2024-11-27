namespace BankManagementAPI.DTOs
{
	public class AccountDTO
	{
		public int Id { get; set; }
		public string AccountType { get; set; }
		public decimal Balance { get; set; }

		public List<TransactionDTO> Transactions { get; set; } 
	}
}
