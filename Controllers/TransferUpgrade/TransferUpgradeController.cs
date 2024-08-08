using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers.TransferUpgrade
{
    [Authorize]
    public class TransferUpgradeController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public TransferUpgradeController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            ViewData["inHouseRooms"] = GetInHouseRooms();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string mode, PmsRoomledger roomLedger)
        {
            SetLayOutData();

            if (roomLedger.Roomlgid == 0)
            {
                ViewData["inHouseRooms"] = GetInHouseRooms();
                return View(roomLedger);
            }

            var hotelDate = GetHotelDate();
            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var ledgerList = _context.PmsRoomledgers
                    .Where(lgr => lgr.Checkinid == roomLedger.Checkinid && lgr.Occudte.Date >= hotelDate.Date && lgr.Cmpyid == cmpyId)
                    .ToList();

            var oldRoomNo = ledgerList.Select(gp => gp.Roomno).FirstOrDefault();

            foreach (var dbLedger in ledgerList)
            {
                // Insert RoomledgerLog
                var rmLedgerLog = new PmsRoomledgerlog()
                {
                    Resvno = dbLedger.Resvno,
                    Cmpyid = dbLedger.Cmpyid,
                    Checkinid = dbLedger.Checkinid,
                    Occudte = dbLedger.Occudte,
                    Occustate = dbLedger.Occustate,
                    Hkeepingflg = dbLedger.Hkeepingflg,
                    Rmtypid = dbLedger.Rmtypid,
                    Roomno = dbLedger.Roomno,
                    Rmrateid = dbLedger.Rmrateid,
                    Price = dbLedger.Price,
                    Extrabedqty = dbLedger.Extrabedqty,
                    Extrabedprice = dbLedger.Extrabedprice,
                    Discountamt = dbLedger.Discountamt,
                    Preferroomno = dbLedger.Preferroomno,
                    Occuremark = dbLedger.Occuremark,
                    Revdtetime = DateTime.Now,
                    Userid = userId
                };

                _context.PmsRoomledgerlogs.Add(rmLedgerLog);

                // Update RoomLedger
                dbLedger.Roomno = roomLedger.Roomno;
                dbLedger.Rmtypid = roomLedger.Rmtypid;
                dbLedger.Rmrateid = roomLedger.Rmrateid;
                dbLedger.Extrabedqty = roomLedger.Extrabedqty;
                dbLedger.Extrabedprice = roomLedger.Extrabedprice;
                dbLedger.Price = roomLedger.Price;
                dbLedger.Discountamt = roomLedger.Discountamt;
                dbLedger.Revdtetime = DateTime.Now;
                dbLedger.Userid = userId;
                _context.PmsRoomledgers.Update(dbLedger);

            }

            // Update Old Room
            var oldRoom = _context.MsHotelRooms.FirstOrDefault(hr => hr.Roomno == oldRoomNo && hr.Cmpyid == cmpyId);
            if (oldRoom != null)
            {
                oldRoom.Occuflg = false;
                _context.MsHotelRooms.Update(oldRoom);
            }

            // Update New Room 
            var newRoom = _context.MsHotelRooms.FirstOrDefault(hr => hr.Roomno == roomLedger.Roomno && hr.Cmpyid == cmpyId);
            if (newRoom != null)
            {
                newRoom.Occuflg = true;
                _context.MsHotelRooms.Update(newRoom);
            }

            // Insert GlobalActionLog
            var newRoomNo = roomLedger.Roomno;
            var globalAction = new PmsGlobalactionlog()
            {
                Formnme = "TransferUpgrade",
                Btnaction = mode,
                Actiondetail = mode + " from " + oldRoomNo + " to " + newRoomNo, // Manually scripted
                Revdtetime = DateTime.Now,
                Userid = userId
            };
            _context.PmsGlobalactionlogs.Add(globalAction);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            SetLayOutData();

            ViewData["inHouseRooms"] = GetInHouseRooms();
            var roomLedger = new PmsRoomledger()
            {
                Roomlgid = id
            };

            return View("~/Views/TransferUpgrade/Index.cshtml", roomLedger);
        }

        #endregion


        #region // In-house Room Modal & Available Room Modal methods //

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
                    StringDepartDte = chkin.Checkindte.AddDays(chkin.Nightqty).ToString("dd MMM yyyy"),
                    Rmrateid = ldg.Rmrateid,
                    Extrabedqty = ldg.Extrabedqty,
                    Extrabedprice = ldg.Extrabedprice,
                    Discountamt = ldg.Discountamt,
                    Roomno = ldg.Roomno,
                }
                )
                .FirstOrDefault();

            if (inHouseRoomInfo != null)
            {
                inHouseRoomInfo.GuestName = GetGuestName(inHouseRoomInfo.CheckInId);
                inHouseRoomInfo.Rmtypdesc = GetRoomTypeCde(inHouseRoomInfo.RmtypId);
                inHouseRoomInfo.Rmprice = GetRoomPrice(inHouseRoomInfo.Rmrateid);
            }

            return inHouseRoomInfo ?? new CheckInModel();
        }

        public IActionResult GetAvailableRooms(int roomLgId)
        {
            try
            {
                var cmpyId = GetCmpyId();

                var ledger = _context.PmsRoomledgers.Where(ledg => ledg.Roomlgid == roomLgId && ledg.Cmpyid == cmpyId).FirstOrDefault();

                var nightQty = _context.PmsCheckins
                    .Where(chkin => chkin.Checkinid == ledger.Checkinid)
                    .Select(chkin => chkin.Nightqty)
                    .FirstOrDefault();

                var roomList = _context.HotelRoomNumberInfoDBSet
                    .FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @noofday = {2}, @cmpyid = {3},@departureDate = {4}", 1, ledger.Occudte, nightQty, cmpyId, ledger.Occudte)
                    .AsEnumerable()
                    .Select(x => new AvailableRoomNumber
                    {
                        RoomId = x.roomid,
                        RoomNo = x.roomno,
                        RmTypId = x.rmtypid,
                        RoomType = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypid == x.rmtypid).Select(typ => typ.Rmtypcde).FirstOrDefault(),
                        BedCde = _context.MsHotelroombeddings.Where(bed => bed.Bedid == x.bedid).Select(bed => bed.Bedcde).FirstOrDefault(),
                        Location = _context.MsHotellocations.Where(hloc => hloc.Locid == x.locid).Select(hloc => hloc.Locdesc).FirstOrDefault(),
                        RoomPrice = _context.MsHotelRoomRates.Where(rate => rate.Rmtypid == x.rmtypid).Select(rate => rate.Price).FirstOrDefault(),
                        RoomLgId = roomLgId
                    })
                    .ToList();

                foreach (var room in roomList)
                {
                    room.No = roomList.IndexOf(room) + 1;
                }
                return PartialView("_ChooseAvailableRoomsPartialView", roomList);
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }

        public CheckInModel GetAvailableRoomInfo(int roomId)
        {
            var availableRoom = _context.MsHotelRooms
                .Where(rm => rm.Roomid == roomId && rm.Cmpyid == GetCmpyId())
                .Join(_context.MsHotelRoomTypes,
                rm => rm.Rmtypid,
                typ => typ.Rmtypid,
                (rm, typ) => new CheckInModel
                {
                    Roomno = rm.Roomno,
                    RmtypId = typ.Rmtypid,
                    Rmrateid = _context.MsHotelRoomRates.Where(rate => rate.Rmtypid == typ.Rmtypid).Select(rate => rate.Rmrateid).FirstOrDefault(),
                    Rmprice = _context.MsHotelRoomRates.Where(rate => rate.Rmtypid == typ.Rmtypid).Select(rate => rate.Price).FirstOrDefault(),
                    Extrabedprice = (decimal)typ.Extrabedprice
                })
                .FirstOrDefault();

            return availableRoom ?? new CheckInModel();
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
