using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactBook.Migrations
{
    public partial class AddImagesToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Categories",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageType",
                table: "Categories",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "Categories");
        }
    }
}
