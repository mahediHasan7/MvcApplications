using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MvcApp1.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedCompnayData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "City 1", "Company 1", "123-456-7890", "12345", "State 1", "1234 Street 1" },
                    { 2, "City 2", "Company 2", "098-765-4321", "54321", "State 2", "4321 Street 2" },
                    { 3, "City 3", "Company 3", "678-901-2345", "67890", "State 3", "6789 Street 3" },
                    { 4, "City 4", "Company 4", "098-765-4321", "09876", "State 4", "0987 Street 4" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
