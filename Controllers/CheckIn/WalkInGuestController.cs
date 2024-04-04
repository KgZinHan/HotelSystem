using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hotel_Core_MVC_V1.Controllers.CheckIn
{
    [Authorize]
    public class WalkInGuestController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public WalkInGuestController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var checkInModel = new CheckInModel()
            {
                Occudte = GetHotelDate(),
                Departdte = GetHotelDate().AddDays(1)
            };

            return View(checkInModel);
        }

        public IActionResult Edit(string roomNo)
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();

            var checkInModel = _context.MsHotelRooms
                .Where(rm => rm.Roomno == roomNo && rm.Cmpyid == cmpyId)
                .Select(rm => new CheckInModel
                {
                    Occudte = hotelDate,
                    Departdte = hotelDate.AddDays(1),
                    Roomno = rm.Roomno,
                    Rmtypdesc = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypid == rm.Rmtypid && typ.Cmpyid == cmpyId).Select(typ => typ.Rmtypcde).FirstOrDefault() ?? "",
                    Rmprice = _context.MsHotelRoomRates.Where(rate => rate.Rmtypid == rm.Rmtypid && rate.Cmpyid == cmpyId && rate.Defrateflg == true).Select(rate => rate.Price).FirstOrDefault(),
                    Extrabedprice = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypid == rm.Rmtypid && typ.Cmpyid == cmpyId).Select(typ => typ.Extrabedprice).FirstOrDefault() ?? 0,
                    Amt = _context.MsHotelRoomRates.Where(rate => rate.Rmtypid == rm.Rmtypid && rate.Cmpyid == cmpyId && rate.Defrateflg == true).Select(rate => rate.Price).FirstOrDefault(),
                    RmtypId = rm.Rmtypid ?? 0,
                    Nightqty = 1
                })
                .FirstOrDefault();

            ViewData["Rates"] = new SelectList(_context.MsHotelRoomRates.Where(rate => rate.Cmpyid == GetCmpyId() && rate.Rmtypid == checkInModel.RmtypId), "Rmrateid", "Rmratedesc");

            return View("~/Views/WalkInGuest/Index.cshtml", checkInModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(CheckInModel checkIn)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SetLayOutData();

                    ViewData["Rates"] = new SelectList(_context.MsHotelRoomRates.Where(rate => rate.Cmpyid == GetCmpyId() && rate.Rmtypid == checkIn.RmtypId), "Rmrateid", "Rmratedesc");
                    ModelState.AddModelError(string.Empty, CommonItems.CommonStrings.COMMON_ERROR_MESSAGE);

                    return View("~/Views/WalkInGuest/Index.cshtml", checkIn);
                }

                var generatedChkInId = GenerateRefNo("CHK");
                var cmpyId = GetCmpyId();
                var userId = GetUserId();
                var hotelDate = GetHotelDate();

                // Add Ledgers 
                for (int i = 0; i < checkIn.Nightqty; i++)
                {
                    var roomLedger = new PmsRoomledger()
                    {
                        Checkinid = generatedChkInId,
                        Occudte = checkIn.Occudte.AddDays(i),
                        Occustate = CommonItems.CommonStrings.LEDGER_OCCUPIED,
                        Hkeepingflg = false,
                        Rmtypid = checkIn.RmtypId,
                        Roomno = checkIn.Roomno,
                        Rmrateid = checkIn.Rmrateid,
                        Price = checkIn.Rmprice,
                        Extrabedqty = checkIn.Extrabedqty,
                        Extrabedprice = checkIn.Extrabedprice,
                        Discountamt = checkIn.Discountamt,
                        Batchno = 1, // Default
                        Cmpyid = cmpyId,
                        Revdtetime = DateTime.Now,
                        Userid = userId
                    };

                    _context.PmsRoomledgers.Update(roomLedger);
                }

                // Update Hotel Room
                var hotelRoom = _context.MsHotelRooms.FirstOrDefault(hr => hr.Roomno == checkIn.Roomno && hr.Cmpyid == cmpyId);
                if (hotelRoom != null)
                {
                    hotelRoom.Occuflg = true;
                    _context.MsHotelRooms.Update(hotelRoom);
                }


                // Insert CheckIn 
                var pmsCheckIn = new PmsCheckin()
                {
                    Checkinid = generatedChkInId,
                    Cmpyid = cmpyId,
                    Checkindte = checkIn.Occudte,
                    Nightqty = checkIn.Nightqty,
                    Contactnme = checkIn.ContactName ?? "",
                    Contactno = checkIn.ContactNo ?? "",
                    Specialinstruct = checkIn.SpecialInstruct,
                    Adultqty = checkIn.Adultqty,
                    Childqty = checkIn.Childqty,
                    Remark = checkIn.Remark,
                    Checkindtetime = DateTime.Now,
                    Revdtetime = DateTime.Now,
                    Userid = userId
                };
                _context.PmsCheckins.Add(pmsCheckIn);

                // Increse checkInLastno
                var checkInNo = _context.MsAutonumbers.FirstOrDefault(auto => auto.Posid == "CHK" && auto.Cmpyid == cmpyId);
                if (checkInNo != null)
                {
                    checkInNo.Lastusedno += 1;
                    _context.MsAutonumbers.Update(checkInNo);
                }

                _context.SaveChanges();

                var roomLgId = _context.PmsRoomledgers
                    .Where(ledg => ledg.Checkinid == generatedChkInId && ledg.Occudte.Date == checkIn.Occudte.Date && ledg.Cmpyid == cmpyId)
                    .Select(ledg => ledg.Roomlgid)
                    .FirstOrDefault();

                return RedirectToAction("Edit", "GuestBilling", new { id = roomLgId });

            }
            catch
            {
                SetLayOutData();

                ViewData["Rates"] = new SelectList(_context.MsHotelRoomRates.Where(rate => rate.Cmpyid == GetCmpyId() && rate.Rmtypid == checkIn.RmtypId), "Rmrateid", "Rmratedesc");
                ModelState.AddModelError(string.Empty, CommonItems.CommonStrings.COMMON_ERROR_MESSAGE);

                return View("~/Views/WalkInGuest/Index.cshtml", checkIn);
            }

        }


        #endregion


        #region // Guests //

        public void SaveGuests(string[][] guests)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var generatedChkInId = GenerateRefNo("CHK");

                // Delete previous Pmscheckinroomguest Data First
                var oldCheckInGuest = _context.PmsCheckinroomguests
                    .Where(rg => rg.Checkinid == generatedChkInId)
                    .ExecuteDelete();


                foreach (var guest in guests)
                {
                    if (guest == null) continue;

                    var guestInfo = new MsGuestdatum()
                    {
                        Saluteid = ParseInt16(guest[1]),
                        Guestfullnme = guest[2],
                        Guestlastnme = guest[3],
                        Idppno = guest[4],
                        Chrgacccde = guest[21],
                        Idissuedte = guest[5] != "" ? ParseDateTime(guest[5]) : null,
                        Dob = guest[6] != "" ? ParseDateTime(guest[6]) : null,
                        Countryid = ParseInt32(guest[7]),
                        Stateid = ParseInt32(guest[8]),
                        Nationid = ParseInt32(guest[9]),
                        Vipflg = ParseBool(guest[10]),
                        Emailaddr = guest[11],
                        Phone1 = guest[12],
                        Phone2 = guest[13],
                        Crlimitamt = ParseDecimal(guest[14]),
                        Remark = guest[15],
                        Gender = ParseByte(guest[16]),
                        Cmpyid = GetCmpyId(),
                        Userid = GetUserId(),
                        Revdtetime = DateTime.Now
                    };

                    if (!string.IsNullOrEmpty(guest[17])) // Update
                    {
                        guestInfo.Guestid = ParseInt32(guest[17]);
                        guestInfo.Lastvisitdte = ParseDateTime(guest[19]);
                        guestInfo.Visitcount = ParseInt16(guest[20]);
                        _context.MsGuestdata.Update(guestInfo);
                        _context.SaveChanges();
                    }
                    else // Insert
                    {
                        guestInfo.Lastvisitdte = null;
                        guestInfo.Visitcount = 0;
                        _context.MsGuestdata.Add(guestInfo);
                        _context.SaveChanges();

                    }

                    // Insert CheckInRoomGuest
                    var roomGuest = new PmsCheckinroomguest()
                    {
                        Checkinid = generatedChkInId,
                        Guestid = guestInfo.Guestid,
                        Principleflg = ParseBool(guest[18])
                    };
                    _context.PmsCheckinroomguests.Add(roomGuest);
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


        #region // Available Rooms //

        public IActionResult GetAvailableRooms(DateTime arriveDte, int nightQty)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            try
            {
                var cmpyId = GetCmpyId();

                var roomList = _context.HotelRoomNumberInfoDBSet
                    .FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @noofday = {2}, @cmpyid = {3},@departureDate = {4}", 1, arriveDte, nightQty, cmpyId, arriveDte)
                    .AsEnumerable()
                    .Select(x => new AvailableRoomNumber
                    {
                        RoomId = x.roomid,
                        RoomNo = x.roomno,
                        RmTypId = x.rmtypid,
                        RoomType = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypid == x.rmtypid).Select(typ => typ.Rmtypcde).FirstOrDefault(),
                        BedCde = _context.MsHotelroombeddings.Where(bed => bed.Bedid == x.bedid).Select(bed => bed.Bedcde).FirstOrDefault(),
                        Location = _context.MsHotellocations.Where(hloc => hloc.Locid == x.locid).Select(hloc => hloc.Locdesc).FirstOrDefault(),
                        RoomPrice = _context.MsHotelRoomRates.Where(rate => rate.Rmtypid == x.rmtypid).Select(rate => rate.Price).FirstOrDefault()
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

        #endregion


        #region // Other Spin-off methods //

        protected string GenerateRefNo(string posId)
        {
            var autoNumber = _context.MsAutonumbers.FirstOrDefault(no => no.Posid == posId && no.Cmpyid == GetCmpyId());
            if (autoNumber == null)
                return "";

            var generateNo = (autoNumber.Lastusedno + 1).ToString();
            if (autoNumber.Zeroleading != null && autoNumber.Zeroleading == true)
            {
                var totalWidth = autoNumber.Runningno - autoNumber.Billprefix.Length - generateNo.Length;
                string paddedString = new string('0', (int)totalWidth) + generateNo;
                return autoNumber.Billprefix + paddedString;
            }
            else
            {
                return autoNumber.Billprefix + generateNo;
            }
        }

        protected PmsReservation? GetReservationByResvNo(string? resvNo)
        {
            var reservation = _context.PmsReservations.FirstOrDefault(resv => resv.Resvno == resvNo && resv.Cmpyid == GetCmpyId());
            return reservation;
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

        public decimal GetRoomPrice(int rmrateId)
        {
            var roomPrice = _context.MsHotelRoomRates
                .Where(rate => rate.Rmrateid == rmrateId && rate.Cmpyid == GetCmpyId())
                .Select(rate => rate.Price)
                .FirstOrDefault();

            return roomPrice;
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
