using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace odaurehonbe.Data;

public partial class TicketClerk
{
    [Key]
    public int AccountID { get; set; }

    public string Name { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
