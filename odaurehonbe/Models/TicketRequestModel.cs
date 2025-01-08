using System.ComponentModel.DataAnnotations;

namespace odaurehonbe.Models
{
    public class TicketRequestModel
    {
        [Required]
        public int BusBusRouteID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        [RegularExpression(@"^(\d+,)*\d+$", ErrorMessage = "Seat numbers must be a comma-separated list of integers.")]
        public string SeatNum { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
    }

}
