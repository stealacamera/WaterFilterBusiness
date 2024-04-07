using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterFilterBusiness.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RedListedCustomers");

            migrationBuilder.AddColumn<DateTime>(
                name: "RedListedAt",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhoneAgentId",
                table: "CustomerCallsHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCallsHistory_PhoneAgentId",
                table: "CustomerCallsHistory",
                column: "PhoneAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCallsHistory_Users_PhoneAgentId",
                table: "CustomerCallsHistory",
                column: "PhoneAgentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCallsHistory_Users_PhoneAgentId",
                table: "CustomerCallsHistory");

            migrationBuilder.DropIndex(
                name: "IX_CustomerCallsHistory_PhoneAgentId",
                table: "CustomerCallsHistory");

            migrationBuilder.DropColumn(
                name: "RedListedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PhoneAgentId",
                table: "CustomerCallsHistory");

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
        }
    }
}
