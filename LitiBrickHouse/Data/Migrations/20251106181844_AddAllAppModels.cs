using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitiBrickHouse.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAllAppModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OptionCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    OptionCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomOptions_OptionCategories_OptionCategoryId",
                        column: x => x.OptionCategoryId,
                        principalTable: "OptionCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderCustomLegos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    HairId = table.Column<int>(type: "int", nullable: true),
                    FaceId = table.Column<int>(type: "int", nullable: true),
                    ClothesId = table.Column<int>(type: "int", nullable: true),
                    PantsId = table.Column<int>(type: "int", nullable: true),
                    Accessory1Id = table.Column<int>(type: "int", nullable: true),
                    Accessory2Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCustomLegos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderCustomLegos_CustomOptions_Accessory1Id",
                        column: x => x.Accessory1Id,
                        principalTable: "CustomOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderCustomLegos_CustomOptions_Accessory2Id",
                        column: x => x.Accessory2Id,
                        principalTable: "CustomOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderCustomLegos_CustomOptions_ClothesId",
                        column: x => x.ClothesId,
                        principalTable: "CustomOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderCustomLegos_CustomOptions_FaceId",
                        column: x => x.FaceId,
                        principalTable: "CustomOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderCustomLegos_CustomOptions_HairId",
                        column: x => x.HairId,
                        principalTable: "CustomOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderCustomLegos_CustomOptions_PantsId",
                        column: x => x.PantsId,
                        principalTable: "CustomOptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderCustomLegos_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    CustomOptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_CustomOptions_CustomOptionId",
                        column: x => x.CustomOptionId,
                        principalTable: "CustomOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomOptions_OptionCategoryId",
                table: "CustomOptions",
                column: "OptionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomLegos_Accessory1Id",
                table: "OrderCustomLegos",
                column: "Accessory1Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomLegos_Accessory2Id",
                table: "OrderCustomLegos",
                column: "Accessory2Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomLegos_ClothesId",
                table: "OrderCustomLegos",
                column: "ClothesId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomLegos_FaceId",
                table: "OrderCustomLegos",
                column: "FaceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomLegos_HairId",
                table: "OrderCustomLegos",
                column: "HairId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomLegos_OrderId",
                table: "OrderCustomLegos",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomLegos_PantsId",
                table: "OrderCustomLegos",
                column: "PantsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_CustomOptionId",
                table: "OrderItems",
                column: "CustomOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderCustomLegos");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropTable(
                name: "CustomOptions");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "OptionCategories");
        }
    }
}
