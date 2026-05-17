using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleHairSalon.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. COMMENT VÌ LỖI AMBIGUOUS: Cột HinhAnh đã được đổi tên hoặc không tồn tại
            // migrationBuilder.RenameColumn(
            //    name: "HinhAnh",
            //    table: "DichVus",
            //    newName: "AnhDichVu");

            // 2. GIỮ NGUYÊN: Các lệnh AlterColumn này giúp đồng bộ kiểu dữ liệu (nvarchar(50) -> nvarchar(max))
            migrationBuilder.AlterColumn<int>(
                name: "TrangThai",
                table: "NhanViens",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "TenNhanVien",
                table: "NhanViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "KinhNghiem",
                table: "NhanViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AnhDaiDien",
                table: "NhanViens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // 3. COMMENT VÌ LỖI TRÙNG TÊN: Cột ChucVu đã có sẵn trong DB của bạn
            // migrationBuilder.AddColumn<string>(
            //    name: "ChucVu",
            //    table: "NhanViens",
            //    type: "nvarchar(max)",
            //    nullable: true);

            // 4. KIỂM TRA: Nếu chạy lệnh lỗi 'Email' thì hãy quay lại comment dòng này sau
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NhanViens",
                type: "nvarchar(max)",
                nullable: true);

            // 5. GIỮ NGUYÊN: Đây là bảng mới hoàn toàn, cần được tạo
            migrationBuilder.CreateTable(
                name: "DanhGias",
                columns: table => new
                {
                    MaDanhGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoSao = table.Column<int>(type: "int", nullable: false),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGias", x => x.MaDanhGia);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DanhGias");
            migrationBuilder.DropColumn(name: "Email", table: "NhanViens");
            // migrationBuilder.DropColumn(name: "ChucVu", table: "NhanViens");
        }
    }
}