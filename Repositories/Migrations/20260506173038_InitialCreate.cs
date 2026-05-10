using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Repositories.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "brands",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_brands", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "categories",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_categories", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "roles",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_roles", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "users",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
					PasswordHash = table.Column<string>(type: "text", nullable: false),
					RoleId = table.Column<int>(type: "integer", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_users", x => x.Id);
					table.ForeignKey(
						name: "FK_users_roles_RoleId",
						column: x => x.RoleId,
						principalTable: "roles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "clothing_items",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
					Description = table.Column<string>(type: "text", nullable: true),
					Color = table.Column<string>(type: "text", nullable: true),
					Size = table.Column<string>(type: "text", nullable: true),
					Season = table.Column<string>(type: "text", nullable: true),
					Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
					ImagePath = table.Column<string>(type: "text", nullable: true),
					CategoryId = table.Column<int>(type: "integer", nullable: false),
					BrandId = table.Column<int>(type: "integer", nullable: false),
					UserId = table.Column<int>(type: "integer", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_clothing_items", x => x.Id);
					table.ForeignKey(
						name: "FK_clothing_items_brands_BrandId",
						column: x => x.BrandId,
						principalTable: "brands",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_clothing_items_categories_CategoryId",
						column: x => x.CategoryId,
						principalTable: "categories",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_clothing_items_users_UserId",
						column: x => x.UserId,
						principalTable: "users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "outfits",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
					UserId = table.Column<int>(type: "integer", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_outfits", x => x.Id);
					table.ForeignKey(
						name: "FK_outfits_users_UserId",
						column: x => x.UserId,
						principalTable: "users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "outfit_items",
				columns: table => new
				{
					OutfitId = table.Column<int>(type: "integer", nullable: false),
					ClothingItemId = table.Column<int>(type: "integer", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_outfit_items", x => new { x.OutfitId, x.ClothingItemId });
					table.ForeignKey(
						name: "FK_outfit_items_clothing_items_ClothingItemId",
						column: x => x.ClothingItemId,
						principalTable: "clothing_items",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_outfit_items_outfits_OutfitId",
						column: x => x.OutfitId,
						principalTable: "outfits",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_brands_Name",
				table: "brands",
				column: "Name",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_categories_Name",
				table: "categories",
				column: "Name",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_clothing_items_BrandId",
				table: "clothing_items",
				column: "BrandId");

			migrationBuilder.CreateIndex(
				name: "IX_clothing_items_CategoryId",
				table: "clothing_items",
				column: "CategoryId");

			migrationBuilder.CreateIndex(
				name: "IX_clothing_items_UserId",
				table: "clothing_items",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_outfit_items_ClothingItemId",
				table: "outfit_items",
				column: "ClothingItemId");

			migrationBuilder.CreateIndex(
				name: "IX_outfits_UserId",
				table: "outfits",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_roles_Name",
				table: "roles",
				column: "Name",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_users_Email",
				table: "users",
				column: "Email",
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_users_RoleId",
				table: "users",
				column: "RoleId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "outfit_items");

			migrationBuilder.DropTable(
				name: "clothing_items");

			migrationBuilder.DropTable(
				name: "outfits");

			migrationBuilder.DropTable(
				name: "brands");

			migrationBuilder.DropTable(
				name: "categories");

			migrationBuilder.DropTable(
				name: "users");

			migrationBuilder.DropTable(
				name: "roles");
		}
	}
}
