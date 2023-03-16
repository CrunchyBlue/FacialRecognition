using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdersApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAgePredictionDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgePrediction",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgePrediction",
                table: "OrderDetails");
        }
    }
}
