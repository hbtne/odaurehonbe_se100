using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odaurehonbe.Data;
using odaurehonbe.Models;

namespace odaurehonbe.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] AccountDto accountDto)
        {
            if (string.IsNullOrEmpty(accountDto.Name) ||
                string.IsNullOrEmpty(accountDto.PhoneNumber) ||
                string.IsNullOrEmpty(accountDto?.Password) ||
                string.IsNullOrEmpty(accountDto.UserName) ||
                string.IsNullOrEmpty(accountDto.UserType))
            {
                return BadRequest(new { message = "Please fill in all the required fields." });
            }

            if (_dbContext.Accounts.Any(a => a.UserName == accountDto.UserName))
            {
                return BadRequest(new { message = "Username already exists." });
            }
            if (_dbContext.Accounts.Any(a => a.AccountID == accountDto.AccountID))
            {
                return BadRequest(new { message = "CCCD already exists." });
            }
            var account = new Account
            {
                UserName = accountDto.UserName,
                Status = accountDto.Status,
                UserType = accountDto.UserType,
                Password = accountDto.Password
            };

            if (accountDto.AccountID > 0)
            {
                account.AccountID = accountDto.AccountID;
            }

            if (accountDto.UserType == "Customer")
            {
                var customer = new Customer
                {
                    AccountID = accountDto.AccountID,
                    Name = accountDto.Name,
                    Gender = accountDto.Gender,
                    PhoneNumber = accountDto.PhoneNumber,
                    Address = accountDto.Address
                };
                account.Customer = customer;

                _dbContext.Accounts.Add(account);
                _dbContext.Customers.Add(customer);
            }
            else
            {
                return BadRequest("Invalid UserType.");
            }

    
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Registration successful!" });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var account = await _dbContext.Accounts
                .FirstOrDefaultAsync(a => a.UserName == request.Email && a.Password == request.Password);

            if (account == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(new
            {
                message = "Login successful!",
                accountType = account.UserType,
                accountId = account.AccountID 
            });
        }
        [HttpPost("signout")]
        public IActionResult SignOut()
        {
            return Ok(new { message = "Signout successful. Please clear local storage." });
        }
      


    }
}
