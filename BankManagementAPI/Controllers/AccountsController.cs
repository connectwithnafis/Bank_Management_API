using BankManagementAPI.Data;
using BankManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using BankManagementAPI.DTOs;

namespace BankManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class AccountsController : ControllerBase
{
	private readonly BankDbContext _context;

	public AccountsController(BankDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
	{
		return await _context.Accounts.ToListAsync();
	}

	// GET: api/Accounts/{id}
	[HttpGet("{id}")]
	public async Task<ActionResult<Account>> GetAccountById(int id)
	{
		var account = await _context.Accounts.FindAsync(id);

		if (account == null)
		{
			return NotFound();
		}

		return account;
	}

	// GET: api/Accounts/User/{userId}
	[HttpGet("User/{userId}")]
	public async Task<ActionResult<IEnumerable<Account>>> GetAccountsByUserId(int userId)
	{
		var accounts = await _context.Accounts.Where(a => a.UserId == userId).ToListAsync();

		if (!accounts.Any())
		{
			return NotFound("No accounts found for this user.");
		}

		return Ok(accounts);
	}

	[HttpGet("{id}/Details")]
	public async Task<ActionResult<AccountDTO>> GetAccountDetails(int id)
	{
		var account = await _context.Accounts
			.Include(x => x.Transactions)
			.FirstOrDefaultAsync(a => a.UserId == id);

		if (account == null)
		{
			return NotFound("Account not found.");
		}

		// Map to DTO and return
		var accountDto = new AccountDTO
		{
			Id = account.Id,
			AccountType = account.AccountType,
			Balance = account.Balance,
			Transactions = account.Transactions.Select(t => new TransactionDTO
			{
				Id = t.Id,
				Date = t.Date,
				TransactionType = t.TransactionType,
				Amount = t.Amount
			}).ToList()
		};

		return Ok(accountDto);
	}

	[HttpGet("AllAccounts")]
	public async Task<ActionResult<IEnumerable<object>>> GetAllAccounts()
	{
		var accountsWithUsers = await _context.Accounts
			.Join(
				_context.Users,
				account => account.UserId,
				user => user.Id,
				(account, user) => new
				{
					UserId = user.Id,
					Username = user.Username,
					Role = user.Role,
					AccountType = account.AccountType,
					Balance = account.Balance
					//,Password = user.Password // Include password (consider hashing or excluding in real scenarios)
				}
			)
			.ToListAsync();

		return Ok(accountsWithUsers);
	}

	[HttpGet("ApproveAccounts")]
	public async Task<ActionResult<IEnumerable<object>>> GetPendingAccounts()
	{
		var pendingAccounts = await _context.Users
			.Where(user => !_context.Accounts.Any(account => account.UserId == user.Id))
			.ToListAsync();

		return Ok(pendingAccounts);
	}

	[HttpPost("ApprovePendingAccounts/{userId}")]
	public async Task<ActionResult<string>> ApprovePendingAccount(int userId)
	{
		try
		{
			var accountRole = await _context.Users
			.Where(user => user.Id == userId).Select(user => user.Role) .FirstOrDefaultAsync();

			var newAccount = new Account
			{
				UserId = userId,
				AccountType = accountRole,
				Balance = 0
			};

			_context.Accounts.Add(newAccount);

			await _context.SaveChangesAsync();

			return Ok("Account approved successfully.");
		}
		catch (Exception ex)
		{
			return StatusCode(500, "An error occurred while approving the account.");
		}
	}


	// GET: api/Accounts/GetUserById/{id}
	[HttpGet("GetUserById/{id}")]
	public async Task<IActionResult> GetUserById(int id)
	{
		var user = await _context.Users
			.Include(u => u.Accounts)          // Include the Accounts for the user
				.ThenInclude(a => a.Transactions) // Include the Transactions for each Account
			.FirstOrDefaultAsync(u => u.Id == id);

		if (user == null)
		{
			return NotFound("User not found.");
		}

		// Map user data to a DTO if needed (recommended)
		var userDto = new EditUserDTO
		{
			Username = user.Username,
			Role = user.Role,
			Accounts = user.Accounts.Select(a => new EditAccountDTO
			{
				Id = a.Id,
				AccountType = a.AccountType,
				Balance = a.Balance,
				Transactions = a.Transactions.Select(t => new EditTransactionDTO
				{
					Id = t.Id,
					TransactionType = t.TransactionType,
					Amount = t.Amount,
					Date = t.Date
				}).ToList()
			}).ToList()
		};

		return Ok(userDto);
	}


	// PUT: api/Accounts/User/{id}
	[HttpPut("EditUser/{id}")]
	public async Task<IActionResult> EditUser(int id, [FromBody] EditUserDTO userDto)
	{
		// Fetch the user from the database
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
		if (user == null)
		{
			return NotFound("User not found.");
		}

		// Update user details
		user.Username = userDto.Username ?? user.Username;
		user.Role = userDto.Role ?? user.Role;
		if (!string.IsNullOrEmpty(userDto.Password))
		{
			user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
		}

		// Save changes to user
		_context.Users.Update(user);

		// If accounts are provided, update accounts
		if (userDto.Accounts != null)
		{
			foreach (var accountDto in userDto.Accounts)
			{
				var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountDto.Id);
				if (account != null)
				{
					account.AccountType = accountDto.AccountType ?? account.AccountType;
					account.Balance = accountDto.Balance ?? account.Balance;

					// If transactions are provided, update transactions
					if (accountDto.Transactions != null)
					{
						foreach (var transactionDto in accountDto.Transactions)
						{
							var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionDto.Id);
							if (transaction != null)
							{
								transaction.TransactionType = transactionDto.TransactionType ?? transaction.TransactionType;
								transaction.Amount = transactionDto.Amount ?? transaction.Amount;
								transaction.Date = transactionDto.Date ?? transaction.Date;

								_context.Transactions.Update(transaction);
							}
							else
							{
								// Add new transaction if it doesn't exist
								_context.Transactions.Add(new Transaction
								{
									AccountId = account.Id,
									TransactionType = transactionDto.TransactionType,
									Amount = transactionDto.Amount ?? 0,
									Date = transactionDto.Date ?? DateTime.UtcNow
								});
							}
						}
					}

					_context.Accounts.Update(account);
				}
				else
				{
					// Add new account if it doesn't exist
					_context.Accounts.Add(new Account
					{
						UserId = user.Id,
						AccountType = accountDto.AccountType,
						Balance = accountDto.Balance ?? 0
					});
				}
			}
		}

