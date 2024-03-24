using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;

namespace WaterFilterBusiness.API.Controllers;

[Route("api/[controller]")]
[ApiController]
// TODO [authorized only to phone operators]
public class CustomersController : Controller
{
    public CustomersController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int pageSize, int page = 1)
    {
        var customers = await _servicesManager.CustomersService
                                              .GetAllAsync(page, pageSize);
        
        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _servicesManager.CustomersService.GetByIdAsync(id);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    // TODO [authorized only to sales agents]
    public async Task<IActionResult> Create(Customer customer)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState); // TODO should return alongside errors

        var result = await _servicesManager.CustomersService.AddAsync(customer);
        return Created(string.Empty, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Customer customer)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        customer.Id = id;
        var result = await _servicesManager.CustomersService.UpdateAsync(customer);

        return result.IsSuccess ? NoContent() : NotFound();
    }
}
