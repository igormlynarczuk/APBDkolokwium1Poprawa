using WebApplication1.Models;

namespace WebApplication1.Repositories;

public interface IClientRepository
{
    Task<bool> DoesClientExist(int id);
    Task<bool> DoesCarExist(int carId);
    Task<ClientDto> GetClient(int clientId);
    Task AddNewClientWithRental(NewClientWithRentalDto newClientWithRental);
}