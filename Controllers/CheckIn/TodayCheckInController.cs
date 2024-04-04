using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel_Core_MVC_V1.Controllers.CheckIn
{
    [Authorize]
    public class TodayCheckInController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public TodayCheckInController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var checkInList = _context.PmsRoomledgers
                .Where(ldg => ldg.Occudte == hotelDte
                && ldg.Occustate.Equals(CommonItems.CommonStrings.LEDGER_OCCUPIED)
                && ldg.Cmpyid == cmpyId)
                .Select(ldg => new CheckInModel
                {
                    RoomLgId = ldg.Roomlgid,
                    CheckInId = ldg.Checkinid,
                    Occudte = (DateTime)ldg.Occudte,
                    RmtypId = ldg.Rmtypid,
                    Rmrateid = ldg.Rmrateid,
                    Extrabedqty = ldg.Extrabedqty,
                    Extrabedprice = ldg.Extrabedprice,
                    Discountamt = ldg.Discountamt,
                    Roomno = ldg.Roomno
                })
                .ToList();

            foreach (var checkIn in checkInList)
            {
                var pmsCheckIn = GetCheckIn(checkIn.CheckInId);
                if (pmsCheckIn != null)
                {
                    checkIn.Nightqty = pmsCheckIn.Nightqty;
                    checkIn.Departdte = checkIn.Occudte.AddDays(pmsCheckIn.Nightqty);
                }
                checkIn.No = checkInList.IndexOf(checkIn) + 1;
                checkIn.Rmtypdesc = GetRoomTypeCde(checkIn.RmtypId);
                checkIn.PaxNo = GetPaxNo(checkIn.RmtypId);
                checkIn.Rmprice = GetRoomPrice(checkIn.Rmrateid);
                checkIn.Amt = checkIn.Rmprice + (checkIn.Extrabedqty * checkIn.Extrabedprice) - checkIn.Discountamt;
            }

            ViewBag.RoomType = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");
            return View(checkInList);
        }

        #endregion


        #region // Other Spin-off methods //


        protected PmsCheckin? GetCheckIn(string? checkInId)
        {
            if (checkInId != null)
            {
                var pmsCheckIn = _context.PmsCheckins.FirstOrDefault(chkIn => chkIn.Checkinid == checkInId && chkIn.Cmpyid == GetCmpyId());
                return pmsCheckIn;
            }
            else { return null; }

        }

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

        protected int GetPaxNo(int rmtypId)
        {
            var paxNo = _context.MsHotelRoomTypes
                .Where(r => r.Rmtypid == rmtypId && r.Cmpyid == GetCmpyId())
                .Select(r => r.Paxno)
                .FirstOrDefault();

            return paxNo;

        }

        protected decimal GetRoomPrice(int rmrateId)
        {
            var roomPrice = _context.MsHotelRoomRates
                .Where(rate => rate.Rmrateid == rmrateId && rate.Cmpyid == GetCmpyId())
                .Select(rate => rate.Price)
                .FirstOrDefault();

            return roomPrice;
        }

        protected IEnumerable<GuestInfo> GetCheckInGuestList(string? checkInId)
        {
            var checkInGuestList = _context.PmsCheckinroomguests
                .Where(chkIn => chkIn.Checkinid == checkInId)
                .Join(_context.MsGuestdata,
                chkIn => chkIn.Guestid,
                guest => guest.Guestid,
                (chkIn, guest) => new GuestInfo
                {
                    Guestid = guest.Guestid,
                    Saluteid = guest.Saluteid,
                    Guestfullnme = guest.Guestfullnme,
                    Guestlastnme = guest.Guestlastnme,
                    Idppno = guest.Idppno,
                    Idissuedte = guest.Idissuedte,
                    Dob = guest.Dob,
                    Countryid = guest.Countryid,
                    Stateid = guest.Stateid,
                    Nationid = guest.Nationid,
                    Vipflg = guest.Vipflg,
                    Emailaddr = guest.Emailaddr,
                    Phone1 = guest.Phone1,
                    Phone2 = guest.Phone2,
                    Crlimitamt = guest.Crlimitamt,
                    Remark = guest.Remark,
                    Gender = guest.Gender,
                    PrincipleFlg = chkIn.Principleflg
                }
                )
                .ToList();

            foreach (var guest in checkInGuestList)
            {
                guest.No = checkInGuestList.IndexOf(guest) + 1;
            }

            return checkInGuestList ?? new List<GuestInfo>();
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
