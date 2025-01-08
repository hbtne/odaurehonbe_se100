using odaurehonbe.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class BusDriver
{
    [Key]
    public int BusDriverID { get; set; }  // ID duy nhất cho mối quan hệ

    public int BusID { get; set; }  // Khóa ngoại đến Bus

    public int DriverID { get; set; }  // Khóa ngoại đến Driver
    [JsonIgnore]
    public Bus Bus { get; set; }  // Mối quan hệ với Bus
    [JsonIgnore]
    public Driver Driver { get; set; }  // Mối quan hệ với Driver
}