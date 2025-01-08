using System;
using System.Collections.Generic;

namespace odaurehonbe.Data;

public partial class Param
{
    public DateTime CancelDate { get; set; }

    public DateTime RefundableDate { get; set; }

    public DateTime PaymentTerm { get; set; }

    public DateTime MaxPromo { get; set; }
}
