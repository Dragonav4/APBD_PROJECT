using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APBD_PROJECT.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    Pesel = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: true),
                    Krs = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Software",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Software", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DiscountType = table.Column<int>(type: "INTEGER", nullable: false),
                    SoftwareId = table.Column<long>(type: "INTEGER", nullable: true),
                    Percentage = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AppliesTo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discounts_Software_SoftwareId",
                        column: x => x.SoftwareId,
                        principalTable: "Software",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SoftwareVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    YearlyPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    SoftwareId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoftwareVersions_Software_SoftwareId",
                        column: x => x.SoftwareId,
                        principalTable: "Software",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RenewalPeriod = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<long>(type: "INTEGER", nullable: false),
                    SoftwareId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Software_SoftwareId",
                        column: x => x.SoftwareId,
                        principalTable: "Software",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SupportYears = table.Column<int>(type: "INTEGER", nullable: false),
                    IsSigned = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<long>(type: "INTEGER", nullable: false),
                    SoftwareVersionId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_SoftwareVersions_SoftwareVersionId",
                        column: x => x.SoftwareVersionId,
                        principalTable: "SoftwareVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionDiscounts",
                columns: table => new
                {
                    DiscountsId = table.Column<long>(type: "INTEGER", nullable: false),
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionDiscounts", x => new { x.DiscountsId, x.SubscriptionId });
                    table.ForeignKey(
                        name: "FK_SubscriptionDiscounts_Discounts_DiscountsId",
                        column: x => x.DiscountsId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionDiscounts_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubscriptionId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionPayments_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractDiscounts",
                columns: table => new
                {
                    ContractId = table.Column<long>(type: "INTEGER", nullable: false),
                    DiscountsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDiscounts", x => new { x.ContractId, x.DiscountsId });
                    table.ForeignKey(
                        name: "FK_ContractDiscounts_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractDiscounts_Discounts_DiscountsId",
                        column: x => x.DiscountsId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContractId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Address", "CompanyName", "Email", "FirstName", "IsSoftDeleted", "Krs", "LastName", "Pesel", "Phone" },
                values: new object[,]
                {
                    { 1L, "ul. Przykładowa 1, Warszawa", null, "jan.nowak@example.com", "Jan", false, null, "Nowak", "85010112345", "123456789" },
                    { 2L, "ul. Firmowa 2, Kraków", "Example Company", "contact@example.com", null, false, "0000123456", null, null, "987654321" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "PasswordHash", "Role", "Username" },
                values: new object[] { 1L, "hash", 1, "testuser" });

            migrationBuilder.InsertData(
                table: "Software",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { 1L, "Utilities", "A test software", "Test Software" },
                    { 2L, "Finance", "Comprehensive accounting software for small businesses", "Accounting Suite" },
                    { 3L, "Productivity", "Tool for tracking and managing project tasks", "Project Manager" }
                });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "Id", "AppliesTo", "DiscountType", "EndDate", "Name", "Percentage", "SoftwareId", "StartDate" },
                values: new object[] { 1L, 1, 0, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Test Discount", 10m, 1L, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "SoftwareVersions",
                columns: new[] { "Id", "SoftwareId", "Version", "YearlyPrice" },
                values: new object[] { 1L, 1L, "1.0.0", 100m });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "ClientId", "IsActive", "Price", "RenewalPeriod", "SoftwareId", "StartDate" },
                values: new object[] { 1L, 2L, true, 100m, 1, 1L, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Contracts",
                columns: new[] { "Id", "ClientId", "EndDate", "IsSigned", "Price", "SoftwareVersionId", "StartDate", "SupportYears" },
                values: new object[] { 1L, 1L, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 1000m, 1L, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "SubscriptionPayments",
                columns: new[] { "Id", "Amount", "PaymentDate", "SubscriptionId" },
                values: new object[] { 1L, 100m, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "ContractId", "PaymentDate" },
                values: new object[] { 1L, 1000m, 1L, new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Krs",
                table: "Clients",
                column: "Krs",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Pesel",
                table: "Clients",
                column: "Pesel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractDiscounts_DiscountsId",
                table: "ContractDiscounts",
                column: "DiscountsId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientId",
                table: "Contracts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_SoftwareVersionId",
                table: "Contracts",
                column: "SoftwareVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_SoftwareId",
                table: "Discounts",
                column: "SoftwareId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ContractId",
                table: "Payments",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_SoftwareVersions_SoftwareId",
                table: "SoftwareVersions",
                column: "SoftwareId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionDiscounts_SubscriptionId",
                table: "SubscriptionDiscounts",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPayments_SubscriptionId",
                table: "SubscriptionPayments",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ClientId",
                table: "Subscriptions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SoftwareId",
                table: "Subscriptions",
                column: "SoftwareId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractDiscounts");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "SubscriptionDiscounts");

            migrationBuilder.DropTable(
                name: "SubscriptionPayments");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "SoftwareVersions");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Software");
        }
    }
}
