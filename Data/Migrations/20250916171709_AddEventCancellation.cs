using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Local_Event_Finder.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCancellation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Events");
        }
    }
}
