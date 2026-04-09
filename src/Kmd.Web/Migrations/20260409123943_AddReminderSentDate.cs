using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kmd.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderSentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderSentDate",
                table: "Reservations",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderSentDate",
                table: "Reservations");
        }
    }
}
