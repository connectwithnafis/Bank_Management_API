namespace BankManagementAPI.DTOs
{
	public class LoanDTO
	{
		public int UserId { get; set; }
		public int AccountId { get; set; }
		public string LoanType { get; set; } // Personal, Home, Auto, etc.
		public decimal LoanAmount { get; set; }
		public int LoanTerm { get; set; } // In months
		public decimal InterestRate { get; set; }
		public DateTime ApplicationDate { get; set; }
		public DateTime? ApprovalDate { get; set; }
	}
}
