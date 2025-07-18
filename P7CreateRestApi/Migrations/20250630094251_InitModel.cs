using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P7CreateRestApi.Migrations
{
    public partial class InitModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BidLists",
                columns: table => new
                {
                    BidListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BidType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BidQuantity = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    AskQuantity = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Bid = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Ask = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Benchmark = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    BidListDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Commentary = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Security = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Trader = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Book = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    CreationName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevisionName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    RevisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DealName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    DealType = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    SourceListId = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Side = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidLists", x => x.BidListId);
                });

            migrationBuilder.CreateTable(
                name: "CurvePoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurveId = table.Column<byte>(type: "tinyint", nullable: true),
                    AsOfDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Term = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurvePoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoodysRating = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    SandPRating = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    FitchRating = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    OrderNumber = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RuleNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Json = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Template = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    SqlStr = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    SqlPart = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    TradeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BuyQuantity = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SellQuantity = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    BuyPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SellPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Benchmark = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    TradeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Security = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Trader = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Book = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    CreationName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevisionName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    RevisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DealName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    DealType = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    SourceListId = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: true),
                    Side = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.TradeId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(125)", maxLength: 125, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BidLists");

            migrationBuilder.DropTable(
                name: "CurvePoints");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "RuleNames");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
