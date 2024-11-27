namespace BankManagementAPI.DTOs
{
	public class EditUserDTO
	{
		public string? Username { get; set; }
		public string? Password { get; set; }
		public string? Role { get; set; }
		public List<EditAccountDTO>? Accounts { get; set; }
	}
}
