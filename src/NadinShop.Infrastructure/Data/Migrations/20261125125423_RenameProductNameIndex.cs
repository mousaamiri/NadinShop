using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NadinShop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Products_Name_ProduceDate",
                table: "Products",
                newName: "IX_Products_Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Products_Name",
                table: "Products",
                newName: "IX_Products_Name_ProduceDate");
        }
    }
}
