using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment3_Group.Migrations
{
    /// <inheritdoc />
    public partial class AddListingContactDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Listings",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhoneNumber",
                table: "Listings",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ContactPhoneNumber",
                table: "Listings");
        }
    }
}
