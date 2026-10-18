using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddBloodRequestTrackingToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingToken",
                table: "tblbloodrequirer",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblbloodrequirer_TrackingToken",
                table: "tblbloodrequirer",
                column: "TrackingToken",
                unique: true,
                filter: "[TrackingToken] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tblbloodrequirer_TrackingToken",
                table: "tblbloodrequirer");

            migrationBuilder.DropColumn(
                name: "TrackingToken",
                table: "tblbloodrequirer");
        }
    }
}
