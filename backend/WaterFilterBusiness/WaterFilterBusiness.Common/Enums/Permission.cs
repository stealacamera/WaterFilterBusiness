namespace WaterFilterBusiness.Common.Enums;

public enum Permission
{
    ManageUsers = 1,

    ManageInventoryItems = 2,
    ReadInventoryMovements = 3,
    ReadInventoryPurchases = 4,

    ManageBigInventory = 5,
    ReadSmallInventory = 6,
    ReadTechnicianInventory = 7,

    CreateSmallInventoryRequests = 8,
    ReadSmallInventoryRequests = 9,
    ResolveSmallInventoryRequests = 10,

    CreateTechinicianInventoryRequests = 11,
    ReadTechinicianInventoryRequests = 12,
    ResolveTechnicianInventoryRequests = 13,
    
    DecreaseTechnicianStock = 14,

    ReadSalesAgentSchedules = 15,
    ReadSalesAgentScheduleChangeHistories = 16,
    CreateSalesAgentSchedules = 17,
    UpdateSalesAgentSchedules = 18,
    DeleteSalesAgentSchedules = 19,

    ReadCustomers = 20,
    ReadCustomerDetails = 21,
    ReadCustomerChangeHistories = 22,
    CreateCustomers = 23,
    UpdateCustomers = 24,

    ReadCustomerCalls = 25,
    CreateCustomerCalls = 26,

    ReadScheduledCalls = 27,
    CreateScheduledCalls = 28,
    EditScheduledCalls = 29,
    DeleteScheduledCalls = 30,

    ReadClientMeetings = 31,
    ReadClientMeetingsForWorker = 32,
    CreateClientMeetings = 33,
    ConcludeClientMeetings = 34,

    ReadClientDebts = 35,
    EditClientDebts = 36,

    ReadSales = 37,
    CreateSales = 38,
    VerifySales = 39,

    ReadStatistics = 40,
}