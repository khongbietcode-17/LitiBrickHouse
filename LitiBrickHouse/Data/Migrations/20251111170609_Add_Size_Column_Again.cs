using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitiBrickHouse.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Size_Column_Again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "CustomOptions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "CustomOptions");
        }
    }
}
