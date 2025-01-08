using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace odaurehonbe.Data;

public partial class Notification
{
    [Key]
    public int NotificationID { get; set; }

    public int TicketID { get; set; }

    public int? ClerkID { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsHandled { get; set; }

    public virtual TicketClerk? Clerk { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;
}
