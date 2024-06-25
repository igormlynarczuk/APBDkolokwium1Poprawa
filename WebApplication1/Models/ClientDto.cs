namespace WebApplication1.Models;

public class ClientDto
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public List<CarRentalDto> CarRentals { get; set; }
}
 
public class CarRentalDto
{
    public int ID { get; set; }
    public int CarID { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int TotalPrice { get; set; }
    public int? Discount { get; set; }
}