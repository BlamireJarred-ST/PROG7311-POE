using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG7311_POE.Migrations
{
    /// <inheritdoc />
    public partial class AddCostZar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostZar",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TransportType",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostZar",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "TransportType",
                table: "ServiceRequests");
        }
    }
}
