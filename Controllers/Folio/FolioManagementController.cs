using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers.Folio
{
    [Authorize]
    public class FolioManagementController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public FolioManagementController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            ViewData["inHouseRooms"] = GetInHouseRooms();
            ViewData["srvcFolioList"] = GetSrvcFolioList(0);

            return View();
        }

        public IActionResult Edit(int id)
        {
            SetLayOutData();

            ViewData["inHouseRooms"] = GetInHouseRooms();
            ViewData["srvcFolioList"] = GetSrvcFolioList(0);

            var pmsRoomLedger = new PmsRoomledger()
            {
                Roomlgid = id
            };

            return RedirectToAction("~/View/FolioManagement/Index.cshtml", pmsRoomLedger);
        } // not use

        [HttpPost]
        public void SaveFolio(string checkInId, string[][] folioTable, string[][] srvcFolioTable)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Delete PmRoomfolioh data first not to conflict
                var foliohId = _context.PmsRoomfoliohs
                    .Where(fh => fh.Checkinid == checkInId)
                    .Select(fh => fh.Foliohid)
                    .FirstOrDefault();

                _context.PmsRoomfoliohs.Where(fh => fh.Checkinid == checkInId).ExecuteDelete();
                _context.PmsRoomfoliods.Where(fd => fd.Foliohid == foliohId).ExecuteDelete();


                // Insert PmsRoomfolioh
                foreach (var folio in folioTable)
                {
                    var roomFolioH = new PmsRoomfolioh()
                    {
                        Checkinid = checkInId,
                        Foliocde = folio[1],
                        Foliodesc = folio[2],
                        Foliocloseflg = false,
                        Cmpyid = GetCmpyId(),
                        Revdtetime = DateTime.Now,
                        Userid = GetUserId()
                    };
                    _context.PmsRoomfoliohs.Add(roomFolioH);
                }

                _context.SaveChanges();

                // Delete PmRoomfoliod data first not to conflict

                // Insert PmsRoomfoliod
                foreach (var srvcFolio in srvcFolioTable)
                {
                    if (srvcFolio[1] == CommonItems.CommonStrings.DEFAULT_FOLIO_CODE)
                    {
                        continue;
                    }

                    // only save which are not main folio
                    var roomFolioD = new PmsRoomfoliod()
                    {
                        Foliohid = GetFolioHId(checkInId, srvcFolio[2]),
                        Srvccde = srvcFolio[1]
                    };

                    _context.PmsRoomfoliods.Add(roomFolioD);
                }

                _context.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        #endregion


        #region // In-house Room Modal methods //

        public IEnumerable<CheckInModel> GetInHouseRooms()
        {
            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var checkInList = _context.PmsCheckins
                .Join(
                    _context.PmsRoomledgers,
                    chkin => chkin.Checkinid,
                    ledg => ledg.Checkinid,
                    (chkin, ledg) => new { Checkin = chkin, Ledger = ledg }
                )
                .Where(joinResult => joinResult.Checkin.Checkoutflg != true)
                .GroupBy(joinResult => new { joinResult.Ledger.Roomno })
                .Select(group => new CheckInModel
                {
                    CheckInId = group.Max(joinResult => joinResult.Checkin.Checkinid),
                    RoomLgId = group.Max(joinResult => joinResult.Ledger.Roomlgid),
                    Roomno = group.Key.Roomno
                })
                .OrderBy(result => result.Roomno)
                .ToList();

            foreach (var checkIn in checkInList)
            {
                var ledger = _context.PmsRoomledgers.Where(ledg => ledg.Roomlgid == checkIn.RoomLgId && ledg.Cmpyid == cmpyId).FirstOrDefault();

                if (ledger != null)
                {
                    checkIn.Occudte = ledger.Occudte;
                    checkIn.RmtypId = ledger.Rmtypid;
                    checkIn.Rmrateid = ledger.Rmrateid;
                    checkIn.Extrabedqty = ledger.Extrabedqty;
                    checkIn.Extrabedprice = ledger.Extrabedprice;
                    checkIn.Discountamt = ledger.Discountamt;
                }

                var chkIn = _context.PmsCheckins.Where(chkin => chkin.Checkinid == checkIn.CheckInId && chkin.Cmpyid == cmpyId).FirstOrDefault();

                if (chkIn != null)
                {
                    checkIn.Departdte = chkIn.Checkindte.AddDays(chkIn.Nightqty);
                    checkIn.Nightqty = chkIn.Nightqty;
                    checkIn.CheckOutFlag = chkIn.Checkoutflg;
                }

                checkIn.GuestName = GetGuestName(checkIn.CheckInId);
                checkIn.Rmtypdesc = GetRoomTypeCde(checkIn.RmtypId);
                checkIn.Rmprice = GetRoomPrice(checkIn.Rmrateid);
                checkIn.No = checkInList.IndexOf(checkIn) + 1;
            }

            return checkInList;
        }

        public CheckInModel GetInHouseRoomInfo(int roomLgId)
        {
            var cmpyId = GetCmpyId();

            var inHouseRoomInfo = _context.PmsRoomledgers
                .Where(ldg => ldg.Roomlgid == roomLgId && ldg.Cmpyid == cmpyId)
                .Join(_context.PmsCheckins,
                ldg => ldg.Checkinid,
                chkin => chkin.Checkinid,
                (ldg, chkin) => new CheckInModel
                {
                    RoomLgId = ldg.Roomlgid,
                    CheckInId = ldg.Checkinid,
                    StringArriveDte = chkin.Checkindte.ToString("dd MMM yyyy"),
                    Nightqty = chkin.Nightqty,
                    Roomno = ldg.Roomno,
                }
                )
                .FirstOrDefault();

            if (inHouseRoomInfo != null)
            {
                inHouseRoomInfo.GuestName = GetGuestName(inHouseRoomInfo.CheckInId);
            }

            return inHouseRoomInfo ?? new CheckInModel();
        }

        #endregion


        #region // Folio Table and Service Folio table method //

        public IEnumerable<SrvcFolio> GetSrvcFolioList(int roomLgId)
        {
            var srvcFolioList = _context.MsHotelservicegrpds
                .Where(gpd => gpd.Cmpyid == GetCmpyId())
                .Select(gpd => new SrvcFolio
                {
                    SrvcCde = gpd.Srvccde,
                    SrvcDesc = gpd.Srvcdesc ?? "",
                    FolioCde = CommonItems.CommonStrings.DEFAULT_FOLIO_CODE
                })
                .ToList();

            var checkInId = GetCheckInId(roomLgId);

            if (checkInId != null)
            {
                foreach (var srvcFolio in srvcFolioList)
                {
                    var thisSrvcFolio = _context.PmsRoomfoliohs
                        .Where(fh => fh.Checkinid == checkInId)
                        .Join(_context.PmsRoomfoliods,
                        fh => fh.Foliohid,
                        fd => fd.Foliohid,
                        (fh, fd) => new SrvcFolio
                        {
                            SrvcCde = fd.Srvccde,
                            FolioCde = fh.Foliocde
                        })
                        .Where(gp => gp.SrvcCde == srvcFolio.SrvcCde)
                        .FirstOrDefault();

                    if (thisSrvcFolio != null)
                    {
                        srvcFolio.FolioCde = thisSrvcFolio.FolioCde ?? CommonItems.CommonStrings.DEFAULT_FOLIO_CODE;
                    }
                }
            }

            return srvcFolioList;
        }

        public IEnumerable<PmsRoomfolioh> GetFolioList(int roomLgId)
        {
            var checkInId = GetCheckInId(roomLgId);

            var folioList = _context.PmsRoomfoliohs
                .Where(fh => fh.Cmpyid == GetCmpyId() && fh.Checkinid == checkInId)
                .Select(fh => new PmsRoomfolioh
                {
                    Foliocde = fh.Foliocde,
                    Foliodesc = fh.Foliodesc
                })
                .ToList();

            return folioList;
        }

        public IEnumerable<SrvcFolio> GetFolioOptions(int roomLgId)
        {
            var checkInId = GetCheckInId(roomLgId);

            var folioOptionList = _context.PmsRoomfoliohs
                .Where(fh => fh.Cmpyid == GetCmpyId() && fh.Checkinid == checkInId)
                .Select(fh => new SrvcFolio
                {
                    FolioCde = fh.Foliocde
                })
                .ToList();

            return folioOptionList;
        }

        #endregion


        #region // Other Spin-off methods //

        public string GetGuestName(string? checkInId)
        {
            var guestName = _context.PmsCheckinroomguests
                .Where(rg => rg.Checkinid == checkInId && rg.Principleflg == true)
                .Join(_context.MsGuestdata,
                rg => rg.Guestid,
                g => g.Guestid,
                (rg, g) => g.Guestfullnme)
                .FirstOrDefault();

            return guestName ?? "";
        }

        protected string GetRoomTypeCde(int rmtypId)
        {
            var roomTypCde = _context.MsHotelRoomTypes
                .Where(rm => rm.Rmtypid == rmtypId && rm.Cmpyid == GetCmpyId())
                .Select(rm => rm.Rmtypcde)
                .FirstOrDefault();

            return roomTypCde ?? "";
        }

        protected decimal GetRoomPrice(int rmrateId)
        {
            var roomPrice = _context.MsHotelRoomRates
                .Where(rate => rate.Rmrateid == rmrateId && rate.Cmpyid == GetCmpyId())
                .Select(rate => rate.Price)
                .FirstOrDefault();

            return roomPrice;
        }

        public int GetFolioHId(string checkInId, string folioCde)
        {
            var folioHId = _context.PmsRoomfoliohs
                        .Where(folio => folio.Checkinid == checkInId && folio.Foliocde == folioCde && folio.Cmpyid == GetCmpyId())
                        .Select(folio => folio.Foliohid)
                        .FirstOrDefault();

            return folioHId;
        }

        public string? GetCheckInId(int roomLgId)
        {
            var checkInId = _context.PmsRoomledgers
                .Where(led => led.Roomlgid == roomLgId)
                .Select(led => led.Checkinid)
                .FirstOrDefault();

            return checkInId;
        }

        #endregion


        #region // Global methods (Important!) //

        protected short GetUserId()
        {
            var userCde = HttpContext.User.Claims.FirstOrDefault()?.Value;
            var userId = (short)_context.MsUsers
                .Where(u => u.Usercde == userCde)
                .Select(u => u.Userid)
                .FirstOrDefault();

            return userId;
        }

        protected short GetCmpyId()
        {
            var cmpyId = _context.MsUsers
                .Where(u => u.Userid == GetUserId())
                .Select(u => u.Cmpyid)
                .FirstOrDefault();

            return cmpyId;
        }

        protected byte GetShiftNo()
        {
            var shiftNo = _context.MsHotelinfos
                .Where(hotel => hotel.Cmpyid == GetCmpyId())
                .Select(hotel => hotel.Curshift)
                .FirstOrDefault();

            return shiftNo ?? 1;
        }

        protected DateTime GetHotelDate()
        {
            var hotelDate = _context.MsHotelinfos
                .Where(hotel => hotel.Cmpyid == GetCmpyId())
                .Select(hotel => hotel.Hoteldte)
                .FirstOrDefault();

            return hotelDate ?? new DateTime(1990, 1, 1);
        }

        protected int GetMsgCount()
        {
            var count1 = _context.MsMessageeditors.Count(me => me.Takedtetime.Date == GetHotelDate().Date && me.Msgtodept == CommonItems.CommonStrings.DEFAULT_LEVEL);

            var user = _context.MsUsers.FirstOrDefault(u => u.Userid == GetUserId());

            var count2 = _context.MsMessageeditors.Count(me => me.Takedtetime.Date == GetHotelDate().Date && me.Msgtodept == user.Deptcde);

            var count3 = _context.MsMessageeditors.Count(me => me.Takedtetime.Date == GetHotelDate().Date && me.Msgtoperson == user.Usernme);

            var total = count1 + count2 + count3;

            return total;
        }


        protected void SetLayOutData()
        {
            var userId = GetUserId();
            var cmpyId = GetCmpyId();

            var userName = _context.MsUsers
                .Where(u => u.Userid == userId)
                .Select(u => u.Usernme)
                .FirstOrDefault();

            ViewData["Username"] = userName ?? "";

            ViewData["Hotel Date"] = GetHotelDate().ToString("dd MMM yyyy");

            ViewData["Hotel Shift"] = GetShiftNo();

            ViewData["MsgCount"] = GetMsgCount();

            var hotelName = _context.MsHotelinfos
                .Where(cmpy => cmpy.Cmpyid == cmpyId)
                .Select(cmpy => cmpy.Hotelnme)
                .FirstOrDefault();

            ViewData["Hotel Name"] = hotelName ?? "";
        }


        #endregion


    }
}
