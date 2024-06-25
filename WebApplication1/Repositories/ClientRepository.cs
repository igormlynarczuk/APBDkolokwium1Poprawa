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
							Client.ID AS ClientID,
							Animal.Name AS AnimalName,
							Type,
							AdmissionDate,
							Owner.ID as OwnerID,
							FirstName,
							LastName,
							Date,
							[Procedure].Name AS ProcedureName,
							Description
						FROM Animal
						JOIN Owner ON Owner.ID = Animal.Owner_ID
						JOIN Procedure_Animal ON Procedure_Animal.Animal_ID = Animal.ID
						JOIN [Procedure] ON [Procedure].ID = Procedure_Animal.Procedure_ID
						WHERE Animal.ID = @ID";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@ID", id);
	    
	    await connection.OpenAsync();

	    var reader = await command.ExecuteReaderAsync();

	    var animalIdOrdinal = reader.GetOrdinal("AnimalID");
	    var animalNameOrdinal = reader.GetOrdinal("AnimalName");
	    var animalTypeOrdinal = reader.GetOrdinal("Type");
	    var admissionDateOrdinal = reader.GetOrdinal("AdmissionDate");
	    var ownerIdOrdinal = reader.GetOrdinal("OwnerID");
	    var firstNameOrdinal = reader.GetOrdinal("FirstName");
	    var lastNameOrdinal = reader.GetOrdinal("LastName");
	    var dateOrdinal = reader.GetOrdinal("Date");
	    var procedureNameOrdinal = reader.GetOrdinal("ProcedureName");
	    var procedureDescriptionOrdinal = reader.GetOrdinal("Description");

	    ClientDto ClientDto = null;

	    while (await reader.ReadAsync())
	    {
		    if (ClientDto is not null)
		    {
			    ClientDto.CarRentals.Add(new CarRentalDto()
			    {
				    ID = reader.GetDateTime(dateOrdinal),
				    CarID = reader.GetString(procedureNameOrdinal),
				    DateFrom = reader.GetString(procedureDescriptionOrdinal),
				    DateTo = reader.GetString(procedureDescriptionOrdinal),
				    TotalPrice = reader.GetString(procedureDescriptionOrdinal),
				    Discount = reader.GetString(procedureDescriptionOrdinal)
			    });
		    }
		    else
		    {
			    ClientDto = new ClientDto()
			    {
				    ID = reader.GetInt32(animalIdOrdinal),
				    FirstName = reader.GetString(animalNameOrdinal),
				    LastName = reader.GetString(animalTypeOrdinal),
				    Address = reader.GetString(animalTypeOrdinal),
				    CarRentals = new List<CarRentalDto>()
				    {
					    new CarRentalDto()
					    {
						    ID = reader.GetDateTime(dateOrdinal),
						    CarID = reader.GetString(procedureNameOrdinal),
						    DateFrom = reader.GetString(procedureDescriptionOrdinal),
						    DateTo = reader.GetString(procedureDescriptionOrdinal),
						    TotalPrice = reader.GetString(procedureDescriptionOrdinal),
						    Discount = reader.GetString(procedureDescriptionOrdinal)
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