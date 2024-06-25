using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class ClientRepository: IClientRepository
{
    private readonly IConfiguration _configuration;
    public ClientRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<bool> DoesClientExist(int id)
    {
        var query = "SELECT 1 FROM clients WHERE ID = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesCarExist(int id)
    {
        var query = "SELECT 1 FROM car WHERE PK = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<ClientDto> GetClient(int clientId)
    {
        throw new NotImplementedException();
    }

    public async Task AddNewClientWithRental(NewClientWithRentalDto newClientWithRental)
    {
        throw new NotImplementedException();
    }
}