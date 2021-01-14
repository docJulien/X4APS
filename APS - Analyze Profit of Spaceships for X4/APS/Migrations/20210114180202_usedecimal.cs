using Microsoft.EntityFrameworkCore.Migrations;

namespace APS.Migrations
{
    public partial class usedecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Volume",
                table: "Wares",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarketMinimumPrice",
                table: "Wares",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarketMaximumPrice",
                table: "Wares",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<decimal>(
                name: "MarketAveragePrice",
                table: "Wares",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Volume",
                table: "Wares",
                type: "double",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "MarketMinimumPrice",
                table: "Wares",
                type: "double",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "MarketMaximumPrice",
                table: "Wares",
                type: "double",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "MarketAveragePrice",
                table: "Wares",
                type: "double",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
