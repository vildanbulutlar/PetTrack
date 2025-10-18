using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Temperature_To_ActivityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemperatureC",
                table: "ActivityLogs",
                newName: "Temperature");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Temperature",
                table: "ActivityLogs",
                newName: "TemperatureC");
        }
    }
}
