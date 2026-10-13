using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnHub.Data.Migrations
{
    public partial class AddCategoryAndPictureColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryLogoUrl",
                table: "Resources",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Resources",
                type: "TEXT",
                maxLength: 300,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryLogoUrl",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Resources");
        }
    }
}
