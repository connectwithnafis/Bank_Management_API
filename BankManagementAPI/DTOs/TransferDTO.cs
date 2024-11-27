namespace BankManagementAPI.DTOs;

public class TransferDTO
{
	public decimal Amount { get; set; }
	public decimal RecipientAccountId { get; set; }
	public bool IsExternal { get; set; }
}

