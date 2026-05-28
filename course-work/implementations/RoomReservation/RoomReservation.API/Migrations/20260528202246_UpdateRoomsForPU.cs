using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RoomReservation.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomsForPU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Building", "Capacity", "Description", "Floor", "Name" },
                values: new object[] { "Ректорат", 500, "Главната аула на университета. Използва се за тържествени събития, конференции и масови лекции.", 1, "Аула Максима" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Building", "Capacity", "Description", "Name", "RoomType" },
                values: new object[] { "ФМИ", 80, "Просторна лекционна зала с интерактивна дъска и климатик.", "Лекционна зала 1", "Lecture" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Building", "Capacity", "Description", "Floor", "HasProjector", "Name", "RoomType" },
                values: new object[] { "ФМИ", 30, "Оборудвана с 30 работни станции. Използва се за практически упражнения по програмиране.", 2, true, "Компютърна лаборатория 3", "ComputerLab" });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Building", "Capacity", "CreatedAt", "Description", "Floor", "HasProjector", "IsActive", "Name", "RoomType" },
                values: new object[,]
                {
                    { 4, "Физически факултет", 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Малка зала за семинари и групови дискусии.", 1, false, true, "Семинарна зала 7", "Seminar" },
                    { 5, "Педагогически факултет", 60, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Лекционна зала с мултимедийна система и климатик.", 2, true, true, "Лекционна зала А", "Lecture" },
                    { 6, "Химически факултет", 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Специализирана лаборатория с лабораторно оборудване.", 1, false, true, "Лаборатория по химия", "Lab" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Building", "Capacity", "Description", "Floor", "Name" },
                values: new object[] { "Корпус А", 120, "Голяма аудитория с климатик", 2, "А201" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Building", "Capacity", "Description", "Name", "RoomType" },
                values: new object[] { "Корпус Б", 30, "Компютърна зала с 30 работни места", "Б105", "ComputerLab" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Building", "Capacity", "Description", "Floor", "HasProjector", "Name", "RoomType" },
                values: new object[] { "Корпус В", 20, "Семинарна зала", 3, false, "В302", "Seminar" });
        }
    }
}
