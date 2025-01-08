using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odaurehonbe.Data;
using odaurehonbe.Models;

namespace odaurehonbe.Controllers
{
    [Route("api/bookticket")]
    [ApiController]
    public class BookTicketController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookTicketController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("bus-bus-routes")]
        public async Task<IActionResult> GetBusBusRoutes()
        {
            try
            {
                var busBusRoutes = await _context.BusBusRoutes
                    .Include(bbr => bbr.BusRoute)
                    .Include(bbr => bbr.Seats)
                    .Select(bbr => new
                    {
                        bbr.BusBusRouteID,
                        bbr.BusRouteID,
                        DepartPlace = bbr.BusRoute.DepartPlace,
                        ArrivalPlace = bbr.BusRoute.ArrivalPlace,
                        DepartureTime = bbr.BusRoute.DepartureTime,
                        Duration = bbr.BusRoute.Duration,
                        PricePerSeat = bbr.Bus.Type == "VIP"
                            ? bbr.BusRoute.PricePerSeatVip
                            : bbr.BusRoute.PricePerSeat,
                        Bus = new
                        {
                            bbr.BusID,
                            bbr.Bus.NumSeat,
                            bbr.Bus.PlateNum,
                            bbr.Bus.Type
                        },
                        SeatsAvailable = bbr.Seats != null
                            ? bbr.Seats.Count(seat => !seat.IsBooked)
                            : 0,
                        Seats = bbr.Seats != null
                            ? bbr.Seats.Select(seat => new
                            {
                                seat.SeatID,
                                seat.SeatNumber,
                                seat.IsBooked
                            }).ToList()
                            : null
                    })
                    .ToListAsync();

                return Ok(busBusRoutes);
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
       [HttpGet("search")]
        public async Task<IActionResult> SearchBusRoutes(
             [FromQuery] string departPlace,
            [FromQuery] string arrivalPlace,
            [FromQuery] DateTime? departureDate,
            [FromQuery] DateTime? returnDate,
            [FromQuery] int? ticketCount)
                {
            try
            {
                var query = _context.BusBusRoutes
                    .Include(bbr => bbr.BusRoute)
                    .Include(bbr => bbr.Seats)
                    .AsQueryable();
                if (!string.IsNullOrEmpty(departPlace))
                {
                    query = query.Where(bbr => bbr.BusRoute.DepartPlace == departPlace);
                }
                if (!string.IsNullOrEmpty(arrivalPlace))
                {
                    query = query.Where(bbr => bbr.BusRoute.ArrivalPlace == arrivalPlace);
                }
                if (departureDate.HasValue)
                {
                    var departureDay = departureDate.Value.ToUniversalTime().Date;
                    query = query.Where(bbr => bbr.BusRoute.DepartureTime.Date == departureDay);
                }
                if (returnDate.HasValue)
                {
                    var returnDay = returnDate.Value.ToUniversalTime().Date; 
                    query = query.Where(bbr => bbr.BusRoute.DepartureTime.Date == returnDay);
                }
                if (ticketCount.HasValue)
                {
                    query = query.Where(bbr => bbr.Seats.Count(seat => !seat.IsBooked) >= ticketCount.Value);
                }
                var results = await query
                    .Select(bbr => new
                    {
                        bbr.BusBusRouteID,
                        bbr.BusRouteID,
                        DepartPlace = bbr.BusRoute.DepartPlace,
                        ArrivalPlace = bbr.BusRoute.ArrivalPlace,
                        DepartureTime = bbr.BusRoute.DepartureTime,
                        Duration = bbr.BusRoute.Duration,
                        PricePerSeat = bbr.Bus.Type == "VIP"
                            ? bbr.BusRoute.PricePerSeatVip
                            : bbr.BusRoute.PricePerSeat,
                        Bus = new
                        {
                            bbr.BusID,
                            bbr.Bus.NumSeat,
                            bbr.Bus.PlateNum,
                            bbr.Bus.Type
                        },
                        SeatsAvailable = bbr.Seats != null
                            ? bbr.Seats.Count(seat => !seat.IsBooked)
                            : 0,
                        Seats = bbr.Seats != null
                            ? bbr.Seats.Select(seat => new
                            {
                                seat.SeatID,
                                seat.SeatNumber,
                                seat.IsBooked
                            }).ToList()
                            : null
                    })
                    .ToListAsync();
                if (returnDate.HasValue)
                {
                    var returnResults = await _context.BusBusRoutes
                        .Include(bbr => bbr.BusRoute)
                        .Include(bbr => bbr.Seats)
                        .Where(bbr => bbr.BusRoute.DepartPlace == arrivalPlace &&
                                      bbr.BusRoute.ArrivalPlace == departPlace &&
                                      bbr.BusRoute.DepartureTime.Date == returnDate.Value.Date)
                        .Select(bbr => new
                        {
                            bbr.BusBusRouteID,
                            bbr.BusRouteID,
                            DepartPlace = bbr.BusRoute.DepartPlace,
                            ArrivalPlace = bbr.BusRoute.ArrivalPlace,
                            DepartureTime = bbr.BusRoute.DepartureTime,
                            Duration = bbr.BusRoute.Duration,
                            PricePerSeat = bbr.Bus.Type == "VIP"
                                ? bbr.BusRoute.PricePerSeatVip
                                : bbr.BusRoute.PricePerSeat,
                            Bus = new
                            {
                                bbr.BusID,
                                bbr.Bus.NumSeat,
                                bbr.Bus.PlateNum,
                                bbr.Bus.Type
                            },
                            SeatsAvailable = bbr.Seats != null
                                ? bbr.Seats.Count(seat => !seat.IsBooked)
                                : 0,
                            Seats = bbr.Seats != null
                                ? bbr.Seats.Select(seat => new
                                {
                                    seat.SeatID,
                                    seat.SeatNumber,
                                    seat.IsBooked
                                }).ToList()
                                : null
                        })
                        .ToListAsync();
                    results.AddRange(returnResults);
                }
                return Ok(results);
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



        [HttpGet("bus-bus-routes/{busBusRouteId}")]
        public async Task<IActionResult> GetBusRouteByBusBusRouteId(int busBusRouteId)
        {
            try
            {
                var busRoute = await _context.BusBusRoutes
                    .Where(bbr => bbr.BusBusRouteID == busBusRouteId)
                    .Include(bbr => bbr.BusRoute)
                    .Include(bbr => bbr.Bus)
                    .Include(bbr => bbr.Seats)
                    .Select(bbr => new
                    {
                        bbr.BusBusRouteID,
                        bbr.BusRouteID,
                        DepartPlace = bbr.BusRoute.DepartPlace,
                        ArrivalPlace = bbr.BusRoute.ArrivalPlace,
                        DepartureTime = bbr.BusRoute.DepartureTime,
                        Duration = bbr.BusRoute.Duration,
                        Price = bbr.Bus.Type == "VIP"
                            ? bbr.BusRoute.PricePerSeatVip
                            : bbr.BusRoute.PricePerSeat,
                        Bus = new
                        {
                            bbr.BusID,
                            bbr.Bus.NumSeat,
                            bbr.Bus.PlateNum,
                            bbr.Bus.Type
                        },
                        SeatsAvailable = bbr.Seats != null
                            ? bbr.Seats.Count(seat => !seat.IsBooked)
                            : 0,
                        Seats = bbr.Seats != null
                            ? bbr.Seats.Select(seat => new
                            {
                                seat.SeatID,
                                seat.SeatNumber,
                                seat.IsBooked
                            }).ToList()
                            : null
                    })
                    .FirstOrDefaultAsync();

                if (busRoute == null)
                {
                    return NotFound($"Bus Bus Route with ID {busBusRouteId} not found.");
                }

                return Ok(busRoute);
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


        [HttpPost("create-tickets")]
        public IActionResult SelectTripAndSeat([FromBody] TicketRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var seatNumbers = request.SeatNum
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();

                var tickets = new List<Ticket>();

                foreach (var seatNumber in seatNumbers)
                {
                    var seat = _context.Seats
                        .Include(s => s.BusBusRoute)
                        .FirstOrDefault(s => s.BusBusRoute.BusBusRouteID == request.BusBusRouteID && s.SeatID == seatNumber);

                    if (seat == null || seat.IsBooked)
                    {
                        return BadRequest(new { error = $"Seat {seatNumber} is not available." });
                    }

                    var ticket = new Ticket
                    {
                        BusBusRouteID = request.BusBusRouteID,
                        CustomerID = request.CustomerID,
                        SeatNum = seatNumber,
                        Type = request.Type,
                        Price = request.Price,
                        BookingDate = DateTime.UtcNow,
                        Status = "Đã thanh toán"
                    };

                    tickets.Add(ticket);

                    seat.IsBooked = true;
                }

                _context.Tickets.AddRange(tickets);
                _context.SaveChanges();

                return Ok(new { message = "Tickets created successfully.", tickets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating tickets.", details = ex.Message });
            }

        }
        [HttpPost("create-multiple-tickets")]
        public IActionResult CreateMultipleTickets([FromBody] List<TicketRequestModel> requests)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTickets = new List<Ticket>();
            var errors = new List<string>();

            try
            {
                foreach (var request in requests)
                {
                    var seatNumbers = request.SeatNum
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToList();

                    foreach (var seatNumber in seatNumbers)
                    {
                        var seat = _context.Seats
                            .Include(s => s.BusBusRoute)
                            .FirstOrDefault(s => s.BusBusRoute.BusBusRouteID == request.BusBusRouteID && s.SeatID == seatNumber);

                        if (seat == null || seat.IsBooked)
                        {
                            errors.Add($"Seat {seatNumber} for BusBusRouteID {request.BusBusRouteID} is not available.");
                            continue;
                        }

                        var ticket = new Ticket
                        {
                            BusBusRouteID = request.BusBusRouteID,
                            CustomerID = request.CustomerID,
                            SeatNum = seatNumber,
                            Type = request.Type,
                            Price = request.Price,
                            BookingDate = DateTime.UtcNow,
                            Status = "Chờ thanh toán"
                        };

                        createdTickets.Add(ticket);

                        seat.IsBooked = true;
                    }
                }

                if (createdTickets.Any())
                {
                    _context.Tickets.AddRange(createdTickets);
                    _context.SaveChanges();
                }

                return Ok(new
                {
                    message = "Multiple tickets created successfully.",
                    tickets = createdTickets,
                    errors = errors.Any() ? errors : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while creating multiple tickets.",
                    details = ex.Message
                });
            }
        }
        [HttpGet("bus-bus-routes-with-ticket-price/{ticketId}")]
        public async Task<IActionResult> FindNewTripAvailability(int ticketId)
        {
            var now = DateTime.Now.ToUniversalTime();

            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketID == ticketId);

                if (ticket == null)
                {
                    return NotFound($"Ticket with ID {ticketId} not found.");
                }

                var ticketPrice = ticket.Price;

                var busBusRoutes = await _context.BusBusRoutes
                    .Where(bbr => bbr.Bus.Type == "Thường" && bbr.BusRoute.PricePerSeat == ticketPrice ||
                                  (bbr.Bus.Type == "VIP" && bbr.BusRoute.PricePerSeatVip == ticketPrice))
                    .Include(bbr => bbr.BusRoute)
                                .Where(bbr => bbr.BusRoute.DepartureTime >= now)

                    .Include(bbr => bbr.Bus)
                    .Include(bbr => bbr.Seats)
                    .Select(bbr => new
                    {
                        bbr.BusBusRouteID,
                        bbr.BusRouteID,
                        DepartPlace = bbr.BusRoute.DepartPlace,
                        ArrivalPlace = bbr.BusRoute.ArrivalPlace,
                        DepartureTime = bbr.BusRoute.DepartureTime,
                        Duration = bbr.BusRoute.Duration,
                        PricePerSeat = bbr.Bus.Type == "VIP"
                            ? bbr.BusRoute.PricePerSeatVip
                            : bbr.BusRoute.PricePerSeat,
                        Bus = new
                        {
                            bbr.BusID,
                            bbr.Bus.NumSeat,
                            bbr.Bus.PlateNum,
                            bbr.Bus.Type
                        },
                        SeatsAvailable = bbr.Seats != null
                            ? bbr.Seats.Count(seat => !seat.IsBooked)
                            : 0,
                        Seats = bbr.Seats != null
                            ? bbr.Seats.Select(seat => new
                            {
                                seat.SeatID,
                                seat.SeatNumber,
                                seat.IsBooked
                            }).ToList()
                            : null
                    })
                    .ToListAsync();

                return Ok(busBusRoutes);
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
        [HttpGet("search_for_change")]
        public async Task<IActionResult> SearchBusRoutes(
    [FromQuery] int ticketId,
    [FromQuery] string departPlace,
    [FromQuery] string arrivalPlace,
    [FromQuery] DateTime? departureDate)
        {
            try
            {
                var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketID == ticketId);
                if (ticket == null)
                {
                    return NotFound($"Ticket with ID {ticketId} not found.");
                }

                var ticketPrice = ticket.Price;

                var query = _context.BusBusRoutes
                    .Include(bbr => bbr.BusRoute)
                    .Include(bbr => bbr.Seats)
                    .Where(bbr =>
                        (bbr.Bus.Type == "VIP" && bbr.BusRoute.PricePerSeatVip == ticketPrice) ||
                        (bbr.Bus.Type == "Thường" && bbr.BusRoute.PricePerSeat == ticketPrice))
                    .AsQueryable();

                if (!string.IsNullOrEmpty(departPlace))
                {
                    query = query.Where(bbr => bbr.BusRoute.DepartPlace == departPlace);
                }
                if (!string.IsNullOrEmpty(arrivalPlace))
                {
                    query = query.Where(bbr => bbr.BusRoute.ArrivalPlace == arrivalPlace);
                }
                if (departureDate.HasValue)
                {
                    var departureDay = departureDate.Value.ToUniversalTime().Date;
                    query = query.Where(bbr => bbr.BusRoute.DepartureTime.Date == departureDay);
                }

                var results = await query
                    .Select(bbr => new
                    {
                        bbr.BusBusRouteID,
                        bbr.BusRouteID,
                        DepartPlace = bbr.BusRoute.DepartPlace,
                        ArrivalPlace = bbr.BusRoute.ArrivalPlace,
                        DepartureTime = bbr.BusRoute.DepartureTime,
                        Duration = bbr.BusRoute.Duration,
                        PricePerSeat = bbr.Bus.Type == "VIP"
                            ? bbr.BusRoute.PricePerSeatVip
                            : bbr.BusRoute.PricePerSeat,
                        Bus = new
                        {
                            bbr.BusID,
                            bbr.Bus.NumSeat,
                            bbr.Bus.PlateNum,
                            bbr.Bus.Type
                        },
                        SeatsAvailable = bbr.Seats != null
                            ? bbr.Seats.Count(seat => !seat.IsBooked)
                            : 0,
                        Seats = bbr.Seats != null
                            ? bbr.Seats.Select(seat => new
                            {
                                seat.SeatID,
                                seat.SeatNumber,
                                seat.IsBooked
                            }).ToList()
                            : null
                    })
                    .ToListAsync();

                return Ok(results);
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



        [HttpGet("bus-bus-route-change/{notifiactionId}")]
        public async Task<IActionResult> GetBusBusRoutesChange(int notifiactionId)
        {

            try
            {
                var noti = await _context.Notifications
                    .FirstOrDefaultAsync(t => t.NotificationID == notifiactionId);
                if (noti == null)
                    return NotFound(new { message = "Notification not found." });
                var newBusBusRouteId = int.Parse(noti.Message.Split('#')[2].Split(' ')[0]);


                var busBusRoutes = await _context.BusBusRoutes
                    .Where(bbr => bbr.BusBusRouteID == newBusBusRouteId)
                    .Include(bbr => bbr.BusRoute)
                    .Include(bbr => bbr.Bus)
                    .Include(bbr => bbr.Seats)
                    .Select(bbr => new
                    {
                        bbr.BusBusRouteID,
                        bbr.BusRouteID,
                        DepartPlace = bbr.BusRoute.DepartPlace,
                        ArrivalPlace = bbr.BusRoute.ArrivalPlace,
                        DepartureTime = bbr.BusRoute.DepartureTime,
                        Duration = bbr.BusRoute.Duration,
                        PricePerSeat = bbr.Bus.Type == "VIP"
                            ? bbr.BusRoute.PricePerSeatVip
                            : bbr.BusRoute.PricePerSeat,
                        Bus = new
                        {
                            bbr.BusID,
                            bbr.Bus.NumSeat,
                            bbr.Bus.PlateNum,
                            bbr.Bus.Type
                        },
                        SeatsAvailable = bbr.Seats != null
                            ? bbr.Seats.Count(seat => !seat.IsBooked)
                            : 0,
                        Seats = bbr.Seats != null
                            ? bbr.Seats.Select(seat => new
                            {
                                seat.SeatID,
                                seat.SeatNumber,
                                seat.IsBooked
                            }).ToList()
                            : null
                    })
                    .ToListAsync();

                return Ok(busBusRoutes);
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

