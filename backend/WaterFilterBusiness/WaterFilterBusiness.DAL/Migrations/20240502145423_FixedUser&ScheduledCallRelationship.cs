using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterFilterBusiness.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixedUserScheduledCallRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduledCalls_CustomerId",
                table: "ScheduledCalls");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledCalls_CustomerId",
                table: "ScheduledCalls",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScheduledCalls_CustomerId",
                table: "ScheduledCalls");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledCalls_CustomerId",
                table: "ScheduledCalls",
                column: "CustomerId",
                unique: true);
        }
    }
}
