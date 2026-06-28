using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class editeReleationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_ProductId' AND object_id = OBJECT_ID('OrderItems'))
                    DROP INDEX [IX_OrderItems_ProductId] ON [OrderItems]");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_ProductId' AND object_id = OBJECT_ID('OrderItems'))
                    DROP INDEX [IX_OrderItems_ProductId] ON [OrderItems]");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId",
                unique: true);
        }
    }
}
