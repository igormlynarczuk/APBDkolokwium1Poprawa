using Microsoft.AspNetCore.Mvc;
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
            return NotFound($"Animal with given ID - {id} doesn't exist");

        var animal = await _clientRepository.GetClient(id);
            
        return Ok(animal);
    }
    
}