﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.EntityFrameworkCore;
using Npgsql.Internal;
using odaurehonbe.Data;
using odaurehonbe.Models;
using System.Reflection.Metadata;
using iTextDocument = iText.Layout.Document;
using iText.Layout;
using iText.Layout.Element;

namespace odaurehonbe.Controllers
{
    public class TicketController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TicketController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("booked-tickets/{accountId}")]
        public async Task<IActionResult> LoadBookingHistoryScreen(int accountId)
        {
            try
            {
                var tickets = await _dbContext.Tickets
                    .Where(t => t.CustomerID == accountId)
                    .Join(
                        _dbContext.BusBusRoutes,
                        ticket => ticket.BusBusRouteID,
                        busBusRoute => busBusRoute.BusBusRouteID,
                        (ticket, busBusRoute) => new { ticket, busBusRoute }
                    )
                    .Join(
                        _dbContext.Buses,
                        combined => combined.busBusRoute.BusID,
                        bus => bus.BusID,
                        (combined, bus) => new { combined.ticket, combined.busBusRoute, bus }
                    )
                    .Join(
                        _dbContext.BusRoutes,
                        combined => combined.busBusRoute.BusRouteID,
                        busRoute => busRoute.BusRouteID,
                        (combined, busRoute) => new { combined.ticket, combined.busBusRoute, combined.bus, busRoute }
                    )
                    .Join(
                        _dbContext.Seats,
                        combined => combined.ticket.SeatNum, 
                        seat => seat.SeatID, 
                        (combined, seat) => new
                        {
                            TicketId = combined.ticket.TicketID,
                            SeatNumber = seat.SeatNumber, 
                            Departure = combined.busRoute.DepartPlace,
                            Destination = combined.busRoute.ArrivalPlace,
                            DepartureTime = combined.busRoute.DepartureTime,
                            BusNumber = combined.busBusRoute.BusID,
                            LicensePlate = combined.bus.PlateNum,
                            Status = combined.ticket.Status,
                            Price = combined.ticket.Price,
                            IsDeparted = DateTime.Now > combined.busRoute.DepartureTime.ToLocalTime() ? "Đã khởi hành" : "Chưa khởi hành"
                        }
                    )
                    .ToListAsync();

                if (!tickets.Any())
                    return NotFound(new { message = "No tickets found for this account." });

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving tickets.", error = ex.Message });
            }
        }
        [HttpGet("check-ticket/{ticketCode}/{phoneNumber}")]
        public async Task<IActionResult> CheckTicketByPhoneAndCode(int ticketCode, string phoneNumber)
        {
            try
            {
                var ticket = await _dbContext.Tickets
                    .Where(t => t.TicketID == ticketCode)
                    .FirstOrDefaultAsync();

                if (ticket == null)
                    return NotFound(new { message = "Ticket not found." });

                var customer = await _dbContext.Customers
                    .Where(c => c.AccountID == ticket.CustomerID && c.PhoneNumber == phoneNumber)
                    .FirstOrDefaultAsync();

                if (customer == null)
                    return Unauthorized(new { message = "Phone number does not match the ticket holder." });


               var ticketDetails = await _dbContext.Tickets
                    .Where(t => t.TicketID == ticketCode)
                    .Join(
                        _dbContext.BusBusRoutes,
                        ticket => ticket.BusBusRouteID,
                        busBusRoute => busBusRoute.BusBusRouteID,
                        (ticket, busBusRoute) => new { ticket, busBusRoute }
                    )
                    .Join(
                        _dbContext.Buses,
                        combined => combined.busBusRoute.BusID,
                        bus => bus.BusID,
                        (combined, bus) => new { combined.ticket, combined.busBusRoute, bus }
                    )
                    .Join(
                        _dbContext.BusRoutes,
                        combined => combined.busBusRoute.BusRouteID,
                        busRoute => busRoute.BusRouteID,
                        (combined, busRoute) => new { combined.ticket, combined.busBusRoute, combined.bus, busRoute }
                    )
                    .Join(
                        _dbContext.Seats,
                        combined => combined.ticket.SeatNum,
                        seat => seat.SeatID,
                        (combined, seat) => new
                        {
                            TicketId = combined.ticket.TicketID,
                            SeatNumber = seat.SeatNumber,
                            Departure = combined.busRoute.DepartPlace,
                            Destination = combined.busRoute.ArrivalPlace,
                            DepartureTime = combined.busRoute.DepartureTime,
                            BusNumber = combined.busBusRoute.BusID,
                            LicensePlate = combined.bus.PlateNum,
                            Status = combined.ticket.Status,
                            Price = combined.ticket.Price,
                            IsDeparted = DateTime.Now > combined.busRoute.DepartureTime.ToLocalTime() ? "Đã khởi hành" : "Chưa khởi hành"
                        }
                    )
                    .FirstOrDefaultAsync();

                if (ticketDetails == null)
                    return NotFound(new { message = "Ticket details not found." });

                return Ok(ticketDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving ticket details.", error = ex.Message });
            }
        }

        [HttpDelete("cancel-ticket/{notificationId}")]
        public async Task<IActionResult> CancelTicket(int notificationId)
        {
            try
            {
      
                var notification = await _dbContext.Notifications
                    .Where(n => n.NotificationID == notificationId)
                    .FirstOrDefaultAsync();

                if (notification == null)
                {
                    return NotFound(new { message = "Notification not found." });
                }

            
                var ticket = await _dbContext.Tickets
                    .Where(t => t.TicketID == notification.TicketID)
                    .FirstOrDefaultAsync();

                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found." });
                }

                var seat = await _dbContext.Seats.FirstOrDefaultAsync(s => s.SeatID == ticket.SeatNum);
                if (seat != null)
                {
                    seat.IsBooked = false;  
                }


                _dbContext.Tickets.Remove(ticket);


                notification.IsHandled = true;


                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Ticket has been canceled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error canceling ticket.", error = ex.Message });
            }
        }
        [HttpDelete("cancel-ticket-by-staff/{ticketId}")]
        public async Task<IActionResult> CancelTicketByStaff(int ticketId)
        {
            try
            {
                var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.TicketID == ticketId);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found." });
                }

                var seat = await _dbContext.Seats.FirstOrDefaultAsync(s => s.SeatID == ticket.SeatNum);
                if (seat != null)
                {
                    seat.IsBooked = false;
                }

                _dbContext.Tickets.Remove(ticket);

                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Ticket has been canceled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error canceling ticket.", error = ex.Message });
            }
        }
        [HttpPost("change-ticket-request/{ticketId}/{newBusBusRouteId}")]
        public async Task<IActionResult> RequestChangeTicket(int ticketId, int newBusBusRouteId)
        {
            try
            {

                var notification = new Notification
                {
                    TicketID = ticketId,
                    ClerkID = null,
               
                    Message = $"Request to change ticket #{ticketId} to BusBusRoute #{newBusBusRouteId}",
                };

                await _dbContext.Notifications.AddAsync(notification);

                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Change request sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error sending change request.", error = ex.Message });
            }
        }

        [HttpPost("generate-pdf")]
        public IActionResult GeneratePdf([FromBody] TicketDto ticket)
        {
            if (ticket == null)
            {
                return BadRequest("Ticket data is required");
            }

            try
            {
                if (string.IsNullOrEmpty(ticket.Departure) || string.IsNullOrEmpty(ticket.Destination))
                {
                    return BadRequest("Required ticket data is missing");
                }

                var stream = CreatePdf(ticket);

                if (stream == null)
                {
                    return StatusCode(500, new { message = "Error generating PDF" });
                }

                return File(stream.ToArray(), "application/pdf", $"ticket-{ticket.TicketId}.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, new { message = "Error generating PDF", error = ex.Message });
            }
        }

        public static MemoryStream CreatePdf(TicketDto ticket)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new PdfWriter(stream);
                    var pdf = new PdfDocument(writer);
                    var document = new iText.Layout.Document(pdf);

                    document.Add(new Paragraph($"Mã vé: {ticket.TicketId.ToString()}"));
                    document.Add(new Paragraph($"Số ghế: {ticket.SeatNumber}"));
                    document.Add(new Paragraph($"Nơi đi: {ticket.Departure}"));
                    document.Add(new Paragraph($"Nơi đến: {ticket.Destination}"));
                    document.Add(new Paragraph($"Giờ khởi hành: {ticket.DepartureTime.ToString("dd/MM/yyyy HH:mm")}"));
                    document.Add(new Paragraph($"Số xe: {ticket.BusNumber.ToString()}"));
                    document.Add(new Paragraph($"Biển số xe: {ticket.LicensePlate}"));

                    document.Close();
                    stream.Position = 0;

                    return stream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }




        [HttpPost("cancel-ticket-request/{ticketId}")]
        public async Task<IActionResult> RequestCancelTicket(int ticketId)
        {
            try
            {

                var notification = new Notification
                {
                    TicketID = ticketId,
                    ClerkID = null,
                    Message = $"Request to cancel ticket #{ticketId} to and refund",
                };

                await _dbContext.Notifications.AddAsync(notification);

                await _dbContext.SaveChangesAsync();

                return Ok(new { message = "Cancel request sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error sending change request.", error = ex.Message });
            }
        }
        //[HttpPost("process-change-ticket/{notificationId}/{clerkId}")]
        //public async Task<IActionResult> ProcessChangeTicket(int notificationId, int clerkId)
        //{
        //    try
        //    {
        //        var notification = await _dbContext.Notifications.Include(n => n.Ticket).FirstOrDefaultAsync(n => n.NotificationID == notificationId);
        //        if (notification == null)
        //            return NotFound(new { message = "Notification not found." });

        //        var ticket = notification.Ticket;
        //        if (ticket == null)
        //            return NotFound(new { message = "Ticket not found." });

        //        var newBusBusRouteId = int.Parse(notification.Message.Split('#')[2].Split(' ')[0]);
        //        var newBusBusRoute = await _dbContext.BusBusRoutes.FirstOrDefaultAsync(bbr => bbr.BusBusRouteID == newBusBusRouteId);
        //        if (newBusBusRoute == null)
        //            return NotFound(new { message = "New BusBusRoute not found." });

        //        ticket.BusBusRouteID = newBusBusRouteId;

        //        notification.ClerkID = clerkId; 
        //        notification.IsHandled = true;

        //        await _dbContext.SaveChangesAsync();

        //        return Ok(new { message = "Ticket changed successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error processing ticket change.", error = ex.Message });
        //    }
        //}
        [HttpPut("update-ticket")]
        public async Task<IActionResult> ProcessTicketChange([FromBody] UpdateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var notification = await _dbContext.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationID == request.NotificationID);

                if (notification == null)
                {
                    return NotFound($"Notification with ID {request.NotificationID} not found.");
                }

                var messageParts = notification.Message?.Split(new[] { "to BusBusRoute #" }, StringSplitOptions.None);
                if (messageParts.Length < 2)
                {
                    return BadRequest("Notification message format is invalid.");
                }

                var ticketId = int.Parse(messageParts[0].Replace("Request to change ticket #", "").Trim());
                var newBusBusRouteId = int.Parse(messageParts[1].Trim());

                var ticket = await _dbContext.Tickets
                    .FirstOrDefaultAsync(t => t.TicketID == ticketId);

                if (ticket == null)
                {
                    return NotFound($"Ticket with ID {ticketId} not found.");
                }

                var oldSeat = await _dbContext.Seats
                    .FirstOrDefaultAsync(s => s.SeatID == ticket.SeatNum && s.BusBusRouteID == ticket.BusBusRouteID);

                if (oldSeat == null)
                {
                    return NotFound("Old seat not found.");
                }

                oldSeat.IsBooked = false;

                var newSeat = await _dbContext.Seats
                    .FirstOrDefaultAsync(s => s.SeatID == request.NewSeatID && s.BusBusRouteID == newBusBusRouteId);

                if (newSeat == null || newSeat.IsBooked)
                {
                    return BadRequest("New seat is either not found or already booked.");
                }

                newSeat.IsBooked = true;

                ticket.SeatNum = request.NewSeatID;
                ticket.BusBusRouteID = newBusBusRouteId;

                notification.IsHandled = true;
                notification.ClerkID = request.ClerkID;

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Ticket updated successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while updating the ticket.",
                    error = ex.Message
                });
            }
        }

    }
}

    