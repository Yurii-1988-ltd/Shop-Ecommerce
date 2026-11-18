using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shippings.Modules.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                table: "Shipment_Tracking_Events",
                newName: "OccurredAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccurredAtUtc",
                table: "Shipment_Tracking_Events",
                newName: "OccurredAt");
        }
    }
}
