namespace WebApplication1.Repositories;

public interface IClientRepository
{
    Task<bool> DoesClientExist(int id);
    Task<bool> DoesCarExist(int carId);
    Task<ClientDto> GetClientWithRentals(int clientId);
    Task AddNewClientWithRental(NewClientWithRentalDto newClientWithRental);
}