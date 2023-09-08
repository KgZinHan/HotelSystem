using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Models;

public partial class HotelCoreMvcContext : DbContext
{
    public HotelCoreMvcContext()
    {
    }

    public HotelCoreMvcContext(DbContextOptions<HotelCoreMvcContext> options)
        : base(options)
    {
    }
    public DbSet<HotelRoomModel> HotelRoomModelDBSet { get; set; }
    public virtual DbSet<LinkHotelRoomRoomAmenity> LinkHotelRoomRoomAmenities { get; set; }

    public virtual DbSet<LinkHotelRoomRoomFeature> LinkHotelRoomRoomFeatures { get; set; }

    public virtual DbSet<MsArea> MsAreas { get; set; }

    public virtual DbSet<MsHotelRoom> MsHotelRooms { get; set; }

    public virtual DbSet<MsHotelRoomRate> MsHotelRoomRates { get; set; }

    public virtual DbSet<MsHotelRoomType> MsHotelRoomTypes { get; set; }

    public virtual DbSet<MsHotelinfo> MsHotelinfos { get; set; }

    public virtual DbSet<MsHotellocation> MsHotellocations { get; set; }

    public virtual DbSet<MsHotelroombedding> MsHotelroombeddings { get; set; }

    public virtual DbSet<MsHotelroomstate> MsHotelroomstates { get; set; }

    public virtual DbSet<MsRoomAmenity> MsRoomAmenities { get; set; }

    public virtual DbSet<MsRoomFeature> MsRoomFeatures { get; set; }

    public virtual DbSet<MsTownship> MsTownships { get; set; }

    public virtual DbSet<MsUser> MsUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=HotelCoreMVCConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HotelRoomModel>(entity => entity.HasNoKey());
        modelBuilder.Entity<LinkHotelRoomRoomAmenity>(entity =>
        {
            entity.ToTable("link_HotelRoom_RoomAmenities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rmamtyid).HasColumnName("rmamtyid");
            entity.Property(e => e.Roomid).HasColumnName("roomid");
        });

        modelBuilder.Entity<LinkHotelRoomRoomFeature>(entity =>
        {
            entity.ToTable("link_HotelRoom_RoomFeatures");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rmfeatureid).HasColumnName("rmfeatureid");
            entity.Property(e => e.Roomid).HasColumnName("roomid");
        });

        modelBuilder.Entity<MsArea>(entity =>
        {
            entity.HasKey(e => e.Areaid);

            entity.ToTable("ms_area");

            entity.Property(e => e.Areaid).HasColumnName("areaid");
            entity.Property(e => e.Areacde)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("areacde");
            entity.Property(e => e.Areadesc)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("areadesc");
        });

