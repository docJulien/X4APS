using Microsoft.EntityFrameworkCore.Migrations;

namespace APS.Migrations
{
    public partial class TradeOperations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "TradeOperations",
                columns: table => new
                {
                    Time = table.Column<double>(nullable: false),
                    OurShipName = table.Column<string>(nullable: true),
                    OurShipId = table.Column<string>(nullable: true),
                    ItemSoldId = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    SoldToName = table.Column<string>(nullable: true),
                    SoldToId = table.Column<string>(nullable: true),
                    Sector = table.Column<string>(nullable: true),
                    Faction = table.Column<string>(nullable: true),
                    Money = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeOperations", x => x.Time);
                });

            migrationBuilder.CreateTable(
                name: "Wares",
                columns: table => new
                {
                    WareID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TransportType = table.Column<string>(nullable: true),
                    MarketMinimumPrice = table.Column<double>(nullable: false),
                    MarketAveragePrice = table.Column<double>(nullable: false),
                    MarketMaximumPrice = table.Column<double>(nullable: false),
                    Volume = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wares", x => x.WareID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configurations");

            migrationBuilder.DropTable(
                name: "TradeOperations");

            migrationBuilder.DropTable(
                name: "Wares");
        }
    }
}
