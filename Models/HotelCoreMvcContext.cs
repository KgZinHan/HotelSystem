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

    public virtual DbSet<LinkHotelRoomRoomAmenity> LinkHotelRoomRoomAmenities { get; set; }

    public virtual DbSet<LinkHotelRoomRoomFeature> LinkHotelRoomRoomFeatures { get; set; }

    public virtual DbSet<MsAgency> MsAgencies { get; set; }

    public virtual DbSet<MsArea> MsAreas { get; set; }

    public virtual DbSet<MsAutonumber> MsAutonumbers { get; set; }

    public virtual DbSet<MsCurrency> MsCurrencies { get; set; }

    public virtual DbSet<MsDepartment> MsDepartments { get; set; }

    public virtual DbSet<MsGuestcountry> MsGuestcountries { get; set; }

    public virtual DbSet<MsGuestdatum> MsGuestdata { get; set; }

    public virtual DbSet<MsGuestnationality> MsGuestnationalities { get; set; }

    public virtual DbSet<MsGuestsalutation> MsGuestsalutations { get; set; }

    public virtual DbSet<MsGueststate> MsGueststates { get; set; }

    public virtual DbSet<MsHotelRoom> MsHotelRooms { get; set; }

    public virtual DbSet<MsHotelRoomRate> MsHotelRoomRates { get; set; }

    public virtual DbSet<MsHotelRoomType> MsHotelRoomTypes { get; set; }

    public virtual DbSet<MsHotelRoomTypeImage> MsHotelRoomTypeImages { get; set; }

    public virtual DbSet<MsHotelinfo> MsHotelinfos { get; set; }

    public virtual DbSet<MsHotellocation> MsHotellocations { get; set; }

    public virtual DbSet<MsHotelroombedding> MsHotelroombeddings { get; set; }

    public virtual DbSet<MsHotelroomstate> MsHotelroomstates { get; set; }

    public virtual DbSet<MsHotelservicegrp> MsHotelservicegrps { get; set; }

    public virtual DbSet<MsHotelservicegrpd> MsHotelservicegrpds { get; set; }

    public virtual DbSet<MsMessageeditor> MsMessageeditors { get; set; }

    public virtual DbSet<MsReason> MsReasons { get; set; }

    public virtual DbSet<MsRoomAmenity> MsRoomAmenities { get; set; }

    public virtual DbSet<MsRoomFeature> MsRoomFeatures { get; set; }

    public virtual DbSet<MsTownship> MsTownships { get; set; }

    public virtual DbSet<MsUser> MsUsers { get; set; }

    public virtual DbSet<PmsCheckin> PmsCheckins { get; set; }

    public virtual DbSet<PmsCheckinroomguest> PmsCheckinroomguests { get; set; }

    public virtual DbSet<PmsGlobalactionlog> PmsGlobalactionlogs { get; set; }

    public virtual DbSet<PmsGuestbilling> PmsGuestbillings { get; set; }

    public virtual DbSet<PmsReservation> PmsReservations { get; set; }

    public virtual DbSet<PmsRoomdailycharge> PmsRoomdailycharges { get; set; }

    public virtual DbSet<PmsRoomfoliod> PmsRoomfoliods { get; set; }

    public virtual DbSet<PmsRoomfolioh> PmsRoomfoliohs { get; set; }

    public virtual DbSet<PmsRoomledger> PmsRoomledgers { get; set; }

    public virtual DbSet<PmsRoomledgerlog> PmsRoomledgerlogs { get; set; }

    public virtual DbSet<Sysconfig> Sysconfigs { get; set; }

    public DbSet<HotelRoomModel> HotelRoomModelDBSet { get; set; }

    public DbSet<AvailableRoomTypeModel> HotelRoomInfoDBSet { get; set; }

    public DbSet<AvailableRoomNumberModel> HotelRoomNumberInfoDBSet { get; set; }

    public DbSet<RoomEnquiryDBModel> RoomEnquiryDBSet { get; set; }

    public DbSet<RoomNoEnquiryDBModel> RoomNoEnquiryDBSet { get; set; }

    public DbSet<GuestDBModel> GuestDBSet { get; set; }

    public DbSet<NightAuditDBModel> NightAuditDBSet { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=HotelCoreMVCConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HotelRoomModel>(builder =>
        {
            builder.HasNoKey();
        });

        modelBuilder.Entity<AvailableRoomTypeModel>(builder =>
        {
            builder.HasNoKey();
        });

        modelBuilder.Entity<AvailableRoomNumberModel>(builder =>
        {
            builder.HasNoKey();
        });

        modelBuilder.Entity<RoomEnquiryDBModel>(builder =>
        {
            builder.HasNoKey();
        });

        modelBuilder.Entity<RoomNoEnquiryDBModel>(builder =>
        {
            builder.HasNoKey();
        });

        modelBuilder.Entity<GuestDBModel>(builder =>
        {
            builder.HasNoKey();
        });

        modelBuilder.Entity<NightAuditDBModel>(builder =>
        {
            builder.HasNoKey();
        });

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

        modelBuilder.Entity<MsAgency>(entity =>
        {
            entity.HasKey(e => e.Agencyid);

            entity.ToTable("ms_agency");

            entity.Property(e => e.Agencyid).HasColumnName("agencyid");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Agencynme)
                .HasMaxLength(100)
                .HasColumnName("agencynme");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Contperson)
                .HasMaxLength(50)
                .HasColumnName("contperson");
            entity.Property(e => e.Creditlimit)
                .HasColumnType("money")
                .HasColumnName("creditlimit");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("website");
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

        modelBuilder.Entity<MsAutonumber>(entity =>
        {
            entity.HasKey(e => e.Autonoid);

            entity.ToTable("ms_autonumber");

            entity.Property(e => e.Autonoid).HasColumnName("autonoid");
            entity.Property(e => e.Allowaccessroom).HasColumnName("allowaccessroom");
            entity.Property(e => e.Billnature)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("billnature");
            entity.Property(e => e.Billprefix)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("billprefix");
            entity.Property(e => e.Billtypcde)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("billtypcde");
            entity.Property(e => e.Bizdte)
                .HasColumnType("date")
                .HasColumnName("bizdte");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Curshift).HasColumnName("curshift");
            entity.Property(e => e.Lastgeneratedte)
                .HasColumnType("date")
                .HasColumnName("lastgeneratedte");
            entity.Property(e => e.Lastusedno).HasColumnName("lastusedno");
            entity.Property(e => e.Noofshift).HasColumnName("noofshift");
            entity.Property(e => e.Posdefloc)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("posdefloc");
            entity.Property(e => e.Posdefsaletyp)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("posdefsaletyp");
            entity.Property(e => e.Posid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("posid");
            entity.Property(e => e.Resetevery)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("resetevery");
            entity.Property(e => e.Runningno).HasColumnName("runningno");
            entity.Property(e => e.Zeroleading).HasColumnName("zeroleading");
        });

        modelBuilder.Entity<MsCurrency>(entity =>
        {
            entity.HasKey(e => e.Currid);

            entity.ToTable("ms_currency");

            entity.Property(e => e.Currid).HasColumnName("currid");
            entity.Property(e => e.Currcde)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("currcde");
            entity.Property(e => e.Currrate)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("currrate");
            entity.Property(e => e.Currtyp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("currtyp");
            entity.Property(e => e.Homeflg).HasColumnName("homeflg");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsDepartment>(entity =>
        {
            entity.HasKey(e => e.Deptcde);

            entity.ToTable("ms_department");

            entity.Property(e => e.Deptcde)
                .HasMaxLength(20)
                .HasColumnName("deptcde");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Deptdesc)
                .HasMaxLength(50)
                .HasColumnName("deptdesc");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsGuestcountry>(entity =>
        {
            entity.HasKey(e => e.Countryid);

            entity.ToTable("ms_guestcountry");

            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Countrydesc)
                .HasMaxLength(50)
                .HasColumnName("countrydesc");
            entity.Property(e => e.Defstateid).HasColumnName("defstateid");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsGuestdatum>(entity =>
        {
            entity.HasKey(e => e.Guestid);

            entity.ToTable("ms_guestdata");

            entity.Property(e => e.Guestid).HasColumnName("guestid");
            entity.Property(e => e.Chrgacccde)
                .HasMaxLength(20)
                .HasColumnName("chrgacccde");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Crlimitamt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("crlimitamt");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("dob");
            entity.Property(e => e.Emailaddr)
                .HasMaxLength(255)
                .HasColumnName("emailaddr");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Guestfullnme)
                .HasMaxLength(100)
                .HasColumnName("guestfullnme");
            entity.Property(e => e.Guestlastnme)
                .HasMaxLength(50)
                .HasColumnName("guestlastnme");
            entity.Property(e => e.Idissuedte)
                .HasColumnType("date")
                .HasColumnName("idissuedte");
            entity.Property(e => e.Idppno)
                .HasMaxLength(100)
                .HasColumnName("idppno");
            entity.Property(e => e.Lastvisitdte)
                .HasColumnType("date")
                .HasColumnName("lastvisitdte");
            entity.Property(e => e.Nationid).HasColumnName("nationid");
            entity.Property(e => e.Phone1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone1");
            entity.Property(e => e.Phone2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone2");
            entity.Property(e => e.Remark)
                .HasMaxLength(255)
                .HasColumnName("remark");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Saluteid).HasColumnName("saluteid");
            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Vipflg).HasColumnName("vipflg");
            entity.Property(e => e.Visitcount).HasColumnName("visitcount");
        });

        modelBuilder.Entity<MsGuestnationality>(entity =>
        {
            entity.HasKey(e => e.Nationid);

            entity.ToTable("ms_guestnationality");

            entity.Property(e => e.Nationid).HasColumnName("nationid");
            entity.Property(e => e.Nationdesc)
                .HasMaxLength(50)
                .HasColumnName("nationdesc");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsGuestsalutation>(entity =>
        {
            entity.HasKey(e => e.Saluteid);

            entity.ToTable("ms_guestsalutation");

            entity.Property(e => e.Saluteid).HasColumnName("saluteid");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Salutedesc)
                .HasMaxLength(20)
                .HasColumnName("salutedesc");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsGueststate>(entity =>
        {
            entity.HasKey(e => e.Gstateid);

            entity.ToTable("ms_gueststate");

            entity.Property(e => e.Gstateid).HasColumnName("gstateid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Gstatedesc)
                .HasMaxLength(50)
                .HasColumnName("gstatedesc");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
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
            entity.Property(e => e.Occuflg).HasColumnName("occuflg");
            entity.Property(e => e.Paxno).HasColumnName("paxno");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmtypid).HasColumnName("rmtypid");
            entity.Property(e => e.Roomno)
                .HasMaxLength(20)
                .HasColumnName("roomno");
            entity.Property(e => e.Roomstate)
                .HasMaxLength(20)
                .HasColumnName("roomstate");
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
            entity.Property(e => e.Defrateflg).HasColumnName("defrateflg");
            entity.Property(e => e.Execrate)
                .HasColumnType("money")
                .HasColumnName("execrate");
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

        modelBuilder.Entity<MsHotelRoomTypeImage>(entity =>
        {
            entity.HasKey(e => e.Rmtypimgid).HasName("PK_ms_HotelRoomTypeImages_1");

            entity.ToTable("ms_HotelRoomTypeImages");

            entity.Property(e => e.Rmtypimgid).HasColumnName("rmtypimgid");
            entity.Property(e => e.Mainimgflg).HasColumnName("mainimgflg");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmtypid).HasColumnName("rmtypid");
            entity.Property(e => e.Rmtypimgdesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rmtypimgdesc");
            entity.Property(e => e.Rmtypmainimg).HasColumnName("rmtypmainimg");
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
            entity.Property(e => e.Curshift).HasColumnName("curshift");
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
            entity.Property(e => e.Lastrundtetime)
                .HasColumnType("datetime")
                .HasColumnName("lastrundtetime");
            entity.Property(e => e.Latecheckintime).HasColumnName("latecheckintime");
            entity.Property(e => e.Nightauditintervalhr).HasColumnName("nightauditintervalhr");
            entity.Property(e => e.Noofshift).HasColumnName("noofshift");
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

        modelBuilder.Entity<MsHotelservicegrp>(entity =>
        {
            entity.HasKey(e => e.Srvcgrpcde);

            entity.ToTable("ms_hotelservicegrp");

            entity.Property(e => e.Srvcgrpcde)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("srvcgrpcde");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Srvcgrpdesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("srvcgrpdesc");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsHotelservicegrpd>(entity =>
        {
            entity.HasKey(e => e.Srvccde);

            entity.ToTable("ms_hotelservicegrpd");

            entity.Property(e => e.Srvccde)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("srvccde");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Deptcde)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("deptcde");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Srvcaccde)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("srvcaccde");
            entity.Property(e => e.Srvcamt)
                .HasColumnType("money")
                .HasColumnName("srvcamt");
            entity.Property(e => e.Srvcdesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("srvcdesc");
            entity.Property(e => e.Srvcgrpcde)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("srvcgrpcde");
            entity.Property(e => e.Sysdefine).HasColumnName("sysdefine");
            entity.Property(e => e.Trantyp)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("trantyp");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsMessageeditor>(entity =>
        {
            entity.HasKey(e => e.Msgtypid);

            entity.ToTable("ms_messageeditor");

            entity.Property(e => e.Msgtypid).HasColumnName("msgtypid");
            entity.Property(e => e.Checkinid)
                .HasMaxLength(24)
                .HasColumnName("checkinid");
            entity.Property(e => e.Guestid).HasColumnName("guestid");
            entity.Property(e => e.Msgdetail)
                .HasMaxLength(500)
                .HasColumnName("msgdetail");
            entity.Property(e => e.Msgtodept)
                .HasMaxLength(20)
                .HasColumnName("msgtodept");
            entity.Property(e => e.Msgtoperson)
                .HasMaxLength(100)
                .HasColumnName("msgtoperson");
            entity.Property(e => e.Msgtypcde)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("msgtypcde");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("priority");
            entity.Property(e => e.Raisebynme)
                .HasMaxLength(100)
                .HasColumnName("raisebynme");
            entity.Property(e => e.Resolvedetail)
                .HasMaxLength(500)
                .HasColumnName("resolvedetail");
            entity.Property(e => e.Resolvedtetime)
                .HasColumnType("datetime")
                .HasColumnName("resolvedtetime");
            entity.Property(e => e.Resolveflg).HasColumnName("resolveflg");
            entity.Property(e => e.Takedtetime)
                .HasColumnType("datetime")
                .HasColumnName("takedtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<MsReason>(entity =>
        {
            entity.HasKey(e => e.Reasonid);

            entity.ToTable("ms_reason");

            entity.Property(e => e.Reasonid).HasColumnName("reasonid");
            entity.Property(e => e.Guestuseflg).HasColumnName("guestuseflg");
            entity.Property(e => e.Reasondesc)
                .HasMaxLength(50)
                .HasColumnName("reasondesc");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
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
            entity.Property(e => e.Deptcde)
                .HasMaxLength(20)
                .HasColumnName("deptcde");
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

        modelBuilder.Entity<PmsCheckin>(entity =>
        {
            entity.HasKey(e => e.Checkinid);

            entity.ToTable("pms_checkin");

            entity.Property(e => e.Checkinid)
                .HasMaxLength(24)
                .HasColumnName("checkinid");
            entity.Property(e => e.Adultqty).HasColumnName("adultqty");
            entity.Property(e => e.Agencyid).HasColumnName("agencyid");
            entity.Property(e => e.Checkindte)
                .HasColumnType("date")
                .HasColumnName("checkindte");
            entity.Property(e => e.Checkindtetime)
                .HasColumnType("datetime")
                .HasColumnName("checkindtetime");
            entity.Property(e => e.Checkinno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("checkinno");
            entity.Property(e => e.Checkoutdtetime)
                .HasColumnType("datetime")
                .HasColumnName("checkoutdtetime");
            entity.Property(e => e.Checkoutflg).HasColumnName("checkoutflg");
            entity.Property(e => e.Childqty).HasColumnName("childqty");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Contactnme)
                .HasMaxLength(100)
                .HasColumnName("contactnme");
            entity.Property(e => e.Contactno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contactno");
            entity.Property(e => e.Nightqty).HasColumnName("nightqty");
            entity.Property(e => e.Paymenttyp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("paymenttyp");
            entity.Property(e => e.Remark)
                .HasMaxLength(255)
                .HasColumnName("remark");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Specialinstruct)
                .HasMaxLength(255)
                .HasColumnName("specialinstruct");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<PmsCheckinroomguest>(entity =>
        {
            entity.HasKey(e => e.Rmguestid);

            entity.ToTable("pms_checkinroomguest");

            entity.Property(e => e.Rmguestid).HasColumnName("rmguestid");
            entity.Property(e => e.Checkinid)
                .HasMaxLength(24)
                .HasColumnName("checkinid");
            entity.Property(e => e.Guestid).HasColumnName("guestid");
            entity.Property(e => e.Principleflg).HasColumnName("principleflg");
        });

        modelBuilder.Entity<PmsGlobalactionlog>(entity =>
        {
            entity.HasKey(e => e.Actionlogid);

            entity.ToTable("pms_globalactionlog");

            entity.Property(e => e.Actionlogid).HasColumnName("actionlogid");
            entity.Property(e => e.Actiondetail)
                .HasMaxLength(100)
                .HasColumnName("actiondetail");
            entity.Property(e => e.Btnaction)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("btnaction");
            entity.Property(e => e.Formnme)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("formnme");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<PmsGuestbilling>(entity =>
        {
            entity.HasKey(e => e.Billd);

            entity.ToTable("pms_guestbilling");

            entity.Property(e => e.Billd).HasColumnName("billd");
            entity.Property(e => e.Billdesp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("billdesp");
            entity.Property(e => e.Billdiscamt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("billdiscamt");
            entity.Property(e => e.Billno)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("billno");
            entity.Property(e => e.Bizdte)
                .HasColumnType("date")
                .HasColumnName("bizdte");
            entity.Property(e => e.Checkinid)
                .HasMaxLength(24)
                .HasColumnName("checkinid");
            entity.Property(e => e.Chrgacccde)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("chrgacccde");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Currcde)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("currcde");
            entity.Property(e => e.Currrate)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("currrate");
            entity.Property(e => e.Custfullnme)
                .HasMaxLength(100)
                .HasColumnName("custfullnme");
            entity.Property(e => e.Deptcde)
                .HasMaxLength(20)
                .HasColumnName("deptcde");
            entity.Property(e => e.Foliocde)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("foliocde");
            entity.Property(e => e.Itemdesc)
                .HasMaxLength(100)
                .HasColumnName("itemdesc");
            entity.Property(e => e.Itemid)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("itemid");
            entity.Property(e => e.Posid)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("posid");
            entity.Property(e => e.Pricebill)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pricebill");
            entity.Property(e => e.Qty)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("qty");
            entity.Property(e => e.Refno2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("refno2");
            entity.Property(e => e.Remark)
                .HasMaxLength(255)
                .HasColumnName("remark");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Roomno)
                .HasMaxLength(20)
                .HasColumnName("roomno");
            entity.Property(e => e.Shiftno).HasColumnName("shiftno");
            entity.Property(e => e.Srvccde)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("srvccde");
            entity.Property(e => e.Taxamt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("taxamt");
            entity.Property(e => e.Uomcde)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("uomcde");
            entity.Property(e => e.Uomrate)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("uomrate");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Voiddatetime)
                .HasColumnType("datetime")
                .HasColumnName("voiddatetime");
            entity.Property(e => e.Voidflg).HasColumnName("voidflg");
            entity.Property(e => e.Voiduserid).HasColumnName("voiduserid");
        });

        modelBuilder.Entity<PmsReservation>(entity =>
        {
            entity.HasKey(e => e.Resvno);

            entity.ToTable("pms_reservation");

            entity.Property(e => e.Resvno)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("resvno");
            entity.Property(e => e.Adult).HasColumnName("adult");
            entity.Property(e => e.Agencyid).HasColumnName("agencyid");
            entity.Property(e => e.Arrivedte)
                .HasColumnType("date")
                .HasColumnName("arrivedte");
            entity.Property(e => e.Canceldtetime)
                .HasColumnType("datetime")
                .HasColumnName("canceldtetime");
            entity.Property(e => e.Child).HasColumnName("child");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Confirmcancelby)
                .HasMaxLength(100)
                .HasColumnName("confirmcancelby");
            entity.Property(e => e.Confirmdtetime)
                .HasColumnType("datetime")
                .HasColumnName("confirmdtetime");
            entity.Property(e => e.Contactnme)
                .HasMaxLength(100)
                .HasColumnName("contactnme");
            entity.Property(e => e.Contactno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contactno");
            entity.Property(e => e.Guestid).HasColumnName("guestid");
            entity.Property(e => e.Nightqty).HasColumnName("nightqty");
            entity.Property(e => e.Pickupdtetime)
                .HasColumnType("datetime")
                .HasColumnName("pickupdtetime");
            entity.Property(e => e.Pickuploc)
                .HasMaxLength(255)
                .HasColumnName("pickuploc");
            entity.Property(e => e.Reqpickup).HasColumnName("reqpickup");
            entity.Property(e => e.Resvdtetime)
                .HasColumnType("datetime")
                .HasColumnName("resvdtetime");
            entity.Property(e => e.Resvmadeby)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("resvmadeby");
            entity.Property(e => e.Resvstate)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("resvstate");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Specialremark)
                .HasMaxLength(255)
                .HasColumnName("specialremark");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Vipstatus).HasColumnName("vipstatus");
        });

        modelBuilder.Entity<PmsRoomdailycharge>(entity =>
        {
            entity.HasKey(e => e.Dailychrgid);

            entity.ToTable("pms_roomdailycharge");

            entity.Property(e => e.Dailychrgid).HasColumnName("dailychrgid");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Checkinid)
                .HasMaxLength(24)
                .HasColumnName("checkinid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Foliohid).HasColumnName("foliohid");
            entity.Property(e => e.Occudte)
                .HasColumnType("date")
                .HasColumnName("occudte");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Srvccde)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("srvccde");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<PmsRoomfoliod>(entity =>
        {
            entity.HasKey(e => e.Foliodid);

            entity.ToTable("pms_roomfoliod");

            entity.Property(e => e.Foliodid).HasColumnName("foliodid");
            entity.Property(e => e.Foliohid).HasColumnName("foliohid");
            entity.Property(e => e.Srvccde)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("srvccde");
        });

        modelBuilder.Entity<PmsRoomfolioh>(entity =>
        {
            entity.HasKey(e => e.Foliohid);

            entity.ToTable("pms_roomfolioh");

            entity.Property(e => e.Foliohid).HasColumnName("foliohid");
            entity.Property(e => e.Checkinid)
                .HasMaxLength(24)
                .HasColumnName("checkinid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Foliocde)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("foliocde");
            entity.Property(e => e.Foliocloseflg).HasColumnName("foliocloseflg");
            entity.Property(e => e.Foliodesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("foliodesc");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<PmsRoomledger>(entity =>
        {
            entity.HasKey(e => e.Roomlgid);

            entity.ToTable("pms_roomledger");

            entity.Property(e => e.Roomlgid).HasColumnName("roomlgid");
            entity.Property(e => e.Batchno).HasColumnName("batchno");
            entity.Property(e => e.Checkinid)
                .HasMaxLength(24)
                .HasColumnName("checkinid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Discountamt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discountamt");
            entity.Property(e => e.Extrabedprice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("extrabedprice");
            entity.Property(e => e.Extrabedqty).HasColumnName("extrabedqty");
            entity.Property(e => e.Hkeepingflg).HasColumnName("hkeepingflg");
            entity.Property(e => e.Occudte)
                .HasColumnType("date")
                .HasColumnName("occudte");
            entity.Property(e => e.Occuremark)
                .HasMaxLength(255)
                .HasColumnName("occuremark");
            entity.Property(e => e.Occustate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("occustate");
            entity.Property(e => e.Preferroomno)
                .HasMaxLength(20)
                .HasColumnName("preferroomno");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Resvno)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("resvno");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmrateid).HasColumnName("rmrateid");
            entity.Property(e => e.Rmtypid).HasColumnName("rmtypid");
            entity.Property(e => e.Roomno)
                .HasMaxLength(20)
                .HasColumnName("roomno");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<PmsRoomledgerlog>(entity =>
        {
            entity.HasKey(e => e.Roomlogid);

            entity.ToTable("pms_roomledgerlog");

            entity.Property(e => e.Roomlogid).HasColumnName("roomlogid");
            entity.Property(e => e.Batchno).HasColumnName("batchno");
            entity.Property(e => e.Checkinid)
                .HasMaxLength(24)
                .HasColumnName("checkinid");
            entity.Property(e => e.Cmpyid).HasColumnName("cmpyid");
            entity.Property(e => e.Discountamt)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("discountamt");
            entity.Property(e => e.Extrabedprice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("extrabedprice");
            entity.Property(e => e.Extrabedqty).HasColumnName("extrabedqty");
            entity.Property(e => e.Hkeepingflg).HasColumnName("hkeepingflg");
            entity.Property(e => e.Occudte)
                .HasColumnType("date")
                .HasColumnName("occudte");
            entity.Property(e => e.Occuremark)
                .HasMaxLength(255)
                .HasColumnName("occuremark");
            entity.Property(e => e.Occustate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("occustate");
            entity.Property(e => e.Preferroomno)
                .HasMaxLength(20)
                .HasColumnName("preferroomno");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Resvno)
                .HasMaxLength(24)
                .IsUnicode(false)
                .HasColumnName("resvno");
            entity.Property(e => e.Revdtetime)
                .HasColumnType("datetime")
                .HasColumnName("revdtetime");
            entity.Property(e => e.Rmrateid).HasColumnName("rmrateid");
            entity.Property(e => e.Rmtypid).HasColumnName("rmtypid");
            entity.Property(e => e.Roomno)
                .HasMaxLength(20)
                .HasColumnName("roomno");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<Sysconfig>(entity =>
        {
            entity.HasKey(e => e.Keycde);

            entity.ToTable("sysconfig");

            entity.Property(e => e.Keycde)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("keycde");
            entity.Property(e => e.Keyvalue)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("keyvalue");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
