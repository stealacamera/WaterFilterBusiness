using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : Controller
{
    public CustomersController(IServicesManager servicesManager) : base(servicesManager)
    {
    }

    [HasPermission(Permission.ReadCustomers)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Required, Range(1, int.MaxValue)] int pageSize,
        [Required, Range(1, int.MaxValue)] int page,
        bool? filterWithCalls = null,
        bool? filterByRedListed = null)
    {
        var results = await _servicesManager.CustomersService
                                            .GetAllAsync(page, pageSize, filterWithCalls, filterByRedListed);

        return Ok(results);
    }

    [HasPermission(Permission.ReadCustomerDetails)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var result = await _servicesManager.CustomersService.GetByIdAsync(id);

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }

    [HasPermission(Permission.ReadCustomerChangeHistories)]
    [HttpGet("{id:int}/change-history")]
    public async Task<IActionResult> GetChangeHistory(int id)
    {
        var result = await _servicesManager.CustomerChangesService.GetAllForCustomer(id);

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }

    [HasPermission(Permission.UpdateCustomers)]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, Customer_UpdateRequestModel customerUpdate)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicesManager.WrapInTransactionAsync<CustomerUpdate>(async () =>
        {
            var currentCustomerResult = await _servicesManager.CustomersService
                                                              .GetByIdAsync(id);

            if (currentCustomerResult.IsFailed)
                return Result.Fail(currentCustomerResult.Errors);

            var updateResult = await _servicesManager.CustomersService.UpdateAsync(id, customerUpdate);

            if (updateResult.IsFailed)
                return Result.Fail(updateResult.Errors);

            var changesResult = await _servicesManager.CustomerChangesService
                                               .CreateAsync(currentCustomerResult.Value, updateResult.Value);

            return changesResult.IsFailed ?
                   Result.Fail(changesResult.Errors) :
                    new CustomerUpdate { Customer = updateResult.Value, OldCustomer = changesResult.Value };
        });

        return result.IsFailed 
               ? BadRequest(result.GetErrorsDictionary()) 
               : Ok(result.Value);
    }

    [HasPermission(Permission.CreateCustomers)]
    [HttpPost]
    public async Task<IActionResult> Create(Customer_AddRequestModel[] customers)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createResult = await _servicesManager.CustomersService.CreateRangeAsync(customers);

        return createResult.IsSuccess 
               ? Created(string.Empty, createResult.Value)
               : BadRequest(createResult.GetErrorsDictionary());
    }
}