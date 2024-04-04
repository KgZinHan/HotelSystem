using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers.Visits
{
    [Authorize]
    public class VisitsController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public VisitsController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            var visitList = await (from chkin in _context.PmsCheckins
                                   join crg in _context.PmsCheckinroomguests on chkin.Checkinid equals crg.Checkinid
                                   join gd in _context.MsGuestdata on crg.Guestid equals gd.Guestid
                                   where crg.Principleflg == true && chkin.Checkoutflg == true
                                   orderby chkin.Checkindte.AddDays(chkin.Nightqty) descending
                                   select new VisitModel
                                   {
                                       GuestName = gd.Guestfullnme,
                                       CheckInId = chkin.Checkinid,
                                       ArriveDate = chkin.Checkindte,
                                       DepartDate = chkin.Checkindte.AddDays(chkin.Nightqty),
                                       NightQty = chkin.Nightqty,
                                   }).ToListAsync();

            foreach (var visit in visitList)
            {
                visit.No = visitList.IndexOf(visit) + 1;
                visit.SpendingAmount = GetSpendingAmount(visit.CheckInId);
            }

            return View(visitList);
        }

        [HttpPost]
        public IActionResult View(long id)
        {
            var cmpyId = GetCmpyId();

            var checkInId = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == id && ledg.Cmpyid == cmpyId)
                .Select(ledg => ledg.Checkinid)
                .FirstOrDefault();

            if (checkInId == null)
            {
                return View();
            }

            return View();
        }

        #endregion


        #region // Other spin-off methods //

        private decimal GetSpendingAmount(string checkInId)
        {
            var billTotal = _context.PmsGuestbillings.Where(gb => gb.Checkinid == checkInId).Sum(gb => gb.Pricebill);
            return billTotal;
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
