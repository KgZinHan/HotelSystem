using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers.Reservation
{
    [Authorize]
    public class ReservationActivityController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public ReservationActivityController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();

            var resvConfirms = await _context.PmsReservations
                    .Where(l => l.Cmpyid == cmpyId && l.Resvdtetime.Date == hotelDate.Date)
                    .Select(u => new ReservationConfirm
                    {
                        ResvNo = u.Resvno,
                        ResvDate = u.Resvdtetime,
                        GuestName = u.Contactnme,
                        ArriveDate = u.Arrivedte,
                        ContactNo = u.Contactno,
                        DepartDate = u.Arrivedte.AddDays(u.Nightqty),
                        Agency = u.Agencyid.ToString() ?? "",
                        State = u.Resvstate.ToString(),
                        Remark = u.Specialremark ?? ""
                    })
                    .ToListAsync();

            for (int i = 0; i < resvConfirms.Count; i++)
            {
                resvConfirms[i].RoomQty = GetTotalRooms(resvConfirms[i].ResvNo);
                resvConfirms[i].State = GetResvState(resvConfirms[i].State);
                resvConfirms[i].No = i + 1;
            }

            var resvAct = new ReservationActivity()
            {
                ReserveCount = GetCount("R"),
                ConfirmCount = GetCount("C"),
                CancelCount = GetCount("N")
            };

            var resvConfirmList = new ReservationConfirmList()
            {
                ResvConfirms = resvConfirms,
                ResvAct = resvAct
            };
            return View(resvConfirmList);
        }

        public async Task<IActionResult> Search(ReservationConfirmList resvConfirmList)
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();

            var searchResv = resvConfirmList.SearchResv;

            var resvConfirms = await _context.PmsReservations
                .Where(c => c.Cmpyid == cmpyId && c.Resvstate.Equals(searchResv.ReserveState[0].ToString()) && c.Resvdtetime.Date == hotelDate.Date)
                .Select(c => new ReservationConfirm
                {
                    ResvNo = c.Resvno,
                    ResvDate = c.Resvdtetime,
                    GuestName = c.Contactnme,
                    ArriveDate = c.Arrivedte,
                    ContactNo = c.Contactno,
                    DepartDate = c.Arrivedte.AddDays(c.Nightqty),
                    Agency = c.Agencyid.ToString() ?? "",
                    State = c.Resvstate.ToString(),
                    Remark = "testing"
                })
                .ToListAsync();

            for (int i = 0; i < resvConfirms.Count; i++)
            {
                resvConfirms[i].RoomQty = GetTotalRooms(resvConfirms[i].ResvNo);
                resvConfirms[i].State = GetResvState(resvConfirms[i].State);
                resvConfirms[i].No = i + 1;
            }

            var resvAct = new ReservationActivity()
            {
                ReserveCount = GetCount("R"),
                ConfirmCount = GetCount("C"),
                CancelCount = GetCount("N")
            };

            resvConfirmList = new ReservationConfirmList()
            {
                ResvConfirms = resvConfirms,
                ResvAct = resvAct
            };

            return View("~/Views/ReservationActivity/Index.cshtml", resvConfirmList);
        }

        #endregion


        #region // Other Spin-off methods //

        protected int GetTotalRooms(string resvNo)
        {
            var cmpyId = GetCmpyId();

            var reservation = _context.PmsReservations
                .Where(resv => resv.Resvno == resvNo && resv.Cmpyid == cmpyId)
                .FirstOrDefault();

            var totalRooms = _context.PmsRoomledgers.Count(l => l.Resvno == resvNo && l.Occudte.Date == reservation.Arrivedte.Date && l.Cmpyid == cmpyId);

            if (totalRooms == 0) // Check in room ledger logs
            {
                totalRooms = _context.PmsRoomledgerlogs.Count(l => l.Resvno == resvNo && l.Occudte == reservation.Arrivedte && l.Cmpyid == cmpyId);
            }

            return totalRooms;
        }

        protected int GetCount(string alphabet)
        {
            var count = _context.PmsReservations
                .Count(resv => resv.Cmpyid == GetCmpyId() && resv.Resvstate.Equals(alphabet) && resv.Resvdtetime.Date == GetHotelDate().Date);

            return count;
        }

        protected string GetResvState(string state)
        {
            return state switch
            {
                "R" => "reserved",
                "C" => "confirmed",
                "N" => "canceled",
                _ => "",
            };
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
