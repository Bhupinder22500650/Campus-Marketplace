using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment3_Group.Migrations
{
    /// <inheritdoc />
    public partial class AddedImagesToListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "ProductImage",
                table: "Listings",
                type: "tinyint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "Listings");
        }
    }
}