        modelBuilder.Entity<MsHotelRoom>(entity =>
        {
            entity.HasKey(e => e.Roomid);

            entity.ToTable("ms_HotelRoom");

            entity.Property(e => e.Roomid).HasColumnName("roomid");
            entity.Property(e => e.Bedid).HasColumnName("bedid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Guestactivemsg)
                .HasMaxLength(255)
                .HasColumnName("guestactivemsg");
            entity.Property(e => e.Isautoapplyrate).HasColumnName("isautoapplyrate");
            entity.Property(e => e.Isdnd).HasColumnName("isdnd");
            entity.Property(e => e.Isguestin).HasColumnName("isguestin");
            entity.Property(e => e.Locid).HasColumnName("locid");
            entity.Property(e => e.Paxno).HasColumnName("paxno");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmtypid).HasColumnName("rmtypid");
            entity.Property(e => e.Roomno)
                .HasMaxLength(20)
                .HasColumnName("roomno");
            entity.Property(e => e.Roomtelno)
                .HasMaxLength(20)
                .HasColumnName("roomtelno");
            entity.Property(e => e.Usefixprice)
                .HasColumnType("money")
                .HasColumnName("usefixprice");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsHotelRoomRate>(entity =>
        {
            entity.HasKey(e => e.Rmrateid);

            entity.ToTable("ms_HotelRoomRate");

            entity.Property(e => e.Rmrateid).HasColumnName("rmrateid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Daymonthend)
                .HasColumnType("date")
                .HasColumnName("daymonthend");
            entity.Property(e => e.Daymonthstart)
                .HasColumnType("date")
                .HasColumnName("daymonthstart");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmratecde)
                .HasMaxLength(20)
                .HasColumnName("rmratecde");
            entity.Property(e => e.Rmratedesc)
                .HasMaxLength(100)
                .HasColumnName("rmratedesc");
            entity.Property(e => e.Rmtypid).HasColumnName("rmtypid");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsHotelRoomType>(entity =>
        {
            entity.HasKey(e => e.Rmtypid);

            entity.ToTable("ms_HotelRoomType");

            entity.Property(e => e.Rmtypid).HasColumnName("rmtypid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Extrabedprice)
                .HasColumnType("money")
                .HasColumnName("extrabedprice");
            entity.Property(e => e.Paxno).HasColumnName("paxno");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmtypcde)
                .HasMaxLength(20)
                .HasColumnName("rmtypcde");
            entity.Property(e => e.Rmtypdesc)
                .HasMaxLength(50)
                .HasColumnName("rmtypdesc");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsHotelinfo>(entity =>
        {
            entity.HasKey(e => e.Cmpyid);

            entity.ToTable("ms_hotelinfo");

            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Address)
                .HasMaxLength(512)
                .HasColumnName("address");
            entity.Property(e => e.Areaid).HasColumnName("areaid");
            entity.Property(e => e.Autopostflg).HasColumnName("autopostflg");
            entity.Property(e => e.Autoposttime).HasColumnName("autoposttime");
            entity.Property(e => e.Checkintime).HasColumnName("checkintime");
            entity.Property(e => e.Checkouttime).HasColumnName("checkouttime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Hoteldte)
                .HasColumnType("date")
                .HasColumnName("hoteldte");
            entity.Property(e => e.Hotelnme)
                .HasMaxLength(100)
                .HasColumnName("hotelnme");
            entity.Property(e => e.Latecheckintime).HasColumnName("latecheckintime");
            entity.Property(e => e.Phone1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone1");
            entity.Property(e => e.Phone2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone2");
            entity.Property(e => e.Phone3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone3");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Tspid).HasColumnName("tspid");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Website)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("website");
        });

        modelBuilder.Entity<MsHotellocation>(entity =>
        {
            entity.HasKey(e => e.Locid);

            entity.ToTable("ms_hotellocation");

            entity.Property(e => e.Locid).HasColumnName("locid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Isoutlet).HasColumnName("isoutlet");
            entity.Property(e => e.Loccde)
                .HasMaxLength(20)
                .HasColumnName("loccde");
            entity.Property(e => e.Locdesc)
                .HasMaxLength(50)
                .HasColumnName("locdesc");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsHotelroombedding>(entity =>
        {
            entity.HasKey(e => e.Bedid);

            entity.ToTable("ms_hotelroombedding");

            entity.Property(e => e.Bedid).HasColumnName("bedid");
            entity.Property(e => e.Bedcde)
                .HasMaxLength(20)
                .HasColumnName("bedcde");
            entity.Property(e => e.Beddesc)
                .HasMaxLength(50)
                .HasColumnName("beddesc");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsHotelroomstate>(entity =>
        {
            entity.HasKey(e => e.Rmstateid);

            entity.ToTable("ms_hotelroomstate");

            entity.Property(e => e.Rmstateid).HasColumnName("rmstateid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmstatecde)
                .HasMaxLength(20)
                .HasColumnName("rmstatecde");
            entity.Property(e => e.Rmstatedesc)
                .HasMaxLength(50)
                .HasColumnName("rmstatedesc");
            entity.Property(e => e.Syscolor).HasColumnName("syscolor");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsRoomAmenity>(entity =>
        {
            entity.HasKey(e => e.Rmamtyid);

            entity.ToTable("ms_RoomAmenities");

            entity.Property(e => e.Rmamtyid).HasColumnName("rmamtyid");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmamtycde)
                .HasMaxLength(20)
                .HasColumnName("rmamtycde");
            entity.Property(e => e.Rmamtydesc)
                .HasMaxLength(50)
                .HasColumnName("rmamtydesc");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsRoomFeature>(entity =>
        {
            entity.HasKey(e => e.Rmfeatureid);

            entity.ToTable("ms_RoomFeatures");

            entity.Property(e => e.Rmfeatureid).HasColumnName("rmfeatureid");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmfeaturecde)
                .HasMaxLength(20)
                .HasColumnName("rmfeaturecde");
            entity.Property(e => e.Rmfeaturedesc)
                .HasMaxLength(100)
                .HasColumnName("rmfeaturedesc");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsTownship>(entity =>
        {
            entity.HasKey(e => e.Tspid);

            entity.ToTable("ms_township");

            entity.Property(e => e.Tspid).HasColumnName("tspid");
            entity.Property(e => e.Areaid).HasColumnName("areaid");
            entity.Property(e => e.Tspcde)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tspcde");
            entity.Property(e => e.Tspdesc)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tspdesc");
        });

        modelBuilder.Entity<MsUser>(entity =>
        {
            entity.HasKey(e => e.Userid);

            entity.ToTable("ms_user");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Mnugrpid).HasColumnName("mnugrpid");
            entity.Property(e => e.Pwd).HasColumnName("pwd");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Usercde)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("usercde");
            entity.Property(e => e.Usernme)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usernme");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
