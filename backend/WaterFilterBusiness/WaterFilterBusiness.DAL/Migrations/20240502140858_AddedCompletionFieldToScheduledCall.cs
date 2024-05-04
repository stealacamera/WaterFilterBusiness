using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterFilterBusiness.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedCompletionFieldToScheduledCall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "TechnicianInventoryRequests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TechnicianInventoryItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SmallInventoryItems");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "InventoryPurchases");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "ClientDebts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BigInventoryItems");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SmallInventoryRequests",
                newName: "RequesterId");

            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "InventoryPurchases",
                newName: "OccurredAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "ScheduledCalls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "VerifiedAt",
                table: "Sales",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "VerificationNote",
                table: "Sales",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210);

            migrationBuilder.AlterColumn<string>(
                name: "RequestNote",
                table: "InventoryRequests",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210);

            migrationBuilder.AlterColumn<string>(
                name: "ConclusionNote",
                table: "InventoryRequests",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "InventoryItems",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleasedAt",
                table: "Commissions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "CommissionRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CompletedAt",
                table: "ClientDebts",
                type: "date",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmallInventoryRequests_RequesterId",
                table: "SmallInventoryRequests",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmallInventoryRequests_Users_RequesterId",
                table: "SmallInventoryRequests",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmallInventoryRequests_Users_RequesterId",
                table: "SmallInventoryRequests");

            migrationBuilder.DropIndex(
                name: "IX_SmallInventoryRequests_RequesterId",
                table: "SmallInventoryRequests");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "ScheduledCalls");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "ClientDebts");

            migrationBuilder.RenameColumn(
                name: "RequesterId",
                table: "SmallInventoryRequests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "InventoryPurchases",
                newName: "CompletedAt");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TechnicianInventoryRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TechnicianInventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SmallInventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "VerifiedAt",
                table: "Sales",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VerificationNote",
                table: "Sales",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestNote",
                table: "InventoryRequests",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConclusionNote",
                table: "InventoryRequests",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "InventoryPurchases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "InventoryItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleasedAt",
                table: "Commissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "CommissionRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "ClientDebts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BigInventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
