using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Hotel_Core_MVC_V1.Controllers.Reservation
{
    [Authorize]
    public class ReservationConfirmController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public ReservationConfirmController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        #region  // Main methods //

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
                        State = u.Resvstate.ToString()
                    })
                    .ToListAsync();

            for (int i = 0; i < resvConfirms.Count; i++)
            {
                resvConfirms[i].RoomQty = GetTotalRooms(resvConfirms[i].ResvNo);
                resvConfirms[i].No = i + 1;
            }

            var resvConfirmList = new ReservationConfirmList()
            {
                ResvConfirms = resvConfirms
            };

            return View(resvConfirmList);

        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Search(ReservationConfirmList resvConfirmList)
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();

            var searchResv = resvConfirmList.SearchResv;

            var query = _context.PmsReservations.AsQueryable(); // Start with the base query

            if (!searchResv.GuestName.IsNullOrEmpty())
            {
                query = query.Where(l => l.Contactnme == searchResv.GuestName);
            }

            if (!searchResv.ReserveState.IsNullOrEmpty())
            {
                query = query.Where(l => l.Resvstate.Equals(searchResv.ReserveState[0].ToString()));
            }

            if (searchResv.FromDate != new DateTime(1990, 1, 1)) // Check Default DateTime or Not
            {
                query = query.Where(l => l.Resvdtetime.Date >= searchResv.FromDate.Date);
            }

            if (searchResv.ToDate != new DateTime(1990, 1, 1)) // Check Default DateTime or Not
            {
                query = query.Where(l => l.Resvdtetime.Date <= searchResv.ToDate.Date);
            }

            if (searchResv.FromDate == new DateTime(1990, 1, 1) && searchResv.ToDate == new DateTime(1990, 1, 1))
            {
                query = query.Where(l => l.Resvdtetime.Date == hotelDate.Date);
            }

            var resvConfirms = await query
                .Where(c => c.Cmpyid == cmpyId)
                .Select(c => new ReservationConfirm
                {
                    ResvNo = c.Resvno,
                    ResvDate = c.Resvdtetime,
                    GuestName = c.Contactnme,
                    ArriveDate = c.Arrivedte,
                    ContactNo = c.Contactno,
                    DepartDate = c.Arrivedte.AddDays(c.Nightqty),
                    Agency = c.Agencyid.ToString() ?? "",
                    State = c.Resvstate.ToString()
                })
                .ToListAsync();

            for (int i = 0; i < resvConfirms.Count; i++)
            {
                resvConfirms[i].RoomQty = GetTotalRooms(resvConfirms[i].ResvNo);
                resvConfirms[i].No = i + 1;
            }

            resvConfirmList = new ReservationConfirmList()
            {
                ResvConfirms = resvConfirms
            };

            return View("~/Views/ReservationConfirm/Index.cshtml", resvConfirmList);
        }

        public async Task<IActionResult> ViewResvDetails(string resvNo)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            var cmpyId = GetCmpyId();

            var reservation = await _context.PmsReservations
                .Where(resv => resv.Resvno == resvNo && resv.Cmpyid == cmpyId)
                .FirstOrDefaultAsync();

            var rmLedgers = new List<ReserveRoomDetails>();

            if (reservation != null && reservation.Resvstate != CommonItems.CommonStrings.RESERVATION_CANCEL)
            {
                rmLedgers = await _context.PmsRoomledgers
                .Where(ldg => ldg.Resvno == resvNo && ldg.Occudte.Date == reservation.Arrivedte.Date && ldg.Cmpyid == cmpyId)
                .Select(rm => new ReserveRoomDetails
                {
                    Rmtypid = rm.Rmtypid,
                    Price = rm.Price,
                    Extrabedqty = rm.Extrabedqty,
                    Extrabedprice = rm.Extrabedprice
                })
                .ToListAsync();
            }
            else
            {
                rmLedgers = await _context.PmsRoomledgerlogs
                .Where(ldg => ldg.Resvno == resvNo && ldg.Occudte == reservation.Arrivedte && ldg.Cmpyid == cmpyId)
                .Select(rm => new ReserveRoomDetails
                {
                    Rmtypid = rm.Rmtypid,
                    Price = rm.Price,
                    Extrabedqty = rm.Extrabedqty,
                    Extrabedprice = rm.Extrabedprice
                })
                .ToListAsync();
            }

            foreach (var ledger in rmLedgers)
            {
                ledger.Rmtypdesc = GetRoomTypeDesc(ledger.Rmtypid);
            }

            return PartialView("_ResvDetailsPartialView", rmLedgers);
        }

        public async Task UpdateReservation(string resvNo, string resvState)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return;
            }

            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var resv = await _context.PmsReservations.FirstOrDefaultAsync(resv => resv.Resvno == resvNo && resv.Cmpyid == cmpyId);

            if (resv == null) return;

            resv.Resvstate = resvState[0].ToString();

            if (resv.Resvstate.Equals(CommonItems.CommonStrings.RESERVATION_CONFIRM))
            {
                resv.Confirmdtetime = DateTime.Now;
            }
            else if (resv.Resvstate.Equals(CommonItems.CommonStrings.RESERVATION_CANCEL))
            {
                resv.Canceldtetime = DateTime.Now;
            }

            resv.Confirmcancelby = GetUserName();
            resv.Cmpyid = cmpyId;
            resv.Userid = userId;

            _context.PmsReservations.Update(resv);

            if (resv.Resvstate.Equals(CommonItems.CommonStrings.RESERVATION_CANCEL))
            {
                var rmLedgers = await _context.PmsRoomledgers
                    .Where(led => led.Resvno == resv.Resvno && led.Checkinid == null && led.Cmpyid == cmpyId)
                    .ToListAsync();

                foreach (var rmLedger in rmLedgers)
                {
                    var rmLedgerLog = new PmsRoomledgerlog()
                    {
                        Resvno = rmLedger.Resvno,
                        Cmpyid = rmLedger.Cmpyid,
                        Checkinid = rmLedger.Checkinid,
                        Occudte = rmLedger.Occudte,
                        Occustate = rmLedger.Occustate,
                        Hkeepingflg = rmLedger.Hkeepingflg,
                        Rmtypid = rmLedger.Rmtypid,
                        Roomno = rmLedger.Roomno,
                        Rmrateid = rmLedger.Rmrateid,
                        Price = rmLedger.Price,
                        Extrabedqty = rmLedger.Extrabedqty,
                        Extrabedprice = rmLedger.Extrabedprice,
                        Discountamt = rmLedger.Discountamt,
                        Preferroomno = rmLedger.Preferroomno,
                        Occuremark = rmLedger.Occuremark,
                        Batchno = rmLedger.Batchno,
                        Revdtetime = DateTime.Now,
                        Userid = userId

                    };
                    _context.PmsRoomledgerlogs.Add(rmLedgerLog);
                }
                _context.PmsRoomledgers.RemoveRange(rmLedgers);

            }
            await _context.SaveChangesAsync();
        }

        #endregion


        #region // Other Spin-off methods //

        protected int GetTotalRooms(string resvNo)
        {
            var cmpyId = GetCmpyId();

            var reservation = _context.PmsReservations
                .Where(resv => resv.Resvno == resvNo && resv.Cmpyid == cmpyId)
                .FirstOrDefault();

            int totalRoom = 0;

            if (reservation == null)
            {
                return totalRoom;
            }

            if (reservation.Resvstate == CommonItems.CommonStrings.RESERVATION_CANCEL)
            {
                totalRoom = _context.PmsRoomledgerlogs
                .Count(l => l.Resvno == resvNo && l.Occudte == reservation.Arrivedte && l.Cmpyid == cmpyId);
            }
            else
            {
                totalRoom = _context.PmsRoomledgers
                .Count(l => l.Resvno == resvNo && l.Occudte.Date == reservation.Arrivedte.Date && l.Cmpyid == cmpyId);
            }

            return totalRoom;
        }

        protected string GetRoomTypeDesc(int rmtypId)
        {
            var roomTypCde = _context.MsHotelRoomTypes
                .Where(rm => rm.Rmtypid == rmtypId && rm.Cmpyid == GetCmpyId())
                .Select(rm => rm.Rmtypdesc)
                .FirstOrDefault();

            return roomTypCde ?? "";
        }

        protected string GetUserName()
        {
            return _context.MsUsers
                .Where(u => u.Userid == GetUserId() && u.Cmpyid == GetCmpyId())
                .Select(u => u.Usernme)
                .FirstOrDefault() ?? "";
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
