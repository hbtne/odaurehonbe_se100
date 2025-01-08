using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odaurehonbe.Data;
using System.Linq;
using System.Threading.Tasks;

namespace odaurehonbe.Controllers
{
    [Route("api/promotion")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PromotionController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetPromotions()
        {
            var promotions = await _context.Promotions.ToListAsync();
            return Ok(promotions);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody] Promotion promotion)
        {
            promotion.StartDate = DateTime.SpecifyKind(promotion.StartDate, DateTimeKind.Utc);
            promotion.EndDate = DateTime.SpecifyKind(promotion.EndDate, DateTimeKind.Utc);

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            return Ok(promotion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromotion(int id, [FromBody] Promotion promotion)
        {
            if (id != promotion.PromoID)
                return BadRequest();

            promotion.StartDate = DateTime.SpecifyKind(promotion.StartDate, DateTimeKind.Utc);
            promotion.EndDate = DateTime.SpecifyKind(promotion.EndDate, DateTimeKind.Utc);

            _context.Entry(promotion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Promotions.Any(p => p.PromoID == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
