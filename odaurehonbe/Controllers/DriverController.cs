using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odaurehonbe.Data;

namespace odaurehonbe.Controllers
{
    [ApiController]
    [Route("api/driver")]
    public class DriverController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public DriverController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("schedule/{accountId}")]
        public async Task<IActionResult> LoadDriverScheduleScreene(int accountId)
        {
            try
            {
                var schedule = await _dbContext.BusDrivers
                    .Where(bd => bd.Driver.AccountID == accountId) 
                    .Include(bd => bd.Bus) 
                    .ThenInclude(bus => bus.BusBusRoutes) 
                    .ThenInclude(bbr => bbr.BusRoute) 
                    .SelectMany(bd => bd.Bus.BusBusRoutes, (bd, bbr) => new
                    {
                        BusId = bd.Bus.BusID,
                        BusRouteId = bbr.BusRoute.BusRouteID,
                        Departure = bbr.BusRoute.DepartPlace,
                        Destination = bbr.BusRoute.ArrivalPlace,
                        DepartureTime = bbr.BusRoute.DepartureTime,
                        Duration = bbr.BusRoute.Duration,
                        LicensePlate = bd.Bus.PlateNum,
                        Status = DateTime.Now > bbr.BusRoute.DepartureTime.ToLocalTime() ? "Đã hoàn thành" : "Chưa khởi hành",
            })
                    .ToListAsync();

                if (!schedule.Any())
                    return NotFound(new { message = "No schedule found for this driver." });

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving schedule.", error = ex.Message });
            }
        }
    }
}
