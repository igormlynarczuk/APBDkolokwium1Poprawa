using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class ClientRepository: IClientRepository
{
    public Task<bool> DoesClientExist(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DoesCarExist(int carId)
    {
        throw new NotImplementedException();
    }

    public Task<ClientDto> GetClient(int clientId)
    {
        throw new NotImplementedException();
    }

    public Task AddNewClientWithRental(NewClientWithRentalDto newClientWithRental)
    {
        throw new NotImplementedException();
    }
}