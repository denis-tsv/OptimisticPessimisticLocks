using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LocksApi.Migrations
{
    /// <inheritdoc />
    public partial class PessimisticLockAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lock_owner_id",
                table: "orders");

            migrationBuilder.CreateTable(
                name: "locks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    owner_id = table.Column<int>(type: "integer", nullable: false),
                    entity_type = table.Column<string>(type: "text", nullable: false),
                    entity_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locks", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_locks_entity_type_entity_id",
                table: "locks",
                columns: new[] { "entity_type", "entity_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "locks");

            migrationBuilder.AddColumn<int>(
                name: "lock_owner_id",
                table: "orders",
                type: "integer",
                nullable: true);
        }
    }
}
