using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shippings.Modules.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Shipping_Method",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Shipping_Method_Code",
                table: "Shipping_Method",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shipping_Method_Code",
                table: "Shipping_Method");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Shipping_Method");
        }
    }
}
