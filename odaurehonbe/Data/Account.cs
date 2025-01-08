using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace odaurehonbe.Data;

public partial class Account
{
    [Key]
    public int AccountID { get; set; }

    public string UserType { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual TicketClerk? TicketClerk { get; set; }
}
