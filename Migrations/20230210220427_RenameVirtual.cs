using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactBook.Migrations
{
    /// <inheritdoc />
    public partial class RenameVirtual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryContact_Categories_CatorgoriesId",
                table: "CategoryContact");

            migrationBuilder.RenameColumn(
                name: "CatorgoriesId",
                table: "CategoryContact",
                newName: "CategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryContact_Categories_CategoriesId",
                table: "CategoryContact",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryContact_Categories_CategoriesId",
                table: "CategoryContact");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryContact",
                newName: "CatorgoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryContact_Categories_CatorgoriesId",
                table: "CategoryContact",
                column: "CatorgoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
