using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.DTOs.Finance;
using WaterFilterBusiness.Common.Enums;

namespace WaterFilterBusiness.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : Controller
    {
        public SalesController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create(Sale_AddRequestModel model)
        {
            var result = await _servicesManager.WrapInTransactionAsync(async () =>
            {
                var createResult = await _servicesManager.SalesService.CreateAsync(model);

                if (createResult.IsFailed)
                    return createResult;

                if (model.PaymentType == PaymentType.MonthlyInstallments)
                {
                    var debtCreateResult = await _servicesManager.ClientDebtsService
                                                                 .CreateAllForSaleAsync(
                                                                    createResult.Value.Meeting.Id,
                                                                    model.MonthlyInstallmentPayment.Value);

                    if (debtCreateResult.IsFailed)
                        return Result.Fail(debtCreateResult.Errors);
                }

                return createResult.Value;
            });

            return result.IsSuccess
                   ? Created(nameof(Create), result.Value)
                   : BadRequest(result.Errors);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page, int pageSize)
        {
            var sales = await _servicesManager.SalesService.GetAllByAsync(page, pageSize);
            return Ok(sales);
        }

        [HttpPatch("{meetingId:int}")]
        public async Task<IActionResult> Verify(int meetingId, [FromBody, MaxLength(210)] string? verificationNote)
        {
            var verifyResult = await _servicesManager.SalesService
                                                     .VerifyAsync(meetingId, verificationNote);

            return verifyResult.IsSuccess
                   ? Ok(verifyResult.Value)
                   : BadRequest(verifyResult.Errors);
        }
    }
}
