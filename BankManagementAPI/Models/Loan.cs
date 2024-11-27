namespace BankManagementAPI.Models
{
	public class Loan
	{
		public string FullName { get; set; }
		public int Id { get; set; }
		public int UserId { get; set; }
		public int AccountId { get; set; }
		public string LoanType { get; set; } // Personal, Home, Auto, etc.
		public decimal LoanAmount { get; set; }
		public int LoanTerm { get; set; } // In months
		public decimal InterestRate { get; set; }
		public string Status { get; set; } // Pending, Approved, Rejected
		public DateTime ApplicationDate { get; set; }
		public DateTime? ApprovalDate { get; set; }
		public User User { get; set; }
		public Account Account { get; set; }
	}
}
