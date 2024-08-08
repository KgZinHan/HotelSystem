using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using static Hotel_Core_MVC_V1.Common.DefaultValues;

namespace Hotel_Core_MVC_V1.Controllers.CheckIn
{
    [Authorize]
    public class CheckInController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public CheckInController(HotelCoreMvcContext context)
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
                .Where(ldg => ldg.Occudte.Date == hotelDte.Date
                && ldg.Occustate.Equals(CommonItems.CommonStrings.LEDGER_RESERVE)
                && ldg.Cmpyid == cmpyId)
                .Select(ldg => new CheckInModel
                {
                    RoomLgId = ldg.Roomlgid,
                    Resvno = ldg.Resvno,
                    Occudte = ldg.Occudte,
                    RmtypId = ldg.Rmtypid,
                    Rmrateid = ldg.Rmrateid,
                    Extrabedqty = ldg.Extrabedqty
                })
                .ToList();

            foreach (var checkIn in checkInList)
            {
                var resv = GetReservationByResvNo(checkIn.Resvno);

                if (resv != null)
                {
                    checkIn.Nightqty = resv.Nightqty;
                    checkIn.Departdte = checkIn.Occudte.AddDays(resv.Nightqty);
                }

                checkIn.No = checkInList.IndexOf(checkIn) + 1;
                checkIn.Rmtypdesc = GetRoomTypeCde(checkIn.RmtypId);
                checkIn.PaxNo = GetPaxNo(checkIn.RmtypId);
                checkIn.Rmprice = GetRoomPrice(checkIn.Rmrateid);
                checkIn.Amt = checkIn.Rmprice + (checkIn.Extrabedqty * checkIn.Extrabedprice) - checkIn.Discountamt;
            }

