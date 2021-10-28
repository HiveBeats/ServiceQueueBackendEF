using Microsoft.EntityFrameworkCore.Migrations;

namespace LocateBackend.Migrations
{
    public partial class MessageSolvedIdx : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_DateSolved",
                table: "Messages",
                column: "DateSolved");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_DateSolved",
                table: "Messages");
        }
    }
}
