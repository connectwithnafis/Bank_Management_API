using BankManagementAPI.Data;
using BankManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly BankDbContext _context;
		private readonly IConfiguration _configuration;

		public UserController(BankDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}


		// GET: api/User/profile
		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			// Retrieve user from the token's claims
			var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User not authenticated.");

			var user = await _context.Users.FindAsync(int.Parse(userId));


			if (user == null)
				return NotFound("User not found.");

			return Ok(new
			{
				username = user.Username,
				role = user.Role,
				userId = user.Id
			});
		}

		[HttpGet("balance")]
		public async Task<IActionResult> CheckBalance()
		{
			var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userId))
				return Unauthorized("User not authenticated.");

			var user = await _context.Users.FindAsync(int.Parse(userId));

			var account = await _context.Accounts.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();
			var accountBalance = account?.Balance ?? 0; 

			if (user == null)
				return NotFound("User not found.");

			return Ok(new
			{
				balance = accountBalance
			});
		}

		// DELETE: api/User/delete/{userId}
		[HttpDelete("delete/{userId}")]
		public async Task<IActionResult> DeleteUser(int userId)
		{
			try
			{
				var user = await _context.Users.FindAsync(userId);
				if (user == null)
				{
					return NotFound("User not found.");
				}

				var account = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId);
				if (account != null)
				{
					if (account.Balance > 0)
					{
						return BadRequest("User has a balance. Please ensure balance is cleared before deletion.");
					}
					else
					{
						_context.Accounts.Remove(account); 
					}
				}

				_context.Users.Remove(user);
				await _context.SaveChangesAsync();

				return Ok("User deleted successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
