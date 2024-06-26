﻿using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.DTOs.Finance;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : Controller
    {
        public SalesController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HasPermission(Permission.ReadSales)]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [Required, Range(1, int.MaxValue)] int page,
            [Required, Range(1, int.MaxValue)] int pageSize)
        {
            var sales = await _servicesManager.SalesService.GetAllByAsync(page, pageSize);
            return Ok(sales);
        }

        [HasPermission(Permission.CreateSales)]
        [HttpPost]
        public async Task<IActionResult> Create(Sale_AddRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
                   : BadRequest(result.GetErrorsDictionary());
        }

        [HasPermission(Permission.VerifySales)]
        [HttpPatch("{meetingId:int}/verify")]
        public async Task<IActionResult> Verify(int meetingId, [FromBody, MaxLength(210)] string? verificationNote)
        {
            var verifyResult = await _servicesManager.SalesService
                                                     .VerifyAsync(meetingId, verificationNote);

            return verifyResult.IsSuccess
                   ? Ok(verifyResult.Value)
                   : BadRequest(verifyResult.GetErrorsDictionary());
        }
    }
}
