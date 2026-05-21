using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroEngine.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddBiographyToHero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Heroes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Heroes");
        }
    }
}
