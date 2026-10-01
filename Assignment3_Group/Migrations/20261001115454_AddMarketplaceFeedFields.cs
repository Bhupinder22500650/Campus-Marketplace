using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment3_Group.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceFeedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ListingPrice",
                table: "Listings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Listings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ListingCondition",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ListingDate",
                table: "Listings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ListingDescription",
                table: "Listings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ListingTitle",
                table: "Listings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_ListingCategory",
                table: "Listings",
                column: "ListingCategory");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_SellerId",
                table: "Listings",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Categories_ListingCategory",
                table: "Listings",
                column: "ListingCategory",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Users_SellerId",
                table: "Listings",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Categories_ListingCategory",
                table: "Listings");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Users_SellerId",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_ListingCategory",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_SellerId",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ListingCondition",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ListingDate",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ListingDescription",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ListingTitle",
                table: "Listings");

            migrationBuilder.AlterColumn<int>(
                name: "ListingPrice",
                table: "Listings",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
