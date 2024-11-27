namespace BankManagementAPI.DTOs;

public class UserRegistrationDTO
{
	public string Username { get; set; }
	public string Password { get; set; }
	public string Role { get; set; } // Default: "Customer"
}