            return View(checkInList);
        }

        public IActionResult Edit(long id)
        {
            SetLayOutData();

            // From Roomledger table
            var ldg = _context.PmsRoomledgers.FirstOrDefault(l => l.Roomlgid == id && l.Cmpyid == GetCmpyId());
            if (ldg == null)
            {
                return RedirectToAction("Index");
            }

            var checkIn = new CheckInModel
            {
                RoomLgId = ldg.Roomlgid,
                Occudte = ldg.Occudte,
                StringArriveDte = ldg.Occudte.ToString("dd MMM yyyy"),
                Resvno = ldg.Resvno,
                BatchNo = ldg.Batchno,
                RmtypId = ldg.Rmtypid,
                Rmrateid = ldg.Rmrateid,
                Extrabedqty = ldg.Extrabedqty,
                Extrabedprice = ldg.Extrabedprice,
                Discountamt = ldg.Discountamt,
                Roomno = ldg.Roomno ?? "",
                HKeepingFlg = ldg.Hkeepingflg,
                Rmtypdesc = GetRoomTypeCde(ldg.Rmtypid),
                PaxNo = GetPaxNo(ldg.Rmtypid),
                Rmprice = ldg.Price
            };
            checkIn.Amt = checkIn.Rmprice + (checkIn.Extrabedqty * checkIn.Extrabedprice) - checkIn.Discountamt;

            // From Reservation table
            var resv = GetReservationByResvNo(checkIn.Resvno);
            if (resv != null)
            {
                var departDte = checkIn.Occudte.AddDays(resv.Nightqty);
                checkIn.Departdte = departDte;
                checkIn.StringDepartDte = departDte.ToString("dd MMM yyyy");
                checkIn.Nightqty = resv.Nightqty;
                checkIn.Adultqty = resv.Adult;
                checkIn.Childqty = resv.Child;
                checkIn.ContactName = resv.Contactnme;
                checkIn.ContactNo = resv.Contactno;
            }

            ViewData["Rmrates"] = new SelectList(_context.MsHotelRoomRates.Where(rate => rate.Cmpyid == GetCmpyId() && rate.Rmtypid == ldg.Rmtypid), "Rmrateid", "Rmratedesc");

            return View(checkIn);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CheckInModel checkIn)
        {
            SetLayOutData();

            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, CommonItems.CommonStrings.COMMON_ERROR_MESSAGE);
                    ViewData["Rmrates"] = new SelectList(_context.MsHotelRoomRates.Where(rate => rate.Cmpyid == GetCmpyId() && rate.Rmtypid == checkIn.RmtypId), "Rmrateid", "Rmratedesc");
                    return View(checkIn);
                }

                var generatedChkInId = GenerateRefNo("CHK");
                var cmpyId = GetCmpyId();
                var userId = GetUserId();
                var hotelDate = GetHotelDate();

                // Update Ledger 
                var ledgerList = _context.PmsRoomledgers
                    .Where(ledg => ledg.Resvno == checkIn.Resvno && ledg.Batchno == checkIn.BatchNo && ledg.Occudte.Date >= hotelDate.Date && ledg.Cmpyid == cmpyId)
                    .ToList();

                foreach (var roomLedger in ledgerList)
                {
                    roomLedger.Checkinid = generatedChkInId;
                    roomLedger.Roomno = checkIn.Roomno;
                    roomLedger.Occustate = CommonItems.CommonStrings.LEDGER_OCCUPIED;
                    roomLedger.Rmrateid = checkIn.Rmrateid;
                    roomLedger.Price = checkIn.Rmprice;
                    roomLedger.Extrabedqty = checkIn.Extrabedqty;
                    roomLedger.Extrabedprice = checkIn.Extrabedprice;
                    roomLedger.Discountamt = checkIn.Discountamt;
                    roomLedger.Revdtetime = DateTime.Now;
                    roomLedger.Userid = userId;
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
                    Contactnme = checkIn.ContactName,
                    Contactno = checkIn.ContactNo,
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

                return RedirectToAction("Edit", "GuestBilling", new { id = checkIn.RoomLgId });

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewData["Rmrates"] = new SelectList(_context.MsHotelRoomRates.Where(rate => rate.Cmpyid == GetCmpyId() && rate.Rmtypid == checkIn.RmtypId), "Rmrateid", "Rmratedesc");
                return View(checkIn);
            }

        }

        #endregion


        #region // Available rooms //

        public IActionResult GetRoomsList(int nightQty, int roomTypeId, long roomLgId)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            try
            {
                var cmpyId = GetCmpyId();

                var ledger = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == roomLgId && ledg.Cmpyid == cmpyId)
                .FirstOrDefault();

                var roomList = _context.HotelRoomNumberInfoDBSet
                    .FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @noofday = {2}, @cmpyid = {3},@departureDate = {4}", 1, ledger.Occudte, nightQty, cmpyId, ledger.Occudte)
                    .AsEnumerable()
                    .Select(x => new AvailableRoomNumber
                    {
                        RoomNo = x.roomno,
                        RmTypId = x.rmtypid,
                        RoomType = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypid == x.rmtypid).Select(typ => typ.Rmtypcde).FirstOrDefault(),
                        BedCde = _context.MsHotelroombeddings.Where(bed => bed.Bedid == x.bedid).Select(bed => bed.Bedcde).FirstOrDefault(),
                        RoomLgId = roomLgId
                    })
                    .Where(gp => gp.RmTypId == roomTypeId)
                    .ToList();

                foreach (var room in roomList)
                {
                    room.No = roomList.IndexOf(room) + 1;
                }
                return PartialView("_ChooseRoomsPartialView", roomList);
            }
            catch (Exception ex)
            {
                return PartialView("_ChooseRoomsPartialView", new List<AvailableRoomNumber>());
            }


            /*var roomList = _context.MsHotelRooms
                .Where(rm => rm.Rmtypid == roomTypeId && rm.Cmpyid == GetCmpyId())
                .Select(rm => new AvailableRoomNumber
                {
                    RoomId = rm.Roomid,
                    RoomNo = rm.Roomno,
                    Location = _context.MsHotellocations.Where(loc => loc.Locid == rm.Locid).Select(loc => loc.Loccde).FirstOrDefault(),
                    RoomType = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypid == rm.Rmtypid).Select(typ => typ.Rmtypcde).FirstOrDefault(),
                    BedCde = _context.MsHotelroombeddings.Where(bed => bed.Bedid == rm.Bedid).Select(bed => bed.Bedcde).FirstOrDefault(),
                    RoomPrice = _context.MsHotelRoomRates.Where(rate => rate.Rmtypid == rm.Rmtypid).Select(rate => rate.Price).FirstOrDefault(),
                    RoomLgId = roomLgId
                })
                .OrderBy(rm => rm.RoomNo)
                .ToList();

            foreach (var room in roomList)
            {
                room.No = roomList.IndexOf(room) + 1;
            }*/


        }

        #endregion


        #region // Guests //

        public IActionResult GuestInfo()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            SetLayOutData();

            // Default Assign for Guest Form
            var guestForm = new MsGuestdatum()
            {
                Chrgacccde = CommonItems.CommonStrings.DEFAULT_GUEST_CODE,
                Nationid = GlobalDefault.NationId,
                Stateid = GlobalDefault.StateId,
                Saluteid = GlobalDefault.SaluteId,
                Countryid = GlobalDefault.CountryId
            };

            var guestChoose = new GuestChoose()
            {
                GuestInfos = new List<GuestInfo>(),
                Guest = guestForm
            };

            ViewData["GuestSalutations"] = new SelectList(_context.MsGuestsalutations, "Saluteid", "Salutedesc");
            ViewData["GuestCountries"] = new SelectList(_context.MsGuestcountries, "Countryid", "Countrydesc");
            ViewData["GuestStates"] = new SelectList(_context.MsGueststates.Where(s => s.Countryid == guestForm.Countryid), "Gstateid", "Gstatedesc");
            ViewData["GuestNationalities"] = new SelectList(_context.MsGuestnationalities, "Nationid", "Nationdesc");

            return PartialView("_ChooseGuestPartialView", guestChoose);
        }

        public async Task<IActionResult> SearchGuest([FromQuery] string keyword, [FromQuery] string[] methods)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            var cmpyId = GetCmpyId();

            var query = _context.MsGuestdata.AsQueryable().AsNoTracking();

            if (!methods.IsNullOrEmpty() && !keyword.IsNullOrEmpty())
            {
                query = query.Where(g =>
                   (methods.Contains("contain") && g.Guestfullnme.Contains(keyword)) ||
                   (methods.Contains("start") && g.Guestfullnme.StartsWith(keyword)) ||
                   (methods.Contains("end") && g.Guestfullnme.EndsWith(keyword))
                 && g.Cmpyid == cmpyId);
            }

            var guestList = await query
                .Select(g => new GuestInfo
                {
                    Guestid = g.Guestid,
                    Guestfullnme = g.Guestfullnme,
                    Nationality = g.Idppno,
                    ChrgAccCde = g.Chrgacccde ?? "",
                    LastVistedDate = g.Lastvisitdte,
                    VisitCount = g.Visitcount
                })
                .ToListAsync();

            return PartialView("_GuestSearchPartialView", guestList);
        }

        public MsGuestdatum GetGuestById(int guestId)
        {
            var guest = _context.MsGuestdata.FirstOrDefault(guest => guest.Guestid == guestId && guest.Cmpyid == GetCmpyId());
            return guest ?? new MsGuestdatum();
        }

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
                        Chrgacccde = guest[5],
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


        #region // Country //

        public async Task<IEnumerable<MsGueststate>> ChangeStateSelectList(int countryId)
        {
            var states = await _context.MsGueststates
                .Where(state => state.Countryid == countryId)
                .ToListAsync();

            return states;
        }

        public int CreateCountry(string countryDesc)
        {
            var country = new MsGuestcountry()
            {
                Countrydesc = countryDesc,
                Defstateid = 1,
                Userid = GetUserId(),
                Revdtetime = DateTime.Now
            };

            _context.MsGuestcountries.Add(country);
            _context.SaveChanges();

            return country.Countryid;
        }


        #endregion


        #region // State //

        public int CreateState(string stateDesc, int countryId)
        {
            var state = new MsGueststate()
            {
                Gstatedesc = stateDesc,
                Countryid = countryId,
                Userid = GetUserId(),
                Revdtetime = DateTime.Now
            };

            _context.MsGueststates.Add(state);
            _context.SaveChanges();

            return state.Gstateid;
        }

        #endregion


        #region // Nationality //

        public int CreateNationality(string nationDesc)
        {
            var nation = new MsGuestnationality()
            {
                Nationdesc = nationDesc,
                Userid = GetUserId(),
                Revdtetime = DateTime.Now
            };

            _context.MsGuestnationalities.Add(nation);
            _context.SaveChanges();

            return nation.Nationid;
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
