namespace odaurehonbe.Models
{
    public class BusRequestModel
    {
        public int BusID { get; set; }
        public int NumSeat { get; set; }
        public string PlateNum { get; set; }
        public string Type { get; set; }
        public string BusRouteIds { get; set; }  
        public string DriverIds { get; set; }    
    }

}
