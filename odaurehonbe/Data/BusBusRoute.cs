using odaurehonbe.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class BusBusRoute
{
    [Key]
    public int BusBusRouteID { get; set; }

    public int BusID { get; set; }

    public int BusRouteID { get; set; }
    [JsonIgnore]
    public Bus Bus { get; set; }
    [JsonIgnore]
    public BusRoute BusRoute { get; set; }
    public ICollection<Seat> Seats { get; set; }
}