using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shippings.Modules.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shipment_Tracking_Events_ShipmentId",
                table: "Shipment_Tracking_Events");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_Tracking_Events_ShipmentId_OccurredAtUtc",
                table: "Shipment_Tracking_Events",
                columns: new[] { "ShipmentId", "OccurredAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shipment_Tracking_Events_ShipmentId_OccurredAtUtc",
                table: "Shipment_Tracking_Events");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_Tracking_Events_ShipmentId",
                table: "Shipment_Tracking_Events",
                column: "ShipmentId");
        }
    }
}
