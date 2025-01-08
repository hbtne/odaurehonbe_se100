using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace odaurehonbe.Data;

public partial class Payment
{
    [Key]
    public int PaymentID { get; set; }

    public int? CustomerID { get; set; }

    public DateTime? PaymentTime { get; set; }

    public int? StaffID { get; set; }

    public decimal TotalFee { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public int? PromoID { get; set; }

    public virtual Promotion? Promo { get; set; }


    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