		// Save changes to database
		await _context.SaveChangesAsync();

		return Ok("User details updated successfully.");
	}

	// POST: api/Accounts
	[HttpPost]
	public async Task<ActionResult<Account>> PostAccount(Account account)
	{
		// Validate account type (Savings, Checking, Loan)
		if (!new[] { "Savings", "Checking", "Loan" }.Contains(account.AccountType))
		{
			return BadRequest("Invalid account type.");
		}

		_context.Accounts.Add(account);
		await _context.SaveChangesAsync();

		return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
	}

	// PUT: api/Accounts/{id}
	[HttpPut("{id}")]
	public async Task<IActionResult> PutAccount(int id, Account account)
	{
		if (id != account.Id)
		{
			return BadRequest();
		}

		// Validate account type (Savings, Checking, Loan)
		if (!new[] { "Savings", "Checking", "Loan" }.Contains(account.AccountType))
		{
			return BadRequest("Invalid account type.");
		}

		_context.Entry(account).State = EntityState.Modified;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!_context.Accounts.Any(a => a.Id == id))
			{
				return NotFound();
			}

			throw;
		}

		return NoContent();
	}

	// DELETE: api/Accounts/{id}
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteAccount(int id)
	{
		var account = await _context.Accounts.FindAsync(id);
		if (account == null)
		{
			return NotFound();
		}

		_context.Accounts.Remove(account);
		await _context.SaveChangesAsync();

		return NoContent();
	}
}
