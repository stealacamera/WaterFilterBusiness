namespace WaterFilterBusiness.Common.Enums;

public enum Permission
{
    ReadUsers = 1,
    CreateUser = 2,
    DeleteUser = 3,

    ReadInventoryItems = 4,
    CreateInventoryItem = 5,
    DeleteInventoryItem = 6,
    UpdateInventoryItem = 7,

    ReadInventoryPurchases = 8,
    CreateInventoryPurchase = 9,

    ReadInventoryRequests = 10,
    CreateInventoryRequest = 11,
    UpdateInventoryRequest = 12,
    DeleteInventoryRequest = 13,

    ReadCustomers = 14,
    CreateCustomer = 15,
    UpdateCustomer = 16,

    ReadCustomerMeetings = 17,
    CreateCustomerMeeting = 18,
    UpdateCustomerMeeting = 19,

    ReadSales = 20,
    CreateSale = 21,
    UpdateSale = 22,

    AccessPerformanceMetricsStatistics = 23,
    AccessOperationalMetricsStatistics = 24
}