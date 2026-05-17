using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LittleHairSalon.Models;

public partial class LittleHairSalonContext : DbContext
{
    public LittleHairSalonContext()
    {
    }

    public LittleHairSalonContext(DbContextOptions<LittleHairSalonContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDon> ChiTietDons { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<Hang> Hangs { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }
    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public DbSet<Course> Courses { get; set; }
    public DbSet<DichVu> DichVus { get; set; }
    public DbSet<DanhGia> DanhGias { get; set; }
    public DbSet<NhanVien> NhanViens { get; set; }
    public DbSet<LichDat> LichDats { get; set; }

    // Đổi dòng hiện tại thành thế này:
    public virtual DbSet<Registration> Registrations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)                   
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=LittleHairSalonDB;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;Connection Timeout=60;MultipleActiveResultSets=true");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDon>(entity =>
        {
            modelBuilder.Entity<DichVu>()
        .Property(d => d.Gia)
        .HasColumnType("decimal(18,2)");

            // Cấu hình tổng tiền cho đơn hàng
            modelBuilder.Entity<DonHang>()
                .Property(d => d.TongTien)
                .HasColumnType("decimal(18,2)");
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietD__CDF0A1141442DD17");

            entity.ToTable("ChiTietDon");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaDonNavigation).WithMany(p => p.ChiTietDons)
                .HasForeignKey(d => d.MaDon)
                .HasConstraintName("FK__ChiTietDo__MaDon__52593CB8");

            entity.HasOne(d => d.MaHangNavigation).WithMany(p => p.ChiTietDons)
                .HasForeignKey(d => d.MaHang)
                .HasConstraintName("FK__ChiTietDo__MaHan__534D60F1");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDon).HasName("PK__DonHang__3D89F568A1A9CF9C");

            entity.ToTable("DonHang");

            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiNhan).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .HasColumnName("SDT");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Mới đặt");
        });

        modelBuilder.Entity<Hang>(entity =>
        {
            entity.HasKey(e => e.MaHang).HasName("PK__Hang__19C0DB1D8F92390E");

            entity.ToTable("Hang");

            entity.Property(e => e.Gia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TenHang).HasMaxLength(200);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.Hangs)
                .HasForeignKey(d => d.MaLoai)
                .HasConstraintName("FK__Hang__MaLoai__4BAC3F29");
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__Loai__730A57597318615E");

            entity.ToTable("Loai");

            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });
        modelBuilder.Entity<DanhGia>(entity =>
        {
            entity.ToTable("DanhGias");
            entity.HasKey(e => e.MaDanhGia);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
