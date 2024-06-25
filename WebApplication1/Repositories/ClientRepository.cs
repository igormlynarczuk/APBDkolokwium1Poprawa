using Microsoft.Data.SqlClient;
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

    public async Task<ClientDto> GetClientWithRentals(int clientId)
        {
            var query = @"SELECT
								c.ID AS ClientID, c.FirstName, c.LastName, c.Address,
								cr.ID AS RentalID, cr.CarID, cr.DateFrom, cr.DateTo, cr.TotalPrice, cr.Discount
								FROM Clients c
								LEFT JOIN CarRentals cr ON c.ID = cr.ClientID
								WHERE c.ID = @ClientID";
 
            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ClientID", clientId);
 
            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();
 
            ClientDto client = null;
 
            while (await reader.ReadAsync())
            {
                if (client == null)
                {
                    client = new ClientDto
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("ClientID")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        Address = reader.GetString(reader.GetOrdinal("Address")),
                        CarRentals = new List<CarRentalDto>()
                    };
                }
 
                if (!reader.IsDBNull(reader.GetOrdinal("RentalID")))
                {
                    client.CarRentals.Add(new CarRentalDto
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("RentalID")),
                        CarID = reader.GetInt32(reader.GetOrdinal("CarID")),
                        DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        TotalPrice = reader.GetInt32(reader.GetOrdinal("TotalPrice")),
                        Discount = reader.IsDBNull(reader.GetOrdinal("Discount")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Discount"))
                    });
                }
            }
 
            return client;
        }
 
        public async Task AddNewClientWithRental(NewClientWithRentalDto newClientWithRental)
        {
            var insertClientQuery = @"INSERT INTO Clients (FirstName, LastName, Address) VALUES (@FirstName, @LastName, @Address);
                                      SELECT CAST(SCOPE_IDENTITY() as int);";
            var insertRentalQuery = @"INSERT INTO CarRentals (ClientID, CarID, DateFrom, DateTo, TotalPrice, Discount)
                                      VALUES (@ClientID, @CarID, @DateFrom, @DateTo, @TotalPrice, @Discount);";
 
            await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            await connection.OpenAsync();
            var transaction = await connection.BeginTransactionAsync();
 
            try
            {
                await using SqlCommand insertClientCommand = new SqlCommand(insertClientQuery, connection, transaction as SqlTransaction);
                insertClientCommand.Parameters.AddWithValue("@FirstName", newClientWithRental.FirstName);
                insertClientCommand.Parameters.AddWithValue("@LastName", newClientWithRental.LastName);
                insertClientCommand.Parameters.AddWithValue("@Address", newClientWithRental.Address);
 
                var clientId = (int)await insertClientCommand.ExecuteScalarAsync();
 
                var totalPrice = (newClientWithRental.DateTo - newClientWithRental.DateFrom).Days * 100; // Assuming daily rental rate is 100
 
                await using SqlCommand insertRentalCommand = new SqlCommand(insertRentalQuery, connection, transaction as SqlTransaction);
                insertRentalCommand.Parameters.AddWithValue("@ClientID", clientId);
                insertRentalCommand.Parameters.AddWithValue("@CarID", newClientWithRental.CarID);
                insertRentalCommand.Parameters.AddWithValue("@DateFrom", newClientWithRental.DateFrom);
                insertRentalCommand.Parameters.AddWithValue("@DateTo", newClientWithRental.DateTo);
                insertRentalCommand.Parameters.AddWithValue("@TotalPrice", totalPrice);
				insertRentalCommand.Parameters.AddWithValue("@Discount", newClientWithRental.Discount.HasValue ? newClientWithRental.Discount.Value : DBNull.Value);
 
                await insertRentalCommand.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}