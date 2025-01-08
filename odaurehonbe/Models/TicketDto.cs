namespace odaurehonbe.Models
{
    public class TicketDto
    {
        public int TicketId { get; set; }
        public string SeatNumber { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public int BusNumber { get; set; }
        public string LicensePlate { get; set; }
    }

}
