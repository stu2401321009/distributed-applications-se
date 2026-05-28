using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomReservation.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFacultyNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "FacultyNumber",
                value: "2401321099");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "FacultyNumber",
                value: "2301001");
        }
    }
}
