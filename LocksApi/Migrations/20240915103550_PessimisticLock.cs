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
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "orders");

            migrationBuilder.AddColumn<int>(
                name: "lock_owner_id",
                table: "orders",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lock_owner_id",
                table: "orders");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "orders",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }
    }
}
