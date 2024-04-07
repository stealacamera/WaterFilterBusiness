using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterFilterBusiness.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSalesAgentMeetings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PhoneOperatorId",
                table: "SalesAgentsMeetings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MeetingOutcomeId",
                table: "SalesAgentsMeetings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "InitialNotes",
                table: "SalesAgentsMeetings",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210);

            migrationBuilder.AlterColumn<string>(
                name: "Afternotes",
                table: "SalesAgentsMeetings",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210);

            migrationBuilder.InsertData(
                table: "MeetingOutcomes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Failed" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MeetingOutcomes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<int>(
                name: "PhoneOperatorId",
                table: "SalesAgentsMeetings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MeetingOutcomeId",
                table: "SalesAgentsMeetings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InitialNotes",
                table: "SalesAgentsMeetings",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Afternotes",
                table: "SalesAgentsMeetings",
                type: "nvarchar(210)",
                maxLength: 210,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(210)",
                oldMaxLength: 210,
                oldNullable: true);
        }
    }
}
