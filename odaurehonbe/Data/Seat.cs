using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace odaurehonbe.Data;

public partial class Seat
{
    [Key]
    public int SeatID { get; set; }

    public string SeatNumber { get; set; } = null!;

    public bool IsBooked { get; set; }

    public int BusBusRouteID { get; set; }

    public virtual BusBusRoute BusBusRoute { get; set; } = null!;
}
