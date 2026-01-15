using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ldc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWeddingListType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListType",
                table: "WeddingLists",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListType",
                table: "WeddingLists");
        }
    }
}
