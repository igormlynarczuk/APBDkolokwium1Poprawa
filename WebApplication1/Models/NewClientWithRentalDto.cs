namespace WebApplication1.Models;

public class NewClientWithRentalDto
{ 
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public string Address { get; set; }
    public int CarID { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int? Discount { get; set; }
}