using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace odaurehonbe.Data;

public partial class Promotion
{
    [Key]
    public int PromoID { get; set; }

    public string Name { get; set; } = null!;

    public float DiscountPercent { get; set; }

    public decimal Discount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? PaymentID { get; set; }

    public virtual Payment? Payment { get; set; }

}
