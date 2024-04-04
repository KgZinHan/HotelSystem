using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers.CheckIn
{
    [Authorize]
    public class ExtendStayController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public ExtendStayController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

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
                var chkIn = _context.PmsCheckins.Where(chkin => chkin.Checkinid == checkIn.CheckInId && chkin.Cmpyid == cmpyId).FirstOrDefault();

                if (chkIn != null)
                {
                    checkIn.Occudte = chkIn.Checkindte;
                    checkIn.Departdte = chkIn.Checkindte.AddDays(chkIn.Nightqty);
                    checkIn.CheckOutFlag = chkIn.Checkoutflg;
                }

                checkIn.No = checkInList.IndexOf(checkIn) + 1;
                checkIn.GuestName = GetGuestName(checkIn.CheckInId);
            }

            return View(checkInList);
        }

        public bool CheckAvailability(int roomLgId, short dayCount)
        {
            try
            {
                var cmpyId = GetCmpyId();

                var ledger = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == roomLgId && ledg.Cmpyid == cmpyId)
                .FirstOrDefault();

                if (ledger == null)
                {
                    return false;
                }

                var checkIn = _context.PmsCheckins
                    .Where(chkIn => chkIn.Checkinid == ledger.Checkinid && chkIn.Cmpyid == cmpyId)
                    .FirstOrDefault();

                if (checkIn == null)
                {
                    return false;
                }

                var departureDate = checkIn.Checkindte.AddDays(checkIn.Nightqty);

                var room = _context.HotelRoomNumberInfoDBSet
                    .FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @noofday = {2}, @cmpyid = {3},@departureDate = {4}", 4, departureDate, dayCount, cmpyId, departureDate)
                    .AsEnumerable()
                    .Select(x => new AvailableRoomNumber
                    {
                        RoomNo = x.roomno,
                        RmTypId = x.rmtypid,
                        RoomType = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypid == x.rmtypid).Select(typ => typ.Rmtypcde).FirstOrDefault(),
                        BedCde = _context.MsHotelroombeddings.Where(bed => bed.Bedid == x.bedid).Select(bed => bed.Bedcde).FirstOrDefault(),
                        RoomLgId = roomLgId
                    })
                    .Where(x => x.RoomNo == ledger.Roomno)
                    .FirstOrDefault();

                if (room != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void ExtendStay(int roomLgId, short dayCount)
        {
            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var ledger = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == roomLgId && ledg.Cmpyid == cmpyId)
                .FirstOrDefault();

            if (ledger == null)
            {
                return;
            }

            var checkIn = _context.PmsCheckins
                .Where(chkIn => chkIn.Checkinid == ledger.Checkinid && chkIn.Cmpyid == cmpyId)
                .FirstOrDefault();

            if (checkIn == null)
            {
                return;
            }

            var newOccuDte = checkIn.Checkindte.AddDays(checkIn.Nightqty);

            for (int i = 0; i < dayCount; i++)
            {
                // Insert PmsRoomledgers
                var newLedger = new PmsRoomledger()
                {
                    Checkinid = ledger.Checkinid,
                    Resvno = ledger.Resvno,
                    Cmpyid = cmpyId,
                    Occudte = newOccuDte.AddDays(i),
                    Occustate = ledger.Occustate,
                    Hkeepingflg = ledger.Hkeepingflg,
                    Rmtypid = ledger.Rmtypid,
                    Roomno = ledger.Roomno,
                    Rmrateid = ledger.Rmrateid,
                    Price = ledger.Price,
                    Extrabedqty = ledger.Extrabedqty,
                    Extrabedprice = ledger.Extrabedprice,
                    Discountamt = ledger.Discountamt,
                    Preferroomno = ledger.Preferroomno,
                    Occuremark = CommonItems.CommonStrings.EXTEND_STAY,
                    Batchno = ledger.Batchno,
                    Revdtetime = DateTime.Now,
                    Userid = userId
                };

                _context.PmsRoomledgers.Add(newLedger);
            }

            // Update PmsCheckins
            checkIn.Nightqty += dayCount;
            checkIn.Revdtetime = DateTime.Now;
            checkIn.Userid = userId;

            _context.PmsCheckins.Update(checkIn);

            // Insert PmsGlobalActionLog
            var log = new PmsGlobalactionlog()
            {
                Formnme = "ExtendStay",
                Btnaction = "Check and Extend",
                Actiondetail = "Extend " + dayCount + " day for check-in ID " + checkIn.Checkinid,
                Userid = userId,
                Revdtetime = DateTime.Now
            };

            _context.PmsGlobalactionlogs.Add(log);

            _context.SaveChanges();

        }

        #endregion


        #region // Utility methods for parsing //

        static Boolean ParseBool(string value)
        {
            if (Boolean.TryParse(value, out Boolean result))
            {
                return result;
            }
            return default;
        }

        static DateTime ParseDateTime(string value)
        {
            if (DateTime.TryParse(value, out DateTime result))
            {
                return result;
            }
            return default;
        }

        static byte ParseByte(string value)
        {
            if (byte.TryParse(value, out byte result))
            {
                return result;
            }
            return default;
        }

        static short ParseInt16(string value)
        {
            if (short.TryParse(value, out short result))
            {
                return result;
            }
            return default;
        }

        static int ParseInt32(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return default;
        }

        static decimal ParseDecimal(string value)
        {
            if (decimal.TryParse(value, out decimal result))
            {
                return result;
            }
            return default;
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
