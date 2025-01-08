namespace odaurehonbe.Models
{
    public class AccountDto
    {
        public int AccountID { get; set; }
        public string? UserName { get; set; }
        public string? Password  { get; set; }
        public string? Status { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? UserType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LicenseNumber { get; set; } // Chỉ có cho Driver
        public string? Address { get; set; }       
        public DateOnly? HireDate { get; set; }     // Chỉ có cho TicketClerk
    }

}
