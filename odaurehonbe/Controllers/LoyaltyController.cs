using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odaurehonbe.Data;
using odaurehonbe.Models;
using System.Linq;
using System.Threading.Tasks;

namespace odaurehonbe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            if (points <= 0)
            {
                return BadRequest("Points must be greater than zero.");
            }

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
            var maxDiscount = 500m; // 500k

            return Ok(new { DiscountRate = discount, MaxDiscount = maxDiscount });
        }

        [HttpPost("{customerId}/apply-discount")]
        public async Task<IActionResult> ApplyCombinedDiscount(int customerId, [FromBody] int promoId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountID == customerId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            var loyaltyDiscount = CalculateDiscount(customer.LoyaltyPoints);

            var promo = await _context.Promotions.FirstOrDefaultAsync(p => p.PromoID == promoId);
            if (promo == null || promo.StartDate > DateTime.UtcNow || promo.EndDate < DateTime.UtcNow)
            {
                return NotFound("Promotion not found or expired.");
            }

            var promoDiscount = promo.DiscountPercent / 100m;
            var totalDiscount = loyaltyDiscount + promoDiscount;

            if (totalDiscount > 0.20m)
            {
                totalDiscount = 0.20m; 
            }

            return Ok(new
            {
                LoyaltyDiscount = loyaltyDiscount * 100, 
                PromoDiscount = promoDiscount * 100, 
                TotalDiscount = totalDiscount * 100 
            });
        }
    }
}
