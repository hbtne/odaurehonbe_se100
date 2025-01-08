using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace odaurehonbe.Data;

public partial class Ticket
{
    [Key]
    public int TicketID { get; set; }

    public int BusBusRouteID { get; set; }

    public int CustomerID { get; set; }

    public int SeatNum { get; set; }

    public DateTime BookingDate { get; set; }

    public string Type { get; set; } = null!;

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public int? PaymentID { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Payment? Payment { get; set; }
}
