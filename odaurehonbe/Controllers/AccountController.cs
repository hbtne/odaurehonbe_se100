﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using odaurehonbe.Data;
using odaurehonbe.Models;

namespace odaurehonbe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;
         public class LoyaltyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoyaltyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{customerId}/loyalty")]
        public async Task<IActionResult> GetLoyaltyInfo(int customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountID == customerId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            var discount = CalculateDiscount(customer.LoyaltyPoints);

            return Ok(new
            {
                LoyaltyPoints = customer.LoyaltyPoints,
                Discount = discount * 100
            });
        }

        [HttpPost("{customerId}/loyalty/add")]
        public async Task<IActionResult> AddLoyaltyPoints(int customerId, [FromBody] int points)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountID == customerId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            customer.LoyaltyPoints += points;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Points added successfully.", LoyaltyPoints = customer.LoyaltyPoints });
        }

        private decimal CalculateDiscount(int loyaltyPoints)
        {
            if (loyaltyPoints < 100)
                return 0;
            else if (loyaltyPoints < 400)
                return 0.05m; 
            else if (loyaltyPoints < 800)
                return 0.10m;
            else if (loyaltyPoints < 1000)
                return 0.15m;
            else
                return 0.20m; 
        }

        [HttpPost("{customerId}/apply-first-purchase")]
        public async Task<IActionResult> ApplyFirstPurchaseDiscount(int customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountID == customerId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            if (customer.LoyaltyPoints > 0)
            {
                return BadRequest("First purchase discount already used.");
            }

            var discount = 0.15m; 
            var maxDiscount = 500m;

            return Ok(new { DiscountRate = discount, MaxDiscount = maxDiscount });
        }
    }

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Driver)
                .Include(a => a.TicketClerk)
                .ToListAsync();

            var result = accounts.Select(account =>
            {
                switch (account.UserType)
                {
                    case "Driver":
                        var driver = account.Driver;
                        return driver != null ? new AccountDto
                        {
                            AccountID = account.AccountID,
                            UserName = account.UserName,
                            Status = account.Status,
                            UserType = account.UserType,
                            Password = account.Password,
                            Name = driver.Name,
                            Gender = driver.Gender,
                            PhoneNumber = driver.PhoneNumber,
                            LicenseNumber = driver.LicenseNumber
                        } : null;

                    case "TicketClerk":
                        var clerk = account.TicketClerk;
                        return clerk != null ? new AccountDto
                        {
                            AccountID = account.AccountID,
                            UserName = account.UserName,
                            Status = account.Status,
                            Name = clerk.Name,
                            UserType = account.UserType,
                            Password = account.Password,
                            Gender = clerk.Gender,
                            PhoneNumber = clerk.PhoneNumber,
                            HireDate = clerk.HireDate
                        } : null;

                    case "Customer":
                        var customer = account.Customer;
                        return customer != null ? new AccountDto
                        {
                            AccountID = account.AccountID,
                            UserName = account.UserName,
                            Status = account.Status,
                            Name = customer.Name,
                            UserType = account.UserType,
                            Password = account.Password,
                            Gender = customer.Gender,
                            PhoneNumber = customer.PhoneNumber,
                            Address = customer.Address,
                            LoyaltyPoints = customer.LoyaltyPoints 
                        } : null;

                    default:
                        return null;
                }
            }).Where(a => a != null).ToList();

            return Ok(result);
        }

 [HttpGet("{id}")]
public async Task<IActionResult> GetAccountById(int id)
{
    var account = await _context.Accounts
        .Include(a => a.Customer)
        .Include(a => a.Driver)
        .Include(a => a.TicketClerk)
        .FirstOrDefaultAsync(a => a.AccountID == id);

    if (account == null)
    {
        return NotFound("Account not found");
    }

    var result = account.UserType switch
    {
        "Driver" when account.Driver != null => new AccountDto
        {
            AccountID = account.AccountID,
            UserName = account.UserName,
            Status = account.Status,
            UserType = account.UserType,
            Password = account.Password,
            Name = account.Driver.Name,
            Gender = account.Driver.Gender,
            PhoneNumber = account.Driver.PhoneNumber,
            LicenseNumber = account.Driver.LicenseNumber
        },
        "TicketClerk" when account.TicketClerk != null => new AccountDto
        {
            AccountID = account.AccountID,
            UserName = account.UserName,
            Status = account.Status,
            UserType = account.UserType,
            Password = account.Password,
            Name = account.TicketClerk.Name,
            Gender = account.TicketClerk.Gender,
            PhoneNumber = account.TicketClerk.PhoneNumber,
            HireDate = account.TicketClerk.HireDate
        },
        "Customer" when account.Customer != null => new AccountDto
        {
            AccountID = account.AccountID,
            UserName = account.UserName,
            Status = account.Status,
            Name = account.Customer.Name,
            Gender = account.Customer.Gender,
            UserType = account.UserType,
            Password = account.Password,
            PhoneNumber = account.Customer.PhoneNumber,
            Address = account.Customer.Address
        },
        _ => null
    };

    if (result == null)
    {
        return NotFound("Account details could not be found.");
    }

    return Ok(result);
}




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountDto accountDto)
        {
            if (id != accountDto.AccountID)
            {
                return BadRequest("Account ID mismatch");
            }

            var account = await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Driver)
                .Include(a => a.TicketClerk)
                .FirstOrDefaultAsync(a => a.AccountID == id);

            if (account == null)
            {
                return NotFound("Account not found");
            }

            account.UserName = accountDto.UserName;
            account.Status = accountDto.Status;
            account.Password = accountDto.Password; 

            switch (accountDto.UserType)
            {
                case "Customer":
                    if (account.Customer == null)
                    {
                        account.Customer = new Customer();
                    }
                    account.Customer.Name = accountDto.Name;
                    account.Customer.Gender = accountDto.Gender;
                    account.Customer.PhoneNumber = accountDto.PhoneNumber;
                    account.Customer.Address = accountDto.Address;
                    break;

                case "Driver":
                    if (account.Driver == null)
                    {
                        account.Driver = new Driver();
                    }
                    account.Driver.Name = accountDto.Name;
                    account.Driver.Gender = accountDto.Gender;
                    account.Driver.PhoneNumber = accountDto.PhoneNumber;
                    account.Driver.LicenseNumber = accountDto.LicenseNumber;
                    break;

                case "TicketClerk":
                    if (account.TicketClerk == null)
                    {
                        account.TicketClerk = new TicketClerk();
                    }
                    account.TicketClerk.Name = accountDto.Name;
                    account.TicketClerk.Gender = accountDto.Gender;
                    account.TicketClerk.PhoneNumber = accountDto.PhoneNumber;
                    account.TicketClerk.HireDate = accountDto.HireDate.Value;
                    break;

                default:
                    return BadRequest("Invalid user type");
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating account with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountDto accountDto)
        {
            try
            {
                if (string.IsNullOrEmpty(accountDto.UserType))
                {
                    return BadRequest("UserType is required.");
                }

                if (string.IsNullOrEmpty(accountDto.Password))
                {
                    return BadRequest("Password is required.");
                }

                if (string.IsNullOrEmpty(accountDto.Name) && accountDto.UserType != "TicketClerk")
                {
                    return BadRequest("Name is required for the selected user type.");
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

                if (accountDto.UserType == "Driver")
                {
                    var driver = new Driver
                    {
                        Name = accountDto.Name,
                        Gender = accountDto.Gender,
                        PhoneNumber = accountDto.PhoneNumber,
                        LicenseNumber = accountDto.LicenseNumber
                    };
                    account.Driver = driver;
                    _context.Accounts.Add(account);
                    _context.Drivers.Add(driver);
            
                }
                else if (accountDto.UserType == "TicketClerk")
                {
                    var ticketClerk = new TicketClerk
                    {
                        Name = accountDto.Name,
                        Gender = accountDto.Gender,
                        PhoneNumber = accountDto.PhoneNumber,
                        HireDate = accountDto.HireDate.Value 
                    };
                    account.TicketClerk = ticketClerk;
                    _context.Accounts.Add(account);

                    _context.TicketClerks.Add(ticketClerk);
                }
                else if (accountDto.UserType == "Customer")
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
                    _context.Accounts.Add(account);
                    _context.Customers.Add(customer);
          
                }
                else
                {
                    return BadRequest("Invalid UserType.");
                }

             
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateAccount), new { id = account.AccountID }, accountDto); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);  
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Driver)
                .Include(a => a.TicketClerk)
                .FirstOrDefaultAsync(a => a.AccountID == id);

            if (account == null)
            {
                return NotFound("Account not found");
            }

            if (account.Customer != null)
            {
                _context.Customers.Remove(account.Customer);
            }
            if (account.Driver != null)
            {
                _context.Drivers.Remove(account.Driver);
            }
            if (account.TicketClerk != null)
            {
                _context.TicketClerks.Remove(account.TicketClerk);
            }

            _context.Accounts.Remove(account);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while deleting the account");
            }
        }


        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountID == id);
        }
    }
}
