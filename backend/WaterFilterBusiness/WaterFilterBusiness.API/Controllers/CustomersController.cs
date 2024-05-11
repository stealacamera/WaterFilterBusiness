using FluentResults;
using Microsoft.AspNetCore.Mvc;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.ViewModels;

namespace WaterFilterBusiness.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : Controller
    {
        public CustomersController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageSize,
            int page = 1,
        {

            OffsetPaginatedList<Customer> results;

            if (includeOnlyRedListed)
                results = await _servicesManager.CustomersService.GetAllRedListedAsync(page, pageSize);
            else
                results = await _servicesManager.CustomersService
                                                .GetAllAsync(page, pageSize, excludeWithCalls, excludeRedListed);

            return Ok(results);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _servicesManager.CustomersService.GetByIdAsync(id);
            return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        }

        [HttpGet("{id:int}/change-history")]
        public async Task<IActionResult> GetChangeHistory(int id)
        {
            var result = await _servicesManager.CustomerChangesService.GetAllForCustomer(id);

            return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Update(int id, CustomerUpdateRequestModel customerUpdate)
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

            return result.IsFailed ? BadRequest(result) : Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IList<Customer> customers)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _servicesManager.CustomersService.CreateRangeAsync(customers);
            return Created(string.Empty, result);
        }
    }
}
