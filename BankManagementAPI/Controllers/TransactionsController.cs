using BankManagementAPI.Data;
using BankManagementAPI.DTOs;
using BankManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TransactionsController : ControllerBase
{
	private readonly BankDbContext _context;

	public TransactionsController(BankDbContext context)
	{
		_context = context;
	}

	// POST: api/Transactions/deposit
	[HttpPost("deposit")]
	public async Task<IActionResult> Deposit([FromBody] TransactionDTO transactionDto)
	{
		var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (string.IsNullOrEmpty(userId))
			return Unauthorized("User not authenticated.");

		var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == int.Parse(userId));

		if (account == null)
			return NotFound("Account not found.");

		// Apply deposit
		account.Balance += transactionDto.Amount;
		_context.Accounts.Update(account);

		var transaction = new Transaction
		{
			AccountId = account.Id,
			Date = DateTime.UtcNow,
			TransactionType = "Deposit",
			Amount = transactionDto.Amount
		};

		_context.Transactions.Add(transaction);
		await _context.SaveChangesAsync();

		return Ok(new { balance = account.Balance });
	}

	// POST: api/Transactions/withdraw
	[HttpPost("withdraw")]
	public async Task<IActionResult> Withdraw([FromBody] TransactionDTO transactionDto)
	{
		var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (string.IsNullOrEmpty(userId))
			return Unauthorized("User not authenticated.");

		var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == int.Parse(userId));

		if (account == null)
			return NotFound("Account not found.");

		if (account.Balance < transactionDto.Amount)
			return BadRequest("Insufficient funds.");

		account.Balance -= transactionDto.Amount;
		_context.Accounts.Update(account);

		var transaction = new Transaction
		{
			AccountId = account.Id,
			Date = DateTime.UtcNow,
			TransactionType = "Withdrawal",
			Amount = transactionDto.Amount
		};

		_context.Transactions.Add(transaction);
		await _context.SaveChangesAsync();

		return Ok(new { balance = account.Balance });
	}

	// POST: api/Transactions/transfer
	[HttpPost("transfer")]
	public async Task<IActionResult> Transfer([FromBody] TransferDTO transferDto)
	{
		var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (string.IsNullOrEmpty(userId))
			return Unauthorized("User not authenticated.");

		// Get the sender's account
		var senderAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == int.Parse(userId));

		if (senderAccount == null)
			return NotFound("Sender account not found.");

		if (senderAccount.Balance < transferDto.Amount)
			return BadRequest("Insufficient funds.");

		// Calculate total transaction amount in the last 24 hours
		var last24HoursTransactions = await _context.Transactions
			.Where(t => t.AccountId == senderAccount.Id && t.Date >= DateTime.UtcNow.AddHours(-24))
			.SumAsync(t => t.Amount);

		if (last24HoursTransactions + transferDto.Amount > 200000000000)
			return BadRequest("Exceeded daily transaction limit of 20,000.");

		// Apply transaction fee (e.g., 1.5% of the transaction amount)
		const decimal feePercentage = 0.015m;
		var transactionFee = transferDto.Amount * feePercentage;

		if (senderAccount.Balance < transferDto.Amount + transactionFee)
			return BadRequest("Insufficient funds to cover the transaction and fees.");

		if (!transferDto.IsExternal) // Internal transfer
		{
			var recipientAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == transferDto.RecipientAccountId);

			if (recipientAccount == null)
				return NotFound("Recipient account not found.");

			// Deduct transaction amount and fee from sender
			senderAccount.Balance -= (transferDto.Amount + transactionFee);
			recipientAccount.Balance += transferDto.Amount;

			_context.Accounts.Update(senderAccount);
			_context.Accounts.Update(recipientAccount);

			// Log the main transaction
			var mainTransaction = new Transaction
			{
				AccountId = senderAccount.Id,
				Date = DateTime.UtcNow,
				TransactionType = "Internal Transfer",
				Amount = transferDto.Amount
			};
			_context.Transactions.Add(mainTransaction);

			// Log the transaction fee
			var feeTransaction = new Transaction
			{
				AccountId = senderAccount.Id,
				Date = DateTime.UtcNow,
				TransactionType = "Transfer Fee",
				Amount = transactionFee
			};
			_context.Transactions.Add(feeTransaction);
		}
		else // External transfer
		{
			var isExternalAccountValid = ValidateExternalAccount(transferDto.RecipientAccountId.ToString());

			if (!isExternalAccountValid)
				return NotFound("External recipient account not found or invalid.");

			// Deduct funds and fee from sender's account
			senderAccount.Balance -= (transferDto.Amount + transactionFee);
			_context.Accounts.Update(senderAccount);

			// Log the main transaction
			var mainTransaction = new Transaction
			{
				AccountId = senderAccount.Id,
				Date = DateTime.UtcNow,
				TransactionType = "External Transfer",
				Amount = transferDto.Amount
			};
			_context.Transactions.Add(mainTransaction);
			 
			var feeTransaction = new Transaction
			{
				AccountId = senderAccount.Id,
				Date = DateTime.UtcNow,
				TransactionType = "Transfer Fee",
				Amount = transactionFee
			};
			_context.Transactions.Add(feeTransaction);

			ProcessExternalTransfer(transferDto);
		}

		await _context.SaveChangesAsync();

		return Ok(new { balance = senderAccount.Balance, fee = transactionFee });
	}


	// Mock external account validation
	private bool ValidateExternalAccount(string externalAccountId)
	{
		return !string.IsNullOrEmpty(externalAccountId);
	}

	private void ProcessExternalTransfer(TransferDTO transferDto)
	{
		Console.WriteLine($"Processing external transfer of {transferDto.Amount} to {transferDto.RecipientAccountId}");
	}
}

