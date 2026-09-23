using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBDMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddEmergencyServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "tblbloodrequirer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HospitalName",
                table: "tblbloodrequirer",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "tblbloodrequirer",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "tblbloodrequirer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitsRequired",
                table: "tblbloodrequirer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Urgency",
                table: "tblbloodrequirer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "tblblooddonars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "tblambulancerequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmbulanceServiceId = table.Column<int>(type: "int", nullable: true),
                    PatientName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PickupAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DestinationHospital = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AmbulanceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Urgency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblambulancerequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblambulances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AmbulanceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    ServiceHours = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblambulances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblbloodbanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    HospitalAffiliation = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OperatingHours = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvailableBloodGroups = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblbloodbanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tblhospitals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmergencyHelpline = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmergencyServices = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TotalEmergencyBeds = table.Column<int>(type: "int", nullable: false),
                    AvailableEmergencyBeds = table.Column<int>(type: "int", nullable: false),
                    TotalIcuBeds = table.Column<int>(type: "int", nullable: false),
                    AvailableIcuBeds = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblhospitals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbloxygenservices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OxygenAvailability = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CylinderCapacity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HomeDeliveryAvailable = table.Column<bool>(type: "bit", nullable: false),
                    ServiceDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbloxygenservices", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblambulancerequests");

            migrationBuilder.DropTable(
                name: "tblambulances");

            migrationBuilder.DropTable(
                name: "tblbloodbanks");

            migrationBuilder.DropTable(
                name: "tblhospitals");

            migrationBuilder.DropTable(
                name: "tbloxygenservices");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "tblbloodrequirer");

            migrationBuilder.DropColumn(
                name: "HospitalName",
                table: "tblbloodrequirer");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "tblbloodrequirer");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "tblbloodrequirer");

            migrationBuilder.DropColumn(
                name: "UnitsRequired",
                table: "tblbloodrequirer");

            migrationBuilder.DropColumn(
                name: "Urgency",
                table: "tblbloodrequirer");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "tblblooddonars");
        }
    }
}
