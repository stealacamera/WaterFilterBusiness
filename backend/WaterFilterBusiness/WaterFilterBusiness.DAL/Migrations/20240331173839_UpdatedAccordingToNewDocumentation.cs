using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WaterFilterBusiness.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAccordingToNewDocumentation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Customers",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(70)",
                oldMaxLength: 70);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Customers",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsQualified",
                table: "Customers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Profession",
                table: "Customers",
                type: "nvarchar(65)",
                maxLength: 65,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CallOutcomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallOutcomes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerChangesHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OldFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldProfession = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldIsQualified = table.Column<bool>(type: "bit", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerChangesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerChangesHistory_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DaysOfWeek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaysOfWeek", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryRequestStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRequestStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeetingOutcomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingOutcomes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedListedCustomers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedListedCustomers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_RedListedCustomers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledCalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PhoneAgentId = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledCalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledCalls_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledCalls_Users_PhoneAgentId",
                        column: x => x.PhoneAgentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCallsHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OutcomeId = table.Column<int>(type: "int", nullable: false),
                    OccuredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCallsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCallsHistory_CallOutcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "CallOutcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCallsHistory_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    CommissionTypeId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(210)", maxLength: 210, nullable: false),
                    WorkerId = table.Column<int>(type: "int", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commissions_CommissionTypes_CommissionTypeId",
                        column: x => x.CommissionTypeId,
                        principalTable: "CommissionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_Users_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesAgentsSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesAgentId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeekId = table.Column<int>(type: "int", nullable: false),
                    BeginHour = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndHour = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAgentsSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesAgentsSchedules_DaysOfWeek_DayOfWeekId",
                        column: x => x.DayOfWeekId,
                        principalTable: "DaysOfWeek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesAgentsSchedules_Users_SalesAgentId",
                        column: x => x.SalesAgentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToolId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConcludedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestNote = table.Column<string>(type: "nvarchar(210)", maxLength: 210, nullable: false),
                    ConclusionNote = table.Column<string>(type: "nvarchar(210)", maxLength: 210, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRequests_InventoryItems_ToolId",
                        column: x => x.ToolId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryRequests_InventoryRequestStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "InventoryRequestStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItemMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToolId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    GiverId = table.Column<int>(type: "int", nullable: false),
                    FromInventoryId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    ToInventoryId = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItemMovements_InventoryItems_ToolId",
                        column: x => x.ToolId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItemMovements_InventoryTypes_FromInventoryId",
                        column: x => x.FromInventoryId,
                        principalTable: "InventoryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItemMovements_InventoryTypes_ToInventoryId",
                        column: x => x.ToInventoryId,
                        principalTable: "InventoryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItemMovements_Users_GiverId",
                        column: x => x.GiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryItemMovements_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesAgentsMeetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneOperatorId = table.Column<int>(type: "int", nullable: false),
                    SalesAgentId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeetingOutcomeId = table.Column<int>(type: "int", nullable: false),
                    InitialNotes = table.Column<string>(type: "nvarchar(210)", maxLength: 210, nullable: false),
                    Afternotes = table.Column<string>(type: "nvarchar(210)", maxLength: 210, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAgentsMeetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesAgentsMeetings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesAgentsMeetings_MeetingOutcomes_MeetingOutcomeId",
                        column: x => x.MeetingOutcomeId,
                        principalTable: "MeetingOutcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesAgentsMeetings_Users_PhoneOperatorId",
                        column: x => x.PhoneOperatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesAgentsMeetings_Users_SalesAgentId",
                        column: x => x.SalesAgentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommissionRequests",
                columns: table => new
                {
                    CommissionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionRequests", x => x.CommissionId);
                    table.ForeignKey(
                        name: "FK_CommissionRequests_Commissions_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "Commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesAgentScheduleChanges_History",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    OldDayOfWeekId = table.Column<int>(type: "int", nullable: false),
                    OldBeginHour = table.Column<TimeOnly>(type: "time", nullable: true),
                    OldEndHour = table.Column<TimeOnly>(type: "time", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAgentScheduleChanges_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesAgentScheduleChanges_History_DaysOfWeek_OldDayOfWeekId",
                        column: x => x.OldDayOfWeekId,
                        principalTable: "DaysOfWeek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesAgentScheduleChanges_History_SalesAgentsSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "SalesAgentsSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmallInventoryRequests",
                columns: table => new
                {
                    InventoryRequestId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmallInventoryRequests", x => x.InventoryRequestId);
                    table.ForeignKey(
                        name: "FK_SmallInventoryRequests_InventoryRequests_InventoryRequestId",
                        column: x => x.InventoryRequestId,
                        principalTable: "InventoryRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianInventoryRequests",
                columns: table => new
                {
                    InventoryRequestId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianInventoryRequests", x => x.InventoryRequestId);
                    table.ForeignKey(
                        name: "FK_TechnicianInventoryRequests_InventoryRequests_InventoryRequestId",
                        column: x => x.InventoryRequestId,
                        principalTable: "InventoryRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicianInventoryRequests_Users_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    MeetingId = table.Column<int>(type: "int", nullable: false),
                    PaymentTypeId = table.Column<int>(type: "int", nullable: false),
                    UpfrontPaymentAmount = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerificationNote = table.Column<string>(type: "nvarchar(210)", maxLength: 210, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.MeetingId);
                    table.ForeignKey(
                        name: "FK_Sales_PaymentTypes_PaymentTypeId",
                        column: x => x.PaymentTypeId,
                        principalTable: "PaymentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sales_SalesAgentsMeetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "SalesAgentsMeetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientDebts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    DeadlineAt = table.Column<DateOnly>(type: "date", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDebts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientDebts_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "MeetingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CallOutcomes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Success" },
                    { 2, "No answer" },
                    { 3, "Rescheduled" },
                    { 4, "Excessive argument" }
                });

            migrationBuilder.InsertData(
                table: "CommissionTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Customer added by sales agent" },
                    { 2, "Monthly-payment contract created" },
                    { 3, "Upfront contract payment" },
                    { 4, "Monthly sales target reached" },
                    { 5, "Water filter installed" }
                });

            migrationBuilder.InsertData(
                table: "DaysOfWeek",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Sunday" },
                    { 2, "Monday" },
                    { 3, "Tuesday" },
                    { 4, "Wednesday" },
                    { 5, "Thursday" },
                    { 6, "Friday" },
                    { 7, "Saturday" }
                });

            migrationBuilder.InsertData(
                table: "InventoryRequestStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "In progress" },
                    { 3, "Completed" },
                    { 4, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "InventoryTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Technician inventory" },
                    { 2, "Small inventory" },
                    { 3, "Big inventory" }
                });

            migrationBuilder.InsertData(
                table: "MeetingOutcomes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Successful" },
                    { 2, "Client cancelled" },
                    { 3, "Agent cancelled" }
                });

            migrationBuilder.InsertData(
                table: "PaymentTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Monthly payments" },
                    { 2, "Full payment upfront" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_FullName",
                table: "Customers",
                column: "FullName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientDebts_SaleId",
                table: "ClientDebts",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_CommissionTypeId",
                table: "Commissions",
                column: "CommissionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_WorkerId",
                table: "Commissions",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCallsHistory_CustomerId",
                table: "CustomerCallsHistory",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCallsHistory_OutcomeId",
                table: "CustomerCallsHistory",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerChangesHistory_CustomerId",
                table: "CustomerChangesHistory",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemMovements_FromInventoryId",
                table: "InventoryItemMovements",
                column: "FromInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemMovements_GiverId",
                table: "InventoryItemMovements",
                column: "GiverId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemMovements_ReceiverId",
                table: "InventoryItemMovements",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemMovements_ToInventoryId",
                table: "InventoryItemMovements",
                column: "ToInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemMovements_ToolId",
                table: "InventoryItemMovements",
                column: "ToolId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_StatusId",
                table: "InventoryRequests",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_ToolId",
                table: "InventoryRequests",
                column: "ToolId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PaymentTypeId",
                table: "Sales",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgentScheduleChanges_History_OldDayOfWeekId",
                table: "SalesAgentScheduleChanges_History",
                column: "OldDayOfWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgentScheduleChanges_History_ScheduleId",
                table: "SalesAgentScheduleChanges_History",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgentsMeetings_CustomerId",
                table: "SalesAgentsMeetings",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgentsMeetings_MeetingOutcomeId",
                table: "SalesAgentsMeetings",
                column: "MeetingOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgentsMeetings_PhoneOperatorId",
                table: "SalesAgentsMeetings",
                column: "PhoneOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgentsMeetings_SalesAgentId",
                table: "SalesAgentsMeetings",
                column: "SalesAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgentsSchedules_DayOfWeekId_BeginHour_SalesAgentId",
                table: "SalesAgentsSchedules",
                columns: new[] { "DayOfWeekId", "BeginHour", "SalesAgentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgentsSchedules_SalesAgentId",
                table: "SalesAgentsSchedules",
                column: "SalesAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledCalls_CustomerId",
                table: "ScheduledCalls",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledCalls_PhoneAgentId",
                table: "ScheduledCalls",
                column: "PhoneAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianInventoryRequests_TechnicianId",
                table: "TechnicianInventoryRequests",
                column: "TechnicianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientDebts");

            migrationBuilder.DropTable(
                name: "CommissionRequests");

            migrationBuilder.DropTable(
                name: "CustomerCallsHistory");

            migrationBuilder.DropTable(
                name: "CustomerChangesHistory");

            migrationBuilder.DropTable(
                name: "InventoryItemMovements");

            migrationBuilder.DropTable(
                name: "RedListedCustomers");

            migrationBuilder.DropTable(
                name: "SalesAgentScheduleChanges_History");

            migrationBuilder.DropTable(
                name: "ScheduledCalls");

            migrationBuilder.DropTable(
                name: "SmallInventoryRequests");

            migrationBuilder.DropTable(
                name: "TechnicianInventoryRequests");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "CallOutcomes");

            migrationBuilder.DropTable(
                name: "InventoryTypes");

            migrationBuilder.DropTable(
                name: "SalesAgentsSchedules");

            migrationBuilder.DropTable(
                name: "InventoryRequests");

            migrationBuilder.DropTable(
                name: "PaymentTypes");

            migrationBuilder.DropTable(
                name: "SalesAgentsMeetings");

            migrationBuilder.DropTable(
                name: "CommissionTypes");

            migrationBuilder.DropTable(
                name: "DaysOfWeek");

            migrationBuilder.DropTable(
                name: "InventoryRequestStatuses");

            migrationBuilder.DropTable(
                name: "MeetingOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_Customers_FullName",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsQualified",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Profession",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Customers",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(70)",
                oldMaxLength: 70,
                oldNullable: true);
        }
    }
}
