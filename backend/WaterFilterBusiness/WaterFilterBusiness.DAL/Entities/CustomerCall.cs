﻿namespace WaterFilterBusiness.DAL.Entities;

public class CustomerCall : StrongEntity
{
    public int CustomerId { get; set; }
    public int PhoneAgentId { get; set; }
    public int OutcomeId { get; set; }
    public DateTime OccuredAt { get; set; }
}

public class ScheduledCall : StrongEntity
{
    public int CustomerId { get; set; }
    public int PhoneAgentId { get; set; }
    public DateTime ScheduledAt { get; set; }
}