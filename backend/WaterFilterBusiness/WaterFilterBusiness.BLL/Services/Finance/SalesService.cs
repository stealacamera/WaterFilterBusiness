using FluentResults;
using WaterFilterBusiness.Common.DTOs;
using WaterFilterBusiness.Common.DTOs.Finance;
using WaterFilterBusiness.Common.DTOs.ViewModels;
using WaterFilterBusiness.Common.Enums;
using WaterFilterBusiness.Common.ErrorHandling.Errors;
using WaterFilterBusiness.Common.ErrorHandling.Errors.Finance;
using WaterFilterBusiness.DAL;

namespace WaterFilterBusiness.BLL.Services.Finance;

public interface ISalesService
{
    Task<Result<Sale>> CreateAsync(Sale_AddRequestModel model);
    Task<Result<Sale>> VerifyAsync(int meetingId, string? verificationNote);
    Task<OffsetPaginatedList<Sale>> GetAllByAsync(int page, int pageSize);
}

internal class SalesService : Service, ISalesService
{
    public SalesService(IWorkUnit workUnit, IUtilityService utilityService) : base(workUnit, utilityService)
    {
    }

    public async Task<Result<Sale>> CreateAsync(Sale_AddRequestModel model)
    {
        if (!await _utilityService.DoesMeetingExistAsync(model.MeetingId))
            return ClientMeetingErrors.NotFound(nameof(model.MeetingId));
        else if(await _utilityService.GetMeetingOutcomeAsync(model.MeetingId) != MeetingOutcome.Successful)
            return SalesErrors.CannotCreate_UnsuccessfulMeeting(nameof(model.MeetingId));
        else if (await _workUnit.SalesRepository.GetByIdAsync(model.MeetingId) != null)
            return SalesErrors.AlreadyCreatedForMeeting(nameof(model.MeetingId));

        if (model.UpfrontPaymentAmount > model.TotalAmount || model.UpfrontPaymentAmount <= 0)
            return SalesErrors.InvalidUpfrontAmountValue(nameof(model.UpfrontPaymentAmount));
        else if (model.UpfrontPaymentAmount == model.TotalAmount && model.PaymentType != PaymentType.FullUpfront)
            return SalesErrors.InvalidPaymentType(nameof(model.PaymentType));
        else if (model.UpfrontPaymentAmount < model.TotalAmount && model.PaymentType != PaymentType.MonthlyInstallments)
            return SalesErrors.InvalidPaymentType(nameof(model.PaymentType));

        var dbModel = await _workUnit.SalesRepository
                                     .AddAsync(new DAL.Entities.Sale
                                     {
                                         CreatedAt = DateTime.Now,
                                         MeetingId = model.MeetingId,
                                         PaymentTypeId = model.PaymentType.Value,
                                         TotalAmount = model.TotalAmount,
                                         UpfrontPaymentAmount = model.UpfrontPaymentAmount,
                                     });

        await _workUnit.SaveChangesAsync();
        return ConvertEntityToModel(dbModel);
    }

    public async Task<OffsetPaginatedList<Sale>> GetAllByAsync(int page, int pageSize)
    {
        var sales = await _workUnit.SalesRepository.GetAllAsync(page, pageSize);

        return new OffsetPaginatedList<Sale>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = sales.TotalCount,
            Values = sales.Values.Select(ConvertEntityToModel).ToList()
        };
    }

    public async Task<Result<Sale>> VerifyAsync(int meetingId, string? verificationNote)
    {
        var sale = await _workUnit.SalesRepository.GetByIdAsync(meetingId);

        if (sale == null)
            return SalesErrors.NotFound(nameof(meetingId));

        if (sale.VerifiedAt == null)
        {
            sale.VerifiedAt = DateTime.Now;
            sale.VerificationNote = verificationNote;

            await _workUnit.SaveChangesAsync();
        }

        return ConvertEntityToModel(sale);
    }

    private Sale ConvertEntityToModel(DAL.Entities.Sale entity)
    {
        return new Sale
        {
            CreatedAt = entity.CreatedAt,
            Meeting = new ClientMeeting_BriefDescription { Id = entity.MeetingId },
            PaymentType = PaymentType.FromValue(entity.PaymentTypeId),
            TotalAmount = entity.TotalAmount,
            UpfrontPaymentAmount = entity.UpfrontPaymentAmount,
            VerificationNote = entity.VerificationNote,
            VerifiedAt = entity.VerifiedAt
        };
    }
}
