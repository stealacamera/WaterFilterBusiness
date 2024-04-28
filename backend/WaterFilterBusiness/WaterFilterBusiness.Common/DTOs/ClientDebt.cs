﻿namespace WaterFilterBusiness.Common.DTOs;

public record ClientDebt(
    int Id,
    Sale_BriefDecsription Sale, 
    decimal Amount, 
    DateOnly DeadlineAt, 
    bool IsCompleted);

public record ClientDebt_AddRequestModel
{
    public int SaleId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DeadlineAt { get; set; }
}