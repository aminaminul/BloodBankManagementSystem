using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class HardenIntegrityAndAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContactNumber",
                table: "tblbloodrequirer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "TrackingToken",
                table: "tblambulancerequests",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "tbladmin",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "tblauditlog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActorRole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActorId = table.Column<int>(type: "int", nullable: true),
                    ActorName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblauditlog", x => x.Id);
                });

            // --- Data repair ---------------------------------------------------------
            // Phone numbers used to live in numeric columns, so 01711223344 became 1711223344.
            migrationBuilder.Sql(@"
UPDATE tblbloodrequirer
SET ContactNumber = '0' + ContactNumber
WHERE LEN(ContactNumber) = 10 AND ContactNumber NOT LIKE '0%';

UPDATE tbladmin
SET MobileNumber = '0' + MobileNumber
WHERE MobileNumber IS NOT NULL AND LEN(MobileNumber) = 10 AND MobileNumber NOT LIKE '0%';
");

            // Existing dispatch requests get an unguessable public tracking token.
            migrationBuilder.Sql(@"
UPDATE tblambulancerequests
SET TrackingToken = REPLACE(CONVERT(varchar(36), NEWID()), '-', '')
WHERE TrackingToken IS NULL;
");

            // Detach requests that point at a donor row which no longer exists, otherwise the
            // new foreign key cannot be created.
            migrationBuilder.Sql(@"
UPDATE r SET r.BloodDonorID = NULL
FROM tblbloodrequirer r
WHERE r.BloodDonorID IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM tblblooddonars d WHERE d.Id = r.BloodDonorID);
");

            migrationBuilder.CreateIndex(
                name: "IX_tbldonationhistory_DonorId",
                table: "tbldonationhistory",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_tblbloodrequirer_BloodDonorID",
                table: "tblbloodrequirer",
                column: "BloodDonorID");

            migrationBuilder.CreateIndex(
                name: "IX_tblblooddonars_EmailId",
                table: "tblblooddonars",
                column: "EmailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblambulancerequests_AmbulanceServiceId",
                table: "tblambulancerequests",
                column: "AmbulanceServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_tblambulancerequests_TrackingToken",
                table: "tblambulancerequests",
                column: "TrackingToken",
                unique: true,
                filter: "[TrackingToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tbladmin_UserName",
                table: "tbladmin",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tblambulancerequests_tblambulances_AmbulanceServiceId",
                table: "tblambulancerequests",
                column: "AmbulanceServiceId",
                principalTable: "tblambulances",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tblbloodrequirer_tblblooddonars_BloodDonorID",
                table: "tblbloodrequirer",
                column: "BloodDonorID",
                principalTable: "tblblooddonars",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_tbldonationhistory_tblblooddonars_DonorId",
                table: "tbldonationhistory",
                column: "DonorId",
                principalTable: "tblblooddonars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblambulancerequests_tblambulances_AmbulanceServiceId",
                table: "tblambulancerequests");

            migrationBuilder.DropForeignKey(
                name: "FK_tblbloodrequirer_tblblooddonars_BloodDonorID",
                table: "tblbloodrequirer");

            migrationBuilder.DropForeignKey(
                name: "FK_tbldonationhistory_tblblooddonars_DonorId",
                table: "tbldonationhistory");

            migrationBuilder.DropTable(
                name: "tblauditlog");

            migrationBuilder.DropIndex(
                name: "IX_tbldonationhistory_DonorId",
                table: "tbldonationhistory");

            migrationBuilder.DropIndex(
                name: "IX_tblbloodrequirer_BloodDonorID",
                table: "tblbloodrequirer");

            migrationBuilder.DropIndex(
                name: "IX_tblblooddonars_EmailId",
                table: "tblblooddonars");

            migrationBuilder.DropIndex(
                name: "IX_tblambulancerequests_AmbulanceServiceId",
                table: "tblambulancerequests");

            migrationBuilder.DropIndex(
                name: "IX_tblambulancerequests_TrackingToken",
                table: "tblambulancerequests");

            migrationBuilder.DropIndex(
                name: "IX_tbladmin_UserName",
                table: "tbladmin");

            migrationBuilder.DropColumn(
                name: "TrackingToken",
                table: "tblambulancerequests");

            migrationBuilder.AlterColumn<long>(
                name: "ContactNumber",
                table: "tblbloodrequirer",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<long>(
                name: "MobileNumber",
                table: "tbladmin",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
