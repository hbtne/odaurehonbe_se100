
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odaurehonbe.Data;
using odaurehonbe.Models;

namespace odaurehonbe.Controllers
{
    [Route("api/bus")]
    [ApiController]
    public class BusesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BusesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetBuses([FromQuery] string? filterType, [FromQuery] string? searchQuery)
        {
            var query = _context.Buses.AsQueryable();

            if (!string.IsNullOrEmpty(filterType))
            {
                query = query.Where(bus => bus.Type.ToUpper() == filterType.ToUpper());
            }


            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (int.TryParse(searchQuery, out int busID))
                {
                    query = query.Where(bus => bus.BusID == busID);  
                }

            }

            var result = query.Include(bus => bus.BusBusRoutes)
                              .Include(bus => bus.BusDrivers)
                              .ToList();

            return Ok(result);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Bus>> GetBus(int id)
        {
            var bus = await _context.Buses.Include(b => b.BusDrivers)
                                          .Include(b => b.BusBusRoutes)
                                          .FirstOrDefaultAsync(b => b.BusID == id);

            if (bus == null)
            {
                return NotFound();
            }

            return bus;
        }

        [HttpPost]
        public async Task<IActionResult> AddBus([FromBody] BusRequestModel busRequest)
        {
            if (busRequest == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var busRouteIds = busRequest.BusRouteIds
                .Split(',')
                .Select(id => int.Parse(id.Trim()))
                .ToList();

            var driverIds = busRequest.DriverIds
                .Split(',')
                .Select(id => int.Parse(id.Trim()))
                .ToList();

            var existingBus = await _context.Buses.FindAsync(busRequest.BusID);
            if (existingBus != null)
            {
                return BadRequest("BusID đã tồn tại.");
            }

            var newBus = new Bus
            {
                BusID = busRequest.BusID,
                NumSeat = busRequest.NumSeat,
                PlateNum = busRequest.PlateNum,
                Type = busRequest.Type,
            };

            _context.Buses.Add(newBus);

            await _context.SaveChangesAsync();

            foreach (var routeId in busRouteIds)
            {
                var busRoute = await _context.BusRoutes.FindAsync(routeId);
                if (busRoute == null)
                {
                    return BadRequest($"BusRoute với ID {routeId} không tồn tại.");
                }

                var busBusRoute = new BusBusRoute
                {
                    BusID = busRequest.BusID,
                    BusRouteID = routeId
                };
                _context.BusBusRoutes.Add(busBusRoute);
                await _context.SaveChangesAsync(); 

                for (int i = 1; i <= busRequest.NumSeat; i++)
                {
                    var seat = new Seat
                    {
                        SeatNumber = $"S{i}",
                        IsBooked = false,
                        BusBusRouteID = busBusRoute.BusBusRouteID 
                    };
                    _context.Seats.Add(seat);
                }
            }

            foreach (var driverId in driverIds)
            {
                var driver = await _context.Drivers.FindAsync(driverId);
                if (driver == null)
                {
                    return BadRequest($"Driver với ID {driverId} không tồn tại.");
                }

                _context.BusDrivers.Add(new BusDriver { BusID = busRequest.BusID, DriverID = driverId });
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBus), new { id = busRequest.BusID }, busRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBus(int id, BusRequestModel busRequest)
        {
            if (id != busRequest.BusID)
            {
                return BadRequest("Bus ID mismatch.");
            }

            var existingBus = await _context.Buses
                                             .Include(b => b.BusDrivers)
                                             .Include(b => b.BusBusRoutes)
                                             .ThenInclude(bbr => bbr.Seats) 
                                             .FirstOrDefaultAsync(b => b.BusID == id);

            if (existingBus == null)
            {
                return NotFound("Bus not found.");
            }

            existingBus.NumSeat = busRequest.NumSeat;
            existingBus.PlateNum = busRequest.PlateNum;
            existingBus.Type = busRequest.Type;

            var existingBusRouteIds = existingBus.BusBusRoutes.Select(br => br.BusRouteID).ToList();
            var busRouteIds = busRequest.BusRouteIds.Split(',').Select(int.Parse).ToList();

            foreach (var busBusRoute in existingBus.BusBusRoutes)
            {
                if (!busRouteIds.Contains(busBusRoute.BusRouteID))
                {
                    _context.Seats.RemoveRange(busBusRoute.Seats);

                    _context.BusBusRoutes.Remove(busBusRoute);
                }
            }

            foreach (var routeId in busRouteIds)
            {
                if (!existingBusRouteIds.Contains(routeId))
                {
                    var busRoute = await _context.BusRoutes.FindAsync(routeId);
                    if (busRoute != null)
                    {
                        var newBusBusRoute = new BusBusRoute
                        {
                            BusID = existingBus.BusID,
                            BusRouteID = routeId
                        };

                        _context.BusBusRoutes.Add(newBusBusRoute);
                        await _context.SaveChangesAsync(); 

                        for (int i = 1; i <= busRequest.NumSeat; i++)
                        {
                            var seat = new Seat
                            {
                                SeatNumber = $"S{i}",
                                IsBooked = false,
                                BusBusRouteID = newBusBusRoute.BusBusRouteID
                            };
                            _context.Seats.Add(seat);
                        }
                    }
                }
            }

            var existingDriverIds = existingBus.BusDrivers.Select(bd => bd.DriverID).ToList();
            var driverIds = busRequest.DriverIds.Split(',').Select(int.Parse).ToList();

            foreach (var busDriver in existingBus.BusDrivers)
            {
                if (!driverIds.Contains(busDriver.DriverID))
                {
                    _context.BusDrivers.Remove(busDriver);
                }
            }

            foreach (var driverId in driverIds)
            {
                if (!existingDriverIds.Contains(driverId))
                {
                    var driver = await _context.Drivers.FindAsync(driverId);
                    if (driver != null)
                    {
                        existingBus.BusDrivers.Add(new BusDriver { BusID = existingBus.BusID, DriverID = driverId });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var bus = await _context.Buses
                                    .Include(b => b.BusDrivers)
                                    .Include(b => b.BusBusRoutes)
                                        .ThenInclude(bbr => bbr.Seats) 
                                    .FirstOrDefaultAsync(b => b.BusID == id);

            if (bus == null)
            {
                return NotFound("Bus not found.");
            }

            foreach (var busBusRoute in bus.BusBusRoutes)
            {
                _context.Seats.RemoveRange(busBusRoute.Seats); 
            }

            _context.BusBusRoutes.RemoveRange(bus.BusBusRoutes);
            _context.BusDrivers.RemoveRange(bus.BusDrivers);

            _context.Buses.Remove(bus);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
