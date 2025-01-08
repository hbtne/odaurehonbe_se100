using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odaurehonbe.Data;
using System.Threading.Tasks;
using System.Linq;
using odaurehonbe.Models;

namespace odaurehonbe.Controllers
{
    [Route("api/notificaton")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GenerateNotifications()
        {
            try
            {
                var noti = await _context.Notifications
                    .OrderByDescending(n => n.CreatedAt) 
                    .ToListAsync();

                return Ok(noti);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message
                });
            }
        }







    }


}

