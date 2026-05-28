using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomReservation.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$W7Ayo78qgXOdNnVhxZAD4OA8secPjnSUDCpYC0kR1tGn.XZYnPZ1S");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$8V1JaL/fTbt6D4bGZBbFGeOSJEA1rQqEB1RoJ8byAcSfRkehaAEUe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$.axHjIPkAftJ9.TU5SDNu.3vnxU5R5X0TA2HwQVnljRA3buZ7Dq.W");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$5MYEsX8XnSHHx9IPlMGM7udD5mEPt1DZlnAGvlWMVz/hOo5VWFjCa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$3FgB5FdKl0BZ1U7kY0tEE.CvC9ELyFlr7MCFf/JnYHPiWqRPOBGOi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$H1h8wHjCgdQ2YJj6FQqBx.b0lQvxaZQ3Qx6RJtBE4Xjf1PKBF3GJu");
        }
    }
}
