using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SampleApi.Migrations
{
    /// <inheritdoc />
    public partial class PessimisticLock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "locked_by_id",
                table: "orders",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "locked_by_id",
                table: "orders");
        }
    }
}
