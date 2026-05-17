using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleHairSalon.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaDon",
                table: "DonHang",
                newName: "MaDH");

            migrationBuilder.RenameColumn(
                name: "MaDon",
                table: "ChiTietDon",
                newName: "MaDH");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietDon_MaDon",
                table: "ChiTietDon",
                newName: "IX_ChiTietDon_MaDH");

            migrationBuilder.AddColumn<int>(
                name: "MaDonHang",
                table: "DonHang",
                type: "int",
                nullable: false,
                defaultValue: 0);

         
            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThoiGianUocTinh = table.Column<int>(type: "int", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    MaNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhanVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KinhNghiem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.MaNhanVien);
                });

            migrationBuilder.CreateTable(
                name: "LichDats",
                columns: table => new
                {
                    MaLichDat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: true),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioDat = table.Column<TimeSpan>(type: "time", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayTaoLich = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichDats", x => x.MaLichDat);
                    table.ForeignKey(
                        name: "FK_LichDats_DichVus_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVus",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichDats_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKh");
                    table.ForeignKey(
                        name: "FK_LichDats_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichDats_MaDichVu",
                table: "LichDats",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_LichDats_MaKhachHang",
                table: "LichDats",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_LichDats_MaNhanVien",
                table: "LichDats",
                column: "MaNhanVien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "LichDats");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropColumn(
                name: "MaDonHang",
                table: "DonHang");

            migrationBuilder.RenameColumn(
                name: "MaDH",
                table: "DonHang",
                newName: "MaDon");

            migrationBuilder.RenameColumn(
                name: "MaDH",
                table: "ChiTietDon",
                newName: "MaDon");

            migrationBuilder.RenameIndex(
                name: "IX_ChiTietDon_MaDH",
                table: "ChiTietDon",
                newName: "IX_ChiTietDon_MaDon");
        }
    }
}
