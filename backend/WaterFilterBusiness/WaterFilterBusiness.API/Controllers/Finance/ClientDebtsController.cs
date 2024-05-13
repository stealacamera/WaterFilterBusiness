using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WaterFilterBusiness.BLL;
using WaterFilterBusiness.Common.Attributes;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Finance;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.Utilities;

namespace WaterFilterBusiness.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientDebtsController : Controller
    {
        public ClientDebtsController(IServicesManager servicesManager) : base(servicesManager)
        {
        }

        [HasPermission(Permission.ReadClientDebts)]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [Required, Range(0, int.MaxValue)] int paginationCursor,
            [Required, Range(1, int.MaxValue)] int pageSize,
            int? filterByClient = null,
            bool? filterByCompletionStatus = null)
        {
            var debts = await _servicesManager.ClientDebtsService
                                              .GetAllAsync(
                                                paginationCursor, pageSize,
                                                filterByClient, filterByCompletionStatus);

            foreach (var debt in debts.Values)
                await PopulateModel(debt);

            return Ok(debts);
        }

        [HasPermission(Permission.EditClientDebts)]
        [HttpPatch("{id:int}/complete")]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var updateResult = await _servicesManager.ClientDebtsService.CompletePaymentAsync(id);

            if (updateResult.IsSuccess)
            {
                await PopulateModel(updateResult.Value);
                return Ok(updateResult.Value);
            }
            else
                return BadRequest(updateResult.GetErrorsDictionary());
        }

        private async Task PopulateModel(ClientDebt model)
        {
            var sale = (await _servicesManager.SalesService
                                                 .GetByIdAsync(model.Sale.Meeting.Id))
                                                 .Value;

            var meeting = (await _servicesManager.ClientMeetingsService
                                                .GetByIdAsync(sale.Meeting.Id))
                                                .Value;

            var salesAgent = (await _servicesManager.UsersService
                                                    .GetByIdAsync(meeting.SalesAgent.Id))
                                                    .Value;
            var customer = (await _servicesManager.CustomersService
                                                  .GetByIdAsync(meeting.Customer.Id))
                                                  .Value;

            model.Sale = new Sale_BriefDecsription
            {
                CreatedAt = sale.CreatedAt,
                Meeting = new ClientMeeting_BriefDescription
                {
                    Id = sale.Meeting.Id,
                    Customer = new Customer_BriefDescription
                    {
                        Id = customer.Id,
                        Address = customer.Address,
                        FullName = customer.FullName
                    },
                    SalesAgent = new User_BriefDescription
                    {
                        Id = salesAgent.Id,
                        Name = salesAgent.Name,
                        Surname = salesAgent.Surname,
                        Username = salesAgent.Username
                    }
                },
                PaymentType = sale.PaymentType,
                TotalAmount = sale.TotalAmount
            };
        }
    }
}
