using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odaurehonbe.Data;
using odaurehonbe.Models;
using OfficeOpenXml;

namespace odaurehonbe.Controllers
{
    [ApiController]
    [Route("api/busroute")]
    public class BusRouteController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public BusRouteController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]

        public async Task<IActionResult> GetBusRoutes([FromQuery] int? searchQuery)
        {
            var query = _dbContext.BusRoutes.AsQueryable();

            if (searchQuery!=null)
            {
                query = query.Where(b =>b.BusRouteID.Equals(searchQuery));
            }

            var results = await query.ToListAsync();

            if (results.Count == 0)
            {
                return NotFound("No bus routes found matching the criteria.");
            }

            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusRoute([FromBody] BusRoute newRoute)
        {
            if (newRoute == null) return BadRequest("Invalid data.");

            _dbContext.BusRoutes.Add(newRoute);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBusRoutes), new { id = newRoute.BusRouteID }, newRoute);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBusRoute(int id, [FromBody] BusRoute updatedRoute)
        {
            Console.WriteLine($"Received busRouteId: {id}");

            var existingRoute = await _dbContext.BusRoutes.FindAsync(id);
            if (existingRoute == null) return NotFound();

            existingRoute.BusRouteID = updatedRoute.BusRouteID;
            existingRoute.DepartPlace = updatedRoute.DepartPlace;
            existingRoute.ArrivalPlace = updatedRoute.ArrivalPlace;
            existingRoute.DepartStation = updatedRoute.DepartStation;
            existingRoute.ArrivalStation = updatedRoute.ArrivalStation;
            existingRoute.DepartureTime = updatedRoute.DepartureTime;
            existingRoute.PricePerSeat = updatedRoute.PricePerSeat;
            existingRoute.PricePerSeatVip = updatedRoute.PricePerSeatVip;
            existingRoute.Duration = updatedRoute.Duration;

            await _dbContext.SaveChangesAsync();

            return NoContent(); 
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusRoute(int id)
        {
            var busRoute = await _dbContext.BusRoutes.FindAsync(id);
            if (busRoute == null) return NotFound();

            _dbContext.BusRoutes.Remove(busRoute);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("bus-route-report")]
        public async Task<IActionResult> GetBusRouteReport([FromQuery] string? id, [FromQuery] string? departure, [FromQuery] string? destination, [FromQuery] DateTime? date)
        {
            try
            {
                var query = _dbContext.BusRoutes
                    .Include(br => br.BusBusRoutes)
                        .ThenInclude(bbr => bbr.Bus)
                    .Include(br => br.BusBusRoutes)
                        .ThenInclude(bbr => bbr.Seats)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(id))
                {
                    query = query.Where(br => br.BusRouteID.ToString().Contains(id));
                }

                if (!string.IsNullOrEmpty(departure))
                {
                    query = query.Where(br => br.DepartPlace.Contains(departure));
                }

                if (!string.IsNullOrEmpty(destination))
                {
                    query = query.Where(br => br.ArrivalPlace.Contains(destination));
                }
       
                if (date.HasValue)
                {
                    query = query.Where(br => br.DepartureTime.Date == date.Value.ToUniversalTime().Date);
                }

                var report = await query
                    .Select(br => new
                    {
                        BusRouteId = br.BusRouteID,
                        Departure = br.DepartPlace,
                        Destination = br.ArrivalPlace,
                        DepartureTime = br.DepartureTime,
                        Duration = br.Duration,
                        BusIds = string.Join(", ", br.BusBusRoutes.Select(bbr => bbr.Bus.BusID)),
                        TotalTickets = br.BusBusRoutes.Sum(bbr => bbr.Bus.NumSeat),
                        SoldTickets = br.BusBusRoutes.Sum(bbr => bbr.Seats.Count(seat => seat.IsBooked)),
                        TotalRevenue = br.BusBusRoutes.Sum(bbr =>
                            bbr.Seats.Where(seat => seat.IsBooked).Count() *
                            (bbr.Bus.Type == "VIP" ? br.PricePerSeatVip : br.PricePerSeat)
                        )
                    })
                    .ToListAsync();

                return Ok(report);
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
        [HttpGet("bus-route-report-export")]
        public async Task<IActionResult> ExportBusRouteReport([FromQuery] string? id, [FromQuery] string? departure, [FromQuery] string? destination, [FromQuery] DateTime? date)
        {
            try
            {
                var query = _dbContext.BusRoutes
                    .Include(br => br.BusBusRoutes)
                        .ThenInclude(bbr => bbr.Bus)
                    .Include(br => br.BusBusRoutes)
                        .ThenInclude(bbr => bbr.Seats)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(id))
                {
                    query = query.Where(br => br.BusRouteID.ToString().Contains(id));
                }

                if (!string.IsNullOrEmpty(departure))
                {
                    query = query.Where(br => br.DepartPlace.Contains(departure));
                }

                if (!string.IsNullOrEmpty(destination))
                {
                    query = query.Where(br => br.ArrivalPlace.Contains(destination));
                }

                if (date.HasValue)
                {
                    query = query.Where(br => br.DepartureTime.Date == date.Value.Date);
                }

                var reportData = await query
                    .Select(br => new
                    {
                        BusRouteId = br.BusRouteID,
                        Departure = br.DepartPlace,
                        Destination = br.ArrivalPlace,
                        DepartureTime = br.DepartureTime,
                        Duration = br.Duration,
                        BusIds = string.Join(", ", br.BusBusRoutes.Select(bbr => bbr.Bus.BusID)),
                        TotalTickets = br.BusBusRoutes.Sum(bbr => bbr.Bus.NumSeat),
                        SoldTickets = br.BusBusRoutes.Sum(bbr => bbr.Seats.Count(seat => seat.IsBooked)),
                        TotalRevenue = br.BusBusRoutes.Sum(bbr =>
                            bbr.Seats.Where(seat => seat.IsBooked).Count() *
                            (bbr.Bus.Type == "VIP" ? br.PricePerSeatVip : br.PricePerSeat)
                        )
                    })
                    .ToListAsync();

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("BusRouteReport");

                // Header
                worksheet.Cells[1, 1].Value = "Bus Route ID";
                worksheet.Cells[1, 2].Value = "Departure";
                worksheet.Cells[1, 3].Value = "Destination";
                worksheet.Cells[1, 4].Value = "Departure Time";
                worksheet.Cells[1, 5].Value = "Duration";
                worksheet.Cells[1, 6].Value = "Bus IDs";
                worksheet.Cells[1, 7].Value = "Total Tickets";
                worksheet.Cells[1, 8].Value = "Sold Tickets";
                worksheet.Cells[1, 9].Value = "Total Revenue";

                // Data
                for (int i = 0; i < reportData.Count; i++)
                {
                    var row = i + 2;
                    var data = reportData[i];
                    worksheet.Cells[row, 1].Value = data.BusRouteId;
                    worksheet.Cells[row, 2].Value = data.Departure;
                    worksheet.Cells[row, 3].Value = data.Destination;
                    worksheet.Cells[row, 4].Value = data.DepartureTime.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[row, 5].Value = data.Duration;
                    worksheet.Cells[row, 6].Value = data.BusIds;
                    worksheet.Cells[row, 7].Value = data.TotalTickets;
                    worksheet.Cells[row, 8].Value = data.SoldTickets;
                    worksheet.Cells[row, 9].Value = data.TotalRevenue;
                }

                worksheet.Cells.AutoFitColumns();

                var fileBytes = package.GetAsByteArray();

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BusRouteReport.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occurred while exporting the report.",
                    Error = ex.Message
                });
            }
        }



    }

}
