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

    public async Task<ClientDto> GetClient(int id)
    {
        var query = @"SELECT 
                            c.ID AS ClientID, c.FirstName, c.LastName, c.Address,
                            cr.ID AS RentalID, cr.CarID, cr.DateFrom, cr.DateTo, cr.TotalPrice, cr.Discount
                          FROM Clients c
                          LEFT JOIN CarRentals cr ON c.ID = cr.ClientID
                          WHERE c.ID = @ClientID";
	    
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@ClientID", id);
 
        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();
 
        ClientDto client = null;

	    ClientDto ClientDto = null;

	    while (await reader.ReadAsync())
	    {
		    if (ClientDto is not null)
		    {
			    ClientDto.CarRentals.Add(new CarRentalDto()
			    {
				    ID = reader.GetInt32(reader.GetOrdinal("RentalID")),
				    CarID = reader.GetInt32(reader.GetOrdinal("CarID")),
				    DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
				    DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
				    TotalPrice = reader.GetInt32(reader.GetOrdinal("TotalPrice")),
				    Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Discount"))
			    });
		    }
		    else
		    {
			    ClientDto = new ClientDto()
			    {
				    ID = reader.GetInt32(reader.GetOrdinal("ClientID")),
				    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
				    LastName = reader.GetString(reader.GetOrdinal("LastName")),
				    Address = reader.GetString(reader.GetOrdinal("Address")),
				    CarRentals = new List<CarRentalDto>()
				    {
					    new CarRentalDto()
					    {
						    ID = reader.GetInt32(reader.GetOrdinal("RentalID")),
						    CarID = reader.GetInt32(reader.GetOrdinal("CarID")),
						    DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
						    DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
						    TotalPrice = reader.GetInt32(reader.GetOrdinal("TotalPrice")),
						    Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Discount"))
					    }
				    }
			    };
		    }
	    }

	    if (ClientDto is null) throw new Exception();
        
        return ClientDto;
    }

    public async Task AddNewClientWithRental(NewClientWithRentalDto newClientWithRental)
    {
        throw new NotImplementedException();
    }
}