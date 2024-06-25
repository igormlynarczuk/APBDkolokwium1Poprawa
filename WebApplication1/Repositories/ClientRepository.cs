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
				    TotalPrice = 
			    });
		    }
		    else
		    {
			    animalDto = new AnimalDto()
			    {
				    Id = reader.GetInt32(animalIdOrdinal),
				    Name = reader.GetString(animalNameOrdinal),
				    Type = reader.GetString(animalTypeOrdinal),
				    AdmissionDate = reader.GetDateTime(admissionDateOrdinal),
				    Owner = new OwnerDto()
				    {
					    Id = reader.GetInt32(ownerIdOrdinal),
					    FirstName = reader.GetString(firstNameOrdinal),
					    LastName = reader.GetString(lastNameOrdinal),
				    },
				    Procedures = new List<ProcedureDto>()
				    {
					    new ProcedureDto()
					    {
						    Date = reader.GetDateTime(dateOrdinal),
						    Name = reader.GetString(procedureNameOrdinal),
						    Description = reader.GetString(procedureDescriptionOrdinal)
					    }
				    }
			    };
		    }
	    }

	    if (animalDto is null) throw new Exception();
        
        return animalDto;
    }

    public async Task AddNewClientWithRental(NewClientWithRentalDto newClientWithRental)
    {
        throw new NotImplementedException();
    }
}