using BankManagementAPI.Data;
using BankManagementAPI.DTOs;
using BankManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankManagementAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class LoansController : ControllerBase
	{
		private readonly BankDbContext _context;

		public LoansController(BankDbContext context)
		{
			_context = context;
		}

		// POST: api/Loans/apply
		[HttpPost("apply")]
		[Authorize]
		public async Task<IActionResult> ApplyForLoan(LoanDTO loanRequest)
		{
			if (loanRequest == null || loanRequest.LoanAmount <= 0 || loanRequest.LoanTerm <= 0)
				return BadRequest("Invalid loan request.");

			var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == int.Parse(userId));

			var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == user.Id);

			if (account == null)
				return NotFound("Account not found.");

			var loan = new Loan
			{
				FullName = user.Username,
				UserId = account.UserId,
				AccountId = account.Id,
				LoanType = loanRequest.LoanType,
				LoanAmount = loanRequest.LoanAmount,
				LoanTerm = loanRequest.LoanTerm,
				InterestRate = loanRequest.InterestRate,
				Status = "Pending",
				ApplicationDate = DateTime.UtcNow
			};

			_context.Loans.Add(loan);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Loan application submitted successfully!", loanId = loan.Id });
		}

		// GET: api/Loans/pending
		[HttpGet("pending")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetPendingLoans()
		{
			var pendingLoans = await _context.Loans
				.Where(l => l.Status == "Pending")
				.ToListAsync();

			return Ok(pendingLoans);
		}

		// POST: api/Loans/approve/{id}
		[HttpPost("approve/{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ApproveLoan(int id)
		{
			var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == id);

			if (loan == null)
				return NotFound("Loan not found.");

			if (loan.Status != "Pending")
				return BadRequest("Only pending loans can be approved.");

			loan.Status = "Approved";
			loan.ApprovalDate = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			return Ok(new { message = "Loan approved successfully." });
		}

		// POST: api/Loans/reject/{id}
		[HttpPost("reject/{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> RejectLoan(int id)
		{
			var loan = await _context.Loans.FirstOrDefaultAsync(l => l.Id == id);

			if (loan == null)
				return NotFound("Loan not found.");

			if (loan.Status != "Pending")
				return BadRequest("Only pending loans can be rejected.");

			loan.Status = "Rejected";
			await _context.SaveChangesAsync();

			return Ok(new { message = "Loan rejected successfully." });
		}

		// GET: api/Loans/user
		[HttpGet("user")]
		[Authorize]
		public async Task<IActionResult> GetUserLoans()
		{
			var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");

			var userLoans = await _context.Loans
				.Where(l => l.UserId == userId)
				.Include(l => l.Account)
				.ToListAsync();

			return Ok(userLoans);
		}
	}
}
