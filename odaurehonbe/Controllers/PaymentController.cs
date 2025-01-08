using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odaurehonbe.Data;

namespace odaurehonbe.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PaymentController (AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaymentDetails(int busBusRouteId, int customerId)
        {
          
            var busBusRoute = await _context.BusBusRoutes
                .Include(b => b.BusRoute)
                .Where(b => b.BusBusRouteID == busBusRouteId)
                .Select(b => new { b.BusRoute.ArrivalPlace, b.BusRoute.DepartPlace })
                .FirstOrDefaultAsync();

            if (busBusRoute == null)
            {
                return NotFound("Bus route not found.");
            }

            var customer = await _context.Customers
                   .Include(c => c.Account)
                   .Where(c => c.AccountID == customerId)
                   .Select(c => new { c.Name, c.Account.UserName, c.PhoneNumber })
                   .FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            var paymentDetails = new
            {
                ArrivalPlace = busBusRoute.ArrivalPlace,
                DepartPlace = busBusRoute.DepartPlace,
                CustomerName = customer.Name,
                CustomerEmail = customer.UserName,
                CustomerPhoneNumber = customer.PhoneNumber
            };

            return Ok(paymentDetails);
        }
        [HttpGet("promo")]
        public async Task<IActionResult> GetPromoDetails(int promoId)
        {
            var now = DateTime.UtcNow; 

            var promo = await _context.Promotions
                .Where(p => p.PromoID == promoId && p.StartDate <= now && p.EndDate >= now)
                .Select(p => new { p.DiscountPercent })
                .FirstOrDefaultAsync();

            if (promo == null)
            {
                return NotFound("Promo not found or not valid.");
            }

            var promoDetails = new
            {
                DiscountPercentage = promo.DiscountPercent
            };

            return Ok(promoDetails);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(Payment payment, int id)
        {
            if (payment == null || payment.Tickets == null || !payment.Tickets.Any())
            {
                return BadRequest("Invalid payment details.");
            }

            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountID == id);
                if (customer != null)
                {
                    payment.CustomerID = customer.AccountID;
                }
                else
                {
                    var staff = await _context.TicketClerks.FirstOrDefaultAsync(s => s.AccountID == id);
                    if (staff != null)
                    {
                        payment.StaffID = staff.AccountID;
                    }
                    else
                    {
                        return NotFound("ID not associated with any Customer or Staff.");
                    }
                }

                payment.PaymentTime = DateTime.UtcNow;
                _context.Payments.Add(payment);

                foreach (var ticket in payment.Tickets)
                {
                    var ticketEntity = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketID == ticket.TicketID);
                    if (ticketEntity != null)
                    {
                        ticketEntity.Status = "Đã thanh toán";
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Payment created successfully.", paymentId = payment.PaymentID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the payment.", error = ex.Message });
            }
        }

    }
}

