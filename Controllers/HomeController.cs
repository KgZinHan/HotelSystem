using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;

namespace Hotel_Core_MVC_V1.Controllers
{

    public class HomeController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public HomeController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //
        public IActionResult Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity != null && claimUser.Identity.IsAuthenticated)
            {
                SetLayOutData();

                return View();
            }
            return RedirectToAction("LogIn", "MsUsers"); // Use RedirectToAction instead of RedirectToActionResult
        }

        public async Task<IActionResult> Search(string keyword)
        {
            var hotelDate = GetHotelDate();
            var cmpyId = GetCmpyId();

            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }
            var resultList = await (from chkin in _context.PmsCheckins
                                    join ledg in _context.PmsRoomledgers on chkin.Checkinid equals ledg.Checkinid
                                    where chkin.Checkoutflg == false && chkin.Cmpyid == cmpyId
                                    join chkInRoomGuest in _context.PmsCheckinroomguests
                                    on chkin.Checkinid equals chkInRoomGuest.Checkinid
                                    join guestData in _context.MsGuestdata
                                    on chkInRoomGuest.Guestid equals guestData.Guestid
                                    where guestData.Guestfullnme.Contains(keyword) || (ledg.Roomno != null && ledg.Roomno.Contains(keyword))
                                    group new { chkin, ledg, guestData } by new { chkin.Checkinid, ledg.Resvno, ledg.Roomno, chkin.Checkindte, chkin.Nightqty, guestData.Guestfullnme, guestData.Phone1 } into grouped
                                    orderby grouped.Key.Roomno
                                    select new HomeModel
                                    {
                                        CheckInid = grouped.Key.Checkinid,
                                        RoomLgId = grouped.Max(x => x.ledg.Roomlgid),
                                        ResvNo = grouped.Key.Resvno,
                                        RoomNo = grouped.Key.Roomno,
                                        CheckInDate = grouped.Key.Checkindte,
                                        NightQty = grouped.Key.Nightqty,
                                        GuestName = grouped.Key.Guestfullnme,
                                        GuestNo = grouped.Key.Phone1
                                    })
                             .ToListAsync();

            var resvResultList = await (from l in _context.PmsRoomledgers
                                        join r in _context.PmsReservations on l.Resvno equals r.Resvno
                                        where r.Contactnme.Contains(keyword) && l.Roomno == null
                                        group new { l, r } by new { r.Resvno, l.Batchno, r.Nightqty, r.Arrivedte, r.Contactnme, r.Contactno } into g
                                        select new HomeModel
                                        {
                                            RoomLgId = g.Min(x => x.l.Roomlgid),
                                            ResvNo = g.Key.Resvno,
                                            CheckInDate = g.Key.Arrivedte,
                                            NightQty = g.Key.Nightqty,
                                            GuestName = g.Key.Contactnme,
                                            GuestNo = g.Key.Contactno
                                        })
                                 .ToListAsync();

            var allResultList = resultList.Union(resvResultList).ToList();

            foreach (var result in allResultList)
            {
                var ledger = _context.PmsRoomledgers.FirstOrDefault(ledg => ledg.Roomlgid == result.RoomLgId && ledg.Cmpyid == cmpyId);

                if (ledger != null)
                {
                    result.RoomType = GetRoomTypeCde(ledger.Rmtypid);
                    result.OccuState = ledger.Occustate;
                }

                result.No = allResultList.IndexOf(result) + 1;
                result.DepartDate = result.CheckInDate.AddDays(result.NightQty);
                result.FolioBalance = GetBalance(result.CheckInid);
            }

            if (!allResultList.Any())
            {
                return PartialView("_BlankView");
            }

            return PartialView("_HomeSearchResultView", allResultList);
        }

        public IActionResult OpenActions(long roomlgId)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            var model = _context.PmsRoomledgers
                .Where(leg => leg.Roomlgid == roomlgId && leg.Cmpyid == GetCmpyId())
                .Select(leg => new HomeModel
                {
                    RoomLgId = leg.Roomlgid,
                    OccuState = leg.Occustate
                })
                .FirstOrDefault();

            return PartialView("_HomeActionsPartialView", model);
        }

        public IActionResult Privacy()
        {
            SetLayOutData();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion


        #region // Occupancies //

        public IActionResult Occupancies()
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();

            var hotelStatusList = new List<Occupancies>();

            var hotelStatus = new Occupancies()
            {
                Description = "TOTAL ROOM",
                Status = _context.MsHotelRooms.Count(hr => hr.Cmpyid == cmpyId)
            };

            hotelStatusList.Add(hotelStatus);

            hotelStatus = new Occupancies()
            {
                Description = "IN-HOUSE GUEST",
                Status = _context.MsHotelRooms.Count(hr => hr.Cmpyid == cmpyId && hr.Occuflg == true)
            };

            hotelStatusList.Add(hotelStatus);

            hotelStatus = new Occupancies()
            {
                Description = "CHECK-IN TODAY",
                Status = _context.PmsCheckins.Count(chkin => chkin.Cmpyid == cmpyId && chkin.Checkindte.Date == hotelDate.Date)
            };

            hotelStatusList.Add(hotelStatus);

            hotelStatus = new Occupancies()
            {
                Description = "CHECK-OUT TODAY",
                Status = _context.PmsCheckins.Count(chkin => chkin.Cmpyid == cmpyId && chkin.Checkoutdtetime == hotelDate) // Check back
            };

            hotelStatusList.Add(hotelStatus);

            hotelStatus = new Occupancies()
            {
                Description = "EXPECTED ARRIVAL",
                Status = _context.PmsReservations.Count(resv => resv.Cmpyid == cmpyId && resv.Arrivedte.Date == hotelDate.Date && resv.Resvstate != "N")
            };

            hotelStatusList.Add(hotelStatus);

            hotelStatus = new Occupancies()
            {
                Description = "EXPECTED DEPARTURE",
                Status = _context.PmsCheckins.Count(chkin => chkin.Cmpyid == cmpyId && chkin.Checkindte.AddDays(chkin.Nightqty).Date == hotelDate.Date)
            };

            hotelStatusList.Add(hotelStatus);

            hotelStatus = new Occupancies()
            {
                Description = "OUT OF ORDER/INVENTORY",
                Status = _context.PmsRoomledgers.Count(ledg => ledg.Cmpyid == cmpyId && ledg.Occudte.Date == hotelDate.Date && ledg.Occustate == CommonItems.CommonStrings.LEDGER_MAINTENANCE)
            };

            hotelStatusList.Add(hotelStatus);

            hotelStatus = new Occupancies()
            {
                Description = "STAY-OVER GUEST",
                Status = _context.PmsRoomledgers.Count(ledg => ledg.Cmpyid == cmpyId && ledg.Occudte.Date == hotelDate.Date && ledg.Occuremark == CommonItems.CommonStrings.EXTEND_STAY)
            };

            hotelStatusList.Add(hotelStatus);

            var totalInHouseRoom = _context.MsHotelRooms.Count(hr => hr.Occuflg == true && hr.Cmpyid == cmpyId);

            var totalRoom = _context.MsHotelRooms.Count(hr => hr.Cmpyid == cmpyId);

            var maintenanceRoom = _context.PmsRoomledgers.Count(ledg => ledg.Cmpyid == cmpyId && ledg.Occudte.Date == hotelDate.Date && ledg.Occustate == CommonItems.CommonStrings.LEDGER_MAINTENANCE);

            float totalOccupancy = (float)totalInHouseRoom * 100 / totalRoom;
            totalOccupancy = (float)Math.Round(totalOccupancy, 2);

            var totalVacancy = totalRoom - totalInHouseRoom - maintenanceRoom;

            hotelStatus = new Occupancies()
            {
                Description = "TOTAL VACANCY",
                Status = totalVacancy
            };

            hotelStatusList.Add(hotelStatus);

            ViewData["OccuPercent"] = totalOccupancy + "%";

            return View(hotelStatusList);
        }

        #endregion


        #region // Shift End //

        public IActionResult ShiftEnd()
        {
            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var hotel = _context.MsHotelinfos.FirstOrDefault(hInfo => hInfo.Cmpyid == cmpyId);

            if (hotel != null)
            {
                if (hotel.Curshift < hotel.Noofshift)
                {
                    hotel.Curshift++;
                }
                else // restart
                {
                    hotel.Curshift = 1;
                }

                hotel.Revdtetime = DateTime.Now;
                hotel.Userid = userId;

                _context.MsHotelinfos.Update(hotel);
                _context.SaveChanges();
            }

            SetLayOutData();

            return View("~/Views/Home/Index.cshtml");
        }

        #endregion


        #region // Cancel Reservation methods //

        public IActionResult OpenCancelResv(long roomLgId)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            var cancelResv = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == roomLgId && ledg.Cmpyid == GetCmpyId())
                .Join(_context.PmsReservations,
                ledg => ledg.Resvno,
                resv => resv.Resvno,
                (ledg, resv) => new HomeModel
                {
                    ResvNo = resv.Resvno,
                    GuestName = resv.Contactnme,
                    GuestNo = resv.Contactno,
                    RoomType = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypid == ledg.Rmtypid).Select(typ => typ.Rmtypcde).FirstOrDefault(),
                    CheckInDate = resv.Arrivedte,
                    DepartDate = resv.Arrivedte.AddDays(resv.Nightqty)
                })
                .FirstOrDefault();

            return PartialView("_HomeCancelResvPartialView", cancelResv);
        }

        [HttpPost]
        public void CancelResv(string resvNo)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                StatusCode(401);
                return;
            }

            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var resv = _context.PmsReservations.FirstOrDefault(resv => resv.Resvno == resvNo && resv.Cmpyid == cmpyId);

            if (resv == null) return;

            resv.Resvstate = CommonItems.CommonStrings.RESERVATION_CANCEL;
            resv.Canceldtetime = DateTime.Now;
            resv.Confirmcancelby = GetUserName();
            resv.Cmpyid = cmpyId;
            resv.Userid = userId;

            _context.PmsReservations.Update(resv);

            var ledgers = _context.PmsRoomledgers
                .Where(led => led.Resvno == resv.Resvno && led.Checkinid == null && led.Cmpyid == cmpyId)
                .ToList();

            foreach (var rmLedger in ledgers)
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

            _context.PmsRoomledgers.RemoveRange(ledgers);


            _context.SaveChanges();
        }

        #endregion


        #region // Check-out methods //

        public IActionResult OpenCheckOut(long roomLgId)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            var checkOut = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == roomLgId && ledg.Cmpyid == GetCmpyId())
                .Join(_context.PmsCheckins,
                ledg => ledg.Checkinid,
                chkIn => chkIn.Checkinid,
                (ledg, chkIn) => new HomeModel
                {
                    RoomLgId = roomLgId,
                    CheckInid = ledg.Checkinid,
                    RoomType = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypid == ledg.Rmtypid).Select(typ => typ.Rmtypcde).FirstOrDefault(),
                    CheckInDate = chkIn.Checkindte,
                    DepartDate = chkIn.Checkindte.AddDays(chkIn.Nightqty)
                })
                .FirstOrDefault();

            if (checkOut != null)
            {
                checkOut.GuestName = GetGuestName(checkOut.CheckInid);
                checkOut.FolioBalance = GetBalance(checkOut.CheckInid);
            }

            return PartialView("_HomeCheckOutPartialView", checkOut);
        }

        public IActionResult CheckOut(string id)
        {
            var cmpyId = GetCmpyId();
            var userId = GetUserId();
            var hotelDate = GetHotelDate();

            try
            {
                // Update PmsRoomfoliohs
                var folioList = _context.PmsRoomfoliohs.Where(rfh => rfh.Checkinid == id && rfh.Cmpyid == cmpyId).ToList();
                if (folioList != null)
                {
                    foreach (var folio in folioList)
                    {
                        if (folio.Foliocloseflg != true)
                        {
                            folio.Foliocloseflg = true;
                            _context.PmsRoomfoliohs.Update(folio);
                        }
                    }
                }

                // Update PmsCheckins
                var checkIn = _context.PmsCheckins.FirstOrDefault(chkIn => chkIn.Checkinid == id && chkIn.Cmpyid == cmpyId);
                if (checkIn != null)
                {
                    checkIn.Checkoutflg = true;
                    checkIn.Checkoutdtetime = DateTime.Now;
                    checkIn.Userid = userId;
                    _context.PmsCheckins.Update(checkIn);
                }

                // Update MsGuest
                var guestList = _context.PmsCheckinroomguests.Where(crg => crg.Checkinid == id).ToList();
                if (guestList != null)
                {
                    foreach (var guest in guestList)
                    {
                        var guestdata = _context.MsGuestdata.FirstOrDefault(g => g.Guestid == guest.Guestid && g.Cmpyid == cmpyId);
                        if (guestdata != null)
                        {
                            guestdata.Lastvisitdte = DateTime.Now;
                            guestdata.Visitcount += 1;
                            guestdata.Revdtetime = DateTime.Now;
                            guestdata.Userid = userId;
                            _context.MsGuestdata.Update(guestdata);
                        }
                    }
                }

                // Update MsHotelRoom
                var ledger = _context.PmsRoomledgers.FirstOrDefault(ledg => ledg.Checkinid == id && ledg.Cmpyid == cmpyId);
                if (ledger != null)
                {
                    var hotelRoom = _context.MsHotelRooms.FirstOrDefault(hr => hr.Roomno == ledger.Roomno && hr.Cmpyid == cmpyId);
                    if (hotelRoom != null)
                    {
                        hotelRoom.Occuflg = false;
                        _context.MsHotelRooms.Update(hotelRoom);
                    }
                }

                // Delete pmsRoomledger and Insert pmsRoomledgerLog
                var extraLedgers = _context.PmsRoomledgers.Where(ledg => ledg.Checkinid == id && ledg.Occudte > hotelDate && ledg.Cmpyid == cmpyId).ToList();

                if (!extraLedgers.IsNullOrEmpty())
                {
                    foreach (var rmLedger in extraLedgers)
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
                    _context.PmsRoomledgers.RemoveRange(extraLedgers);
                }

                _context.SaveChanges();

                return RedirectToAction("index");
            }
            catch
            {
                return RedirectToAction("index");
            }
        }

        #endregion


        #region // Other spin-off methods //

        protected string GetRoomTypeCde(int rmtypId)
        {
            var roomTypCde = _context.MsHotelRoomTypes
                .Where(rm => rm.Rmtypid == rmtypId && rm.Cmpyid == GetCmpyId())
                .Select(rm => rm.Rmtypcde)
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

        public decimal GetBalance(string? checkInId)
        {
            var billList = _context.PmsGuestbillings
                .Where(gb => gb.Checkinid == checkInId && gb.Voidflg != true && gb.Cmpyid == GetCmpyId())
                .ToList();

            decimal? balance = 0;

            foreach (var bill in billList)
            {
                balance += bill.Qty * bill.Pricebill;
            }

            return balance ?? 0;
        }

        public string GetGuestName(string? checkInId)
        {
            var guestName = _context.PmsCheckinroomguests
                .Where(crg => crg.Checkinid == checkInId && crg.Principleflg == true)
                .Join(_context.MsGuestdata,
                crg => crg.Guestid,
                gd => gd.Guestid,
                (crg, gd) => gd.Guestfullnme
                )
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