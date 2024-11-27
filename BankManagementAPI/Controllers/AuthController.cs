using BankManagementAPI.Data;
using BankManagementAPI.DTOs;
using BankManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly BankDbContext _context;
	private readonly IConfiguration _configuration;

	public AuthController(BankDbContext context, IConfiguration configuration)
	{
		_context = context;
		_configuration = configuration;
	}

	[HttpPost("register")]
	public async Task<ActionResult> Register(UserRegistrationDTO registration)
	{
		if (await _context.Users.AnyAsync(u => u.Username == registration.Username))
		{
			return BadRequest("Username is already taken.");
		}

		var user = new User
		{
			Username = registration.Username,
			Password = BCrypt.Net.BCrypt.HashPassword(registration.Password), // Hash password
			Role = registration.Role ?? "Customer"
		};

		_context.Users.Add(user);
		await _context.SaveChangesAsync();

		return Ok("Registration successful.");
	}
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
	{
		// Validate the user credentials
		var user = _context.Users.SingleOrDefault(u => u.Username == loginRequest.Username);

		if (user == null || !VerifyPassword(user.Password, loginRequest.Password))
		{
			return Unauthorized(new { message = "Invalid username or password" });
		}

		// Generate JWT token
		var token = GenerateJwtToken(user);

		return Ok(new { Token = token });
	}

	private bool VerifyPassword(string storedPassword, string enteredPassword)
	{
		// Ideally, you should compare hashed passwords, not plain text passwords
		//return storedPassword == enteredPassword;
		return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPassword);
	}

	private string GenerateJwtToken(User user)
	{
		var claims = new[]
		{
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Role, user.Role),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
			};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: _configuration["JwtSettings:Issuer"],
			audience: _configuration["JwtSettings:Audience"],
			claims: claims,
			expires: DateTime.Now.AddDays(1),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
	public class LoginRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
