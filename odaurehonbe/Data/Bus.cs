using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace odaurehonbe.Data
{
    public class Bus
    {
        [Key]
        public int BusID { get; set; }

        public int NumSeat { get; set; }

        public string PlateNum { get; set; }

        public string Type { get; set; }


        public ICollection<BusDriver> BusDrivers { get; set; }

        public ICollection<BusBusRoute> BusBusRoutes { get; set; }
    }
}