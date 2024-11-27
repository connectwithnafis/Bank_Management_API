namespace BankManagementAPI.Models;

public class User
{
	public int Id { get; set; }
	public string Username { get; set; }
	public string Password { get; set; } 
	public string Role { get; set; } 
	public ICollection<Account> Accounts { get; set; } = new List<Account>();
	public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
