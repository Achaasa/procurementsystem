using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace procurementsystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDelFlagToProcurementItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "delFlag",
                table: "ProcurementItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "delFlag",
                table: "ProcurementItems");
        }
    }
}
