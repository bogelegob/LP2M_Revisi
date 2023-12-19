using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LP2M_Revisi.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Buku> Bukus { get; set; }

    public virtual DbSet<Detailbuku> Detailbukus { get; set; }

    public virtual DbSet<Hakciptum> Hakcipta { get; set; }

    public virtual DbSet<Hakpaten> Hakpatens { get; set; }

    public virtual DbSet<Jurnal> Jurnals { get; set; }

    public virtual DbSet<Pengabdianmasyarakat> Pengabdianmasyarakats { get; set; }

    public virtual DbSet<Pengguna> Penggunas { get; set; }

    public virtual DbSet<Prodi> Prodis { get; set; }

    public virtual DbSet<Prosiding> Prosidings { get; set; }

    public virtual DbSet<Seminar> Seminars { get; set; }

    public virtual DbSet<Surattuga> Surattugas { get; set; }
    public virtual DbSet<Pengaduan> Pengaduan { get; set; }
    public virtual DbSet<DataModel> Data { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<Buku>(entity =>
        {
            entity.ToTable("buku");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Editby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("editby");
            entity.Property(e => e.Editdate)
                .HasColumnType("datetime")
                .HasColumnName("editdate");
            entity.Property(e => e.Inputby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("inputby");
            entity.Property(e => e.Inputdate)
                .HasColumnType("datetime")
                .HasColumnName("inputdate");
            entity.Property(e => e.Isbn)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("isbn");
            entity.Property(e => e.Judulbuku)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("judulbuku");
            entity.Property(e => e.Penerbit)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("penerbit");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Tahun).HasColumnName("tahun");

            entity.HasOne(d => d.EditbyNavigation).WithMany(p => p.BukuEditbyNavigations)
                .HasForeignKey(d => d.Editby)
                .HasConstraintName("FK_bukus_s2");

            entity.HasOne(d => d.InputbyNavigation).WithMany(p => p.BukuInputbyNavigations)
                .HasForeignKey(d => d.Inputby)
                .HasConstraintName("FK_bukus_s");
        });

        modelBuilder.Entity<Detailbuku>(entity =>
        {
            entity.HasKey(e => new { e.Idbuku, e.Idpengguna });

            entity.ToTable("detailbuku");

            entity.Property(e => e.Idbuku)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idbuku");
            entity.Property(e => e.Idpengguna)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idpengguna");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.IdbukuNavigation).WithMany(p => p.Detailbukus)
                .HasForeignKey(d => d.Idbuku)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_detailbuku_buku");

            entity.HasOne(d => d.IdpenggunaNavigation).WithMany(p => p.Detailbukus)
                .HasForeignKey(d => d.Idpengguna)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_detailbuku_pengguna");
        });

        modelBuilder.Entity<Hakciptum>(entity =>
        {
            entity.ToTable("hakcipta");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Editby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("editby");
            entity.Property(e => e.Editdate)
                .HasColumnType("date")
                .HasColumnName("editdate");
            entity.Property(e => e.Inputby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("inputby");
            entity.Property(e => e.Inputdate)
                .HasColumnType("date")
                .HasColumnName("inputdate");
            entity.Property(e => e.Judul)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("judul");
            entity.Property(e => e.Keterangan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("keterangan");
            entity.Property(e => e.Noaplikasi)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("noaplikasi");
            entity.Property(e => e.Nosertifikat)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nosertifikat");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.EditbyNavigation).WithMany(p => p.HakciptumEditbyNavigations)
                .HasForeignKey(d => d.Editby)
                .HasConstraintName("FK_hakciptas_s2");

            entity.HasOne(d => d.InputbyNavigation).WithMany(p => p.HakciptumInputbyNavigations)
                .HasForeignKey(d => d.Inputby)
                .HasConstraintName("FK_hakciptas_s");

            entity.HasMany(d => d.Idpenggunas).WithMany(p => p.Idhakcipta)
                .UsingEntity<Dictionary<string, object>>(
                    "Detailhakciptum",
                    r => r.HasOne<Pengguna>().WithMany()
                        .HasForeignKey("Idpengguna")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailhakcipta_pengguna"),
                    l => l.HasOne<Hakciptum>().WithMany()
                        .HasForeignKey("Idhakcipta")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailhakcipta_hakcipta"),
                    j =>
                    {
                        j.HasKey("Idhakcipta", "Idpengguna");
                        j.ToTable("detailhakcipta");
                        j.IndexerProperty<string>("Idhakcipta")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idhakcipta");
                        j.IndexerProperty<string>("Idpengguna")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idpengguna");
                    });
        });

        modelBuilder.Entity<Hakpaten>(entity =>
        {
            entity.ToTable("hakpaten");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Editby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("editby");
            entity.Property(e => e.Editdate)
                .HasColumnType("date")
                .HasColumnName("editdate");
            entity.Property(e => e.Inputby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("inputby");
            entity.Property(e => e.Inputdate)
                .HasColumnType("date")
                .HasColumnName("inputdate");
            entity.Property(e => e.Judul)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("judul");
            entity.Property(e => e.Nopermohonan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nopermohonan");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TanggalPenerimaan)
                .HasColumnType("date")
                .HasColumnName("tanggal_penerimaan");

            entity.HasOne(d => d.EditbyNavigation).WithMany(p => p.HakpatenEditbyNavigations)
                .HasForeignKey(d => d.Editby)
                .HasConstraintName("FK_hakpatens_s2");

            entity.HasOne(d => d.InputbyNavigation).WithMany(p => p.HakpatenInputbyNavigations)
                .HasForeignKey(d => d.Inputby)
                .HasConstraintName("FK_hakpatens_s");

            entity.HasMany(d => d.Idpenggunas).WithMany(p => p.Idhakpatens)
                .UsingEntity<Dictionary<string, object>>(
                    "Detailhakpaten",
                    r => r.HasOne<Pengguna>().WithMany()
                        .HasForeignKey("Idpengguna")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailhakpaten_pengguna"),
                    l => l.HasOne<Hakpaten>().WithMany()
                        .HasForeignKey("Idhakpaten")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailhakpaten_hakpaten"),
                    j =>
                    {
                        j.HasKey("Idhakpaten", "Idpengguna");
                        j.ToTable("detailhakpaten");
                        j.IndexerProperty<string>("Idhakpaten")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idhakpaten");
                        j.IndexerProperty<string>("Idpengguna")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idpengguna");
                    });
        });

        modelBuilder.Entity<Jurnal>(entity =>
        {
            entity.ToTable("jurnal");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Editby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("editby");
            entity.Property(e => e.Editdate)
                .HasColumnType("datetime")
                .HasColumnName("editdate");
            entity.Property(e => e.Halamanakhir).HasColumnName("halamanakhir");
            entity.Property(e => e.Halamanawal).HasColumnName("halamanawal");
            entity.Property(e => e.Inputby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("inputby");
            entity.Property(e => e.Inputdate)
                .HasColumnType("datetime")
                .HasColumnName("inputdate");
            entity.Property(e => e.Issn)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("issn");
            entity.Property(e => e.Judulmakalah)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("judulmakalah");
            entity.Property(e => e.Kategori)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("kategori");
            entity.Property(e => e.Namajurnal)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("namajurnal");
            entity.Property(e => e.Nomor).HasColumnName("nomor");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("url");
            entity.Property(e => e.Volume).HasColumnName("volume");

            entity.HasOne(d => d.EditbyNavigation).WithMany(p => p.JurnalEditbyNavigations)
                .HasForeignKey(d => d.Editby)
                .HasConstraintName("FK_jurnals_s2");

            entity.HasOne(d => d.InputbyNavigation).WithMany(p => p.JurnalInputbyNavigations)
                .HasForeignKey(d => d.Inputby)
                .HasConstraintName("FK_jurnals_s");

            entity.HasMany(d => d.Idpenggunas).WithMany(p => p.Idjurnals)
                .UsingEntity<Dictionary<string, object>>(
                    "Detailjurnal",
                    r => r.HasOne<Pengguna>().WithMany()
                        .HasForeignKey("Idpengguna")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailjurnal_pengguna"),
                    l => l.HasOne<Jurnal>().WithMany()
                        .HasForeignKey("Idjurnal")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailjurnal_jurnal"),
                    j =>
                    {
                        j.HasKey("Idjurnal", "Idpengguna");
                        j.ToTable("detailjurnal");
                        j.IndexerProperty<string>("Idjurnal")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idjurnal");
                        j.IndexerProperty<string>("Idpengguna")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idpengguna");
                    });
        });

        modelBuilder.Entity<Pengabdianmasyarakat>(entity =>
        {
            entity.ToTable("pengabdianmasyarakat");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Buktipendukung).HasColumnName("buktipendukung");
            entity.Property(e => e.Editby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("editby");
            entity.Property(e => e.Editdate)
                .HasColumnType("datetime")
                .HasColumnName("editdate");
            entity.Property(e => e.Inputby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("inputby");
            entity.Property(e => e.Inputdate)
                .HasColumnType("datetime")
                .HasColumnName("inputdate");
            entity.Property(e => e.Jumlahpenerima).HasColumnName("jumlahpenerima");
            entity.Property(e => e.Laporan).HasColumnName("laporan");
            entity.Property(e => e.MahasiswaProdiNim)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("mahasiswa_prodi_nim");
            entity.Property(e => e.Namakegiatan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("namakegiatan");
            entity.Property(e => e.Namafilesurat)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("namafilesurat");
            entity.Property(e => e.Namafilelaporan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("namafilelaporan");
            entity.Property(e => e.Namafilebukti)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("namafilebukti");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Surattugas).HasColumnName("surattugas");
            entity.Property(e => e.Waktupelaksanaan)
                .HasColumnType("date")
                .HasColumnName("waktupelaksanaan");

            entity.HasOne(d => d.EditbyNavigation).WithMany(p => p.PengabdianmasyarakatEditbyNavigations)
                .HasForeignKey(d => d.Editby)
                .HasConstraintName("FK_pengabdianmasyarakats_s2");

            entity.HasOne(d => d.InputbyNavigation).WithMany(p => p.PengabdianmasyarakatInputbyNavigations)
                .HasForeignKey(d => d.Inputby)
                .HasConstraintName("FK_pengabdianmasyarakats_s");

            entity.HasMany(d => d.Idpenggunas).WithMany(p => p.Idpengabdians)
                .UsingEntity<Dictionary<string, object>>(
                    "Detailpengabdian",
                    r => r.HasOne<Pengguna>().WithMany()
                        .HasForeignKey("Idpengguna")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailpengabdian_pengguna"),
                    l => l.HasOne<Pengabdianmasyarakat>().WithMany()
                        .HasForeignKey("Idpengabdian")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailpengabdian_pengabdianmasyarakat"),
                    j =>
                    {
                        j.HasKey("Idpengabdian", "Idpengguna");
                        j.ToTable("detailpengabdian");
                        j.IndexerProperty<string>("Idpengabdian")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idpengabdian");
                        j.IndexerProperty<string>("Idpengguna")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idpengguna");
                    });
        });

        modelBuilder.Entity<Pengguna>(entity =>
        {
            entity.ToTable("pengguna");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Nama)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("nama");
            entity.Property(e => e.Notelepon)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("notelepon");
            entity.Property(e => e.Password)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Prodi).HasColumnName("prodi");
            entity.Property(e => e.Role)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.ProdiNavigation).WithMany(p => p.Penggunas)
                .HasForeignKey(d => d.Prodi)
                .HasConstraintName("FK_pengguna_prodi");

            entity.HasMany(d => d.IdProsidings).WithMany(p => p.IdPenggunas)
                .UsingEntity<Dictionary<string, object>>(
                    "Detailprosiding",
                    r => r.HasOne<Prosiding>().WithMany()
                        .HasForeignKey("IdProsiding")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailprosiding_prosiding"),
                    l => l.HasOne<Pengguna>().WithMany()
                        .HasForeignKey("IdPengguna")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailprosiding_pengguna"),
                    j =>
                    {
                        j.HasKey("IdPengguna", "IdProsiding");
                        j.ToTable("detailprosiding");
                        j.IndexerProperty<string>("IdPengguna")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("id_pengguna");
                        j.IndexerProperty<string>("IdProsiding")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("id_prosiding");
                    });
        });

        modelBuilder.Entity<Prodi>(entity =>
        {
            entity.ToTable("prodi");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nama)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nama");
        });

        modelBuilder.Entity<Prosiding>(entity =>
        {
            entity.ToTable("prosiding");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Editby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("editby");
            entity.Property(e => e.Editdate)
                .HasColumnType("date")
                .HasColumnName("editdate");
            entity.Property(e => e.Inputby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("inputby");
            entity.Property(e => e.Inputdate)
                .HasColumnType("date")
                .HasColumnName("inputdate");
            entity.Property(e => e.Judulpaper)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("judulpaper");
            entity.Property(e => e.Judulprogram)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("judulprogram");
            entity.Property(e => e.Kategori)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("kategori");
            entity.Property(e => e.Keterangan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("keterangan");
            entity.Property(e => e.Penyelenggara)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("penyelenggara");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Tempatpelaksanaan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("tempatpelaksanaan");
            entity.Property(e => e.Waktuterbit)
                .HasColumnType("date")
                .HasColumnName("waktuterbit");

            entity.HasOne(d => d.EditbyNavigation).WithMany(p => p.ProsidingEditbyNavigations)
                .HasForeignKey(d => d.Editby)
                .HasConstraintName("FK_prosidings_s2");

            entity.HasOne(d => d.InputbyNavigation).WithMany(p => p.ProsidingInputbyNavigations)
                .HasForeignKey(d => d.Inputby)
                .HasConstraintName("FK_prosidings_s");
        });

        modelBuilder.Entity<Seminar>(entity =>
        {
            entity.ToTable("seminar");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Editby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("editby");
            entity.Property(e => e.Editdate)
                .HasColumnType("date")
                .HasColumnName("editdate");
            entity.Property(e => e.Inputby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("inputby");
            entity.Property(e => e.Inputdate)
                .HasColumnType("date")
                .HasColumnName("inputdate");
            entity.Property(e => e.Judulpaper)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("judulpaper");
            entity.Property(e => e.Judulprogram)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("judulprogram");
            entity.Property(e => e.Kategori)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("kategori");
            entity.Property(e => e.Keterangan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("keterangan");
            entity.Property(e => e.Penyelenggara)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("penyelenggara");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Tempatpelaksanaan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("tempatpelaksanaan");
            entity.Property(e => e.Waktupelaksanaan)
                .HasColumnType("date")
                .HasColumnName("waktupelaksanaan");

            entity.HasOne(d => d.EditbyNavigation).WithMany(p => p.SeminarEditbyNavigations)
                .HasForeignKey(d => d.Editby)
                .HasConstraintName("FK_seminars_s2");

            entity.HasOne(d => d.InputbyNavigation).WithMany(p => p.SeminarInputbyNavigations)
                .HasForeignKey(d => d.Inputby)
                .HasConstraintName("FK_seminars_s");

            entity.HasMany(d => d.Idpenggunas).WithMany(p => p.Idseminars)
                .UsingEntity<Dictionary<string, object>>(
                    "Detailseminar",
                    r => r.HasOne<Pengguna>().WithMany()
                        .HasForeignKey("Idpengguna")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailseminar_pengguna"),
                    l => l.HasOne<Seminar>().WithMany()
                        .HasForeignKey("Idseminar")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailseminar_seminar"),
                    j =>
                    {
                        j.HasKey("Idseminar", "Idpengguna");
                        j.ToTable("detailseminar");
                        j.IndexerProperty<string>("Idseminar")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idseminar");
                        j.IndexerProperty<string>("Idpengguna")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idpengguna");
                    });
        });

        modelBuilder.Entity<Surattuga>(entity =>
        {
            entity.ToTable("surattugas");

            entity.Property(e => e.Id)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Namafile)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("namafile");
            entity.Property(e => e.Namafilesurat)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("namafilesurat");
            entity.Property(e => e.Keterangan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("keterangan");
            entity.Property(e => e.Buktipendukung).HasColumnName("buktipendukung");
            entity.Property(e => e.Editby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("editby");
            entity.Property(e => e.Editdate)
                .HasColumnType("datetime")
                .HasColumnName("editdate");
            entity.Property(e => e.Inputby)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("inputby");
            entity.Property(e => e.Inputdate)
                .HasColumnType("datetime")
                .HasColumnName("inputdate");
            entity.Property(e => e.tanggalselesai)
                .HasColumnType("datetime")
                .HasColumnName("tanggalselesai");
            entity.Property(e => e.Masapelaksanaan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("masapelaksanaan");
            entity.Property(e => e.Namakegiatan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("namakegiatan");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Surattugas).HasColumnName("surattugas");

            entity.HasOne(d => d.EditbyNavigation).WithMany(p => p.SurattugaEditbyNavigations)
                .HasForeignKey(d => d.Editby)
                .HasConstraintName("FK_surattugas_s2");

            entity.HasOne(d => d.InputbyNavigation).WithMany(p => p.SurattugaInputbyNavigations)
                .HasForeignKey(d => d.Inputby)
                .HasConstraintName("FK_surattugas_s");

            entity.HasMany(d => d.Idpenggunas).WithMany(p => p.Idsurattugas)
                .UsingEntity<Dictionary<string, object>>(
                    "Detailsurattuga",
                    r => r.HasOne<Pengguna>().WithMany()
                        .HasForeignKey("Idpengguna")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailsurattugas_pengguna"),
                    l => l.HasOne<Surattuga>().WithMany()
                        .HasForeignKey("Idsurattugas")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_detailsurattugas_surattugas"),
                    j =>
                    {
                        j.HasKey("Idsurattugas", "Idpengguna");
                        j.ToTable("detailsurattugas");
                        j.IndexerProperty<string>("Idsurattugas")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idsurattugas");
                        j.IndexerProperty<string>("Idpengguna")
                            .HasMaxLength(6)
                            .IsUnicode(false)
                            .HasColumnName("idpengguna");
                    });
        });

        modelBuilder.Entity<Pengaduan>(entity =>
        {
            entity.ToTable("pengaduan");

            entity.Property(e => e.Id).HasColumnName("idpengaduan");
            entity.Property(e => e.pengguna)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idpengguna");
            entity.Property(e => e.buku)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idbuku");
            entity.Property(e => e.prosiding)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idprosiding");
            entity.Property(e => e.seminar)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idseminar");
            entity.Property(e => e.jurnal)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idjurnal");
            entity.Property(e => e.hakcipta)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idhakcipta");
            entity.Property(e => e.hakpaten)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idhakpaten");
            entity.Property(e => e.pengabdian )
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("idpengabdian");
            entity.Property(e => e.Keterangan)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("keterangan");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.createdate)
                .HasColumnType("datetime")
                .HasColumnName("createdate");
            entity.Property(e => e.updatedate)
                .HasColumnType("datetime")
                .HasColumnName("updatedate");
            entity.HasOne(d => d.BukuNavigation).WithMany(p => p.BukuPengaduanNavigations)
                .HasForeignKey(d => d.buku)
                .HasConstraintName("FK_pengaduan_buku");

            entity.HasOne(d => d.HakciptaNavigation).WithMany(p => p.HakCiptaPengaduanNavigations)
                .HasForeignKey(d => d.hakcipta)
                .HasConstraintName("FK_pengaduan_hakcipta");
            
            entity.HasOne(d => d.HakpatenNavigation).WithMany(p => p.HakPatenPengaduanNavigations)
                .HasForeignKey(d => d.hakpaten)
                .HasConstraintName("FK_pengaduan_hakpaten");

            entity.HasOne(d => d.ProsidingNavigation).WithMany(p => p.ProsidingPengaduanNavigations)
                .HasForeignKey(d => d.prosiding)
                .HasConstraintName("FK_pengaduan_prosiding"); 
            
            entity.HasOne(d => d.PengabdianmasyarakatNavigation).WithMany(p => p.PengabdianPengaduanNavigations)
                .HasForeignKey(d => d.pengabdian)
                .HasConstraintName("FK_pengaduan_pengabdian");

            entity.HasOne(d => d.SeminarNavigation).WithMany(p => p.SeminarPengaduanNavigations)
                .HasForeignKey(d => d.seminar)
                .HasConstraintName("FK_pengaduan_seminar");
            
            entity.HasOne(d => d.JurnalNavigation).WithMany(p => p.JurnalPengaduanNavigations)
                .HasForeignKey(d => d.jurnal)
                .HasConstraintName("FK_pengaduan_jurnal");

            entity.HasOne(d => d.PenggunaNavigation).WithMany(p => p.PenggunaPengaduanNavigations)
                .HasForeignKey(d => d.pengguna)
                .HasConstraintName("FK_pengaduan_pengguna");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
