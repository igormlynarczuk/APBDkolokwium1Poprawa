using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientRepository _clientRepository;

    public ClientController(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
        if (!await _clientRepository.DoesClientExist(id))
            return NotFound($"Client with given ID - {id} doesn't exist");

        var client = await _clientRepository.GetClient(id);
            
        return Ok(client);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddClientWithRental(NewClientWithRentalDto newClientWithRental)
    {
        if (!await _clientRepository.DoesCarExist(newClientWithRental.CarID))
            return NotFound($"Car with given ID - {newClientWithRental.CarID} doesn't exist");
 
        await _clientRepository.AddNewClientWithRental(newClientWithRental);
 
        return Created(Request.Path.Value ?? "api/clients", newClientWithRental);
    }

}