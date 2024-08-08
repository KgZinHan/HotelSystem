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
    public class InHouseGuestController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public InHouseGuestController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var inHouseList = _context.PmsCheckins
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

            foreach (var inHouse in inHouseList)
            {
                // from roomledgers
                var ledger = _context.PmsRoomledgers.Where(ledg => ledg.Roomlgid == inHouse.RoomLgId && ledg.Cmpyid == cmpyId).FirstOrDefault();
                if (ledger != null)
                {

                    inHouse.RmtypId = ledger.Rmtypid;
                    inHouse.Rmprice = ledger.Price;
                    inHouse.Extrabedqty = ledger.Extrabedqty;
                    inHouse.Extrabedprice = ledger.Extrabedprice;
                    inHouse.Discountamt = ledger.Discountamt;
                }

                // from checkins
                var chkIn = _context.PmsCheckins.Where(chkin => chkin.Checkinid == inHouse.CheckInId && chkin.Cmpyid == cmpyId).FirstOrDefault();
                if (chkIn != null)
                {
                    inHouse.Departdte = chkIn.Checkindte.AddDays(chkIn.Nightqty);
                    inHouse.Nightqty = chkIn.Nightqty;
                    inHouse.CheckOutFlag = chkIn.Checkoutflg;
                }

                inHouse.No = inHouseList.IndexOf(inHouse) + 1;
                inHouse.GuestName = GetGuestName(inHouse.CheckInId);
                inHouse.Rmtypdesc = GetRoomTypeCde(inHouse.RmtypId);
                inHouse.Amt = inHouse.Rmprice + (inHouse.Extrabedqty * inHouse.Extrabedprice) - inHouse.Discountamt;
            }

            return View(inHouseList);
        }

        public IActionResult Edit(long id)
        {
            SetLayOutData();

            // From Roomledgers 
            var ldg = _context.PmsRoomledgers.FirstOrDefault(l => l.Roomlgid == id && l.Cmpyid == GetCmpyId());
            if (ldg == null)
            {
                return RedirectToAction("Index");
            }

            var checkIn = new CheckInModel
            {
                RoomLgId = ldg.Roomlgid,
                CheckInId = ldg.Checkinid,
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

            // From Checkins
            var pmsCheckIn = GetCheckIn(checkIn.CheckInId);
            if (pmsCheckIn != null)
            {
                checkIn.Occudte = pmsCheckIn.Checkindte;
                checkIn.StringArriveDte = pmsCheckIn.Checkindte.ToString("dd MMM yyyy");
                checkIn.Nightqty = pmsCheckIn.Nightqty;
                checkIn.Departdte = checkIn.Occudte.AddDays(pmsCheckIn.Nightqty);
                checkIn.StringDepartDte = checkIn.Departdte.ToString("dd MMM yyyy");
                checkIn.Adultqty = pmsCheckIn.Adultqty;
                checkIn.Childqty = pmsCheckIn.Childqty;
                checkIn.ContactName = pmsCheckIn.Contactnme;
                checkIn.ContactNo = pmsCheckIn.Contactno;
                checkIn.SpecialInstruct = pmsCheckIn.Specialinstruct;
                checkIn.Remark = pmsCheckIn.Remark;
            }

            // From CheckInRoomGuests
            var checkInGuestList = GetCheckInGuestList(checkIn.CheckInId);

            var inHouseGuestModel = new InHouseGuestModels()
            {
                CheckInModel = checkIn,
                CheckInGuestList = checkInGuestList
            };

            ViewData["Rmrates"] = new SelectList(_context.MsHotelRoomRates.Where(rate => rate.Cmpyid == GetCmpyId() && rate.Rmtypid == ldg.Rmtypid), "Rmrateid", "Rmratedesc");

            return View(inHouseGuestModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(InHouseGuestModels model)
        {
            var checkIn = model.CheckInModel;
            try
            {
                if (!ModelState.IsValid || checkIn.CheckInId.IsNullOrEmpty())
                {
                    ModelState.AddModelError(string.Empty, CommonItems.CommonStrings.COMMON_ERROR_MESSAGE);
                    model.CheckInGuestList = GetCheckInGuestList(checkIn.CheckInId);
                    ViewData["Rmrates"] = new SelectList(_context.MsHotelRoomRates.Where(rate => rate.Cmpyid == GetCmpyId() && rate.Rmtypid == checkIn.RmtypId), "Rmrateid", "Rmratedesc");
                    return View(model);
                }

                var cmpyId = GetCmpyId();
                var userId = GetUserId();
                var hotelDate = GetHotelDate();

                // Update Ledger  
                var ledgerList = await _context.PmsRoomledgers
                    .Where(lgr => lgr.Checkinid == checkIn.CheckInId && lgr.Occudte.Date >= hotelDate.Date && lgr.Cmpyid == cmpyId)
                    .ToListAsync();
                if (ledgerList != null)
                {
                    foreach (var roomLedger in ledgerList)
                    {
                        roomLedger.Roomno = checkIn.Roomno;
                        roomLedger.Rmrateid = checkIn.Rmrateid;
                        roomLedger.Price = checkIn.Rmprice;
                        roomLedger.Extrabedqty = checkIn.Extrabedqty;
                        roomLedger.Extrabedprice = checkIn.Extrabedprice;
                        roomLedger.Discountamt = checkIn.Discountamt;
                        roomLedger.Hkeepingflg = checkIn.HKeepingFlg;
                        roomLedger.Revdtetime = DateTime.Now;
                        roomLedger.Userid = userId;
                        _context.PmsRoomledgers.Update(roomLedger);
                    }
                }

                // Update Hotel Room
                var hotelRoom = _context.MsHotelRooms.FirstOrDefault(hr => hr.Roomno == checkIn.Roomno && hr.Cmpyid == cmpyId);
                if (hotelRoom != null)
                {
                    hotelRoom.Occuflg = true;
                    _context.MsHotelRooms.Update(hotelRoom);
                }

                // Update CheckIn
                var dbCheckIn = await _context.PmsCheckins.FirstOrDefaultAsync(chkIn => chkIn.Checkinid == checkIn.CheckInId && chkIn.Cmpyid == cmpyId);
                if (dbCheckIn != null)
                {
                    dbCheckIn.Nightqty = checkIn.Nightqty; // newly added
                    dbCheckIn.Specialinstruct = checkIn.SpecialInstruct;
                    dbCheckIn.Adultqty = checkIn.Adultqty;
                    dbCheckIn.Childqty = checkIn.Childqty;
                    dbCheckIn.Remark = checkIn.Remark;
                    dbCheckIn.Revdtetime = DateTime.Now;
                    dbCheckIn.Userid = userId;
                    _context.PmsCheckins.Update(dbCheckIn);
                };

                await _context.SaveChangesAsync();

                return RedirectToAction("Add", "GuestBilling", new { id = checkIn.RoomLgId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                model.CheckInGuestList = GetCheckInGuestList(checkIn.CheckInId);
                ViewData["Rmrates"] = new SelectList(_context.MsHotelRoomRates.Where(rate => rate.Cmpyid == GetCmpyId() && rate.Rmtypid == checkIn.RmtypId), "Rmrateid", "Rmratedesc");
                return View(model);
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
            catch
            {
                return PartialView("_ChooseRoomsPartialView", new List<AvailableRoomNumber>());
            }
        }

        #endregion


        #region // Guests //

        public IActionResult GuestInfo(string checkInId)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            SetLayOutData();

            // Guest Info List

            var guestList = _context.PmsCheckinroomguests
                .Where(rg => rg.Checkinid == checkInId)
                .Join(_context.MsGuestdata,
                rg => rg.Guestid,
                gd => gd.Guestid,
                (rg, gd) => new GuestInfo
                {
                    Guestid = gd.Guestid,
                    Guestfullnme = gd.Guestfullnme,
                    Nationality = _context.MsGuestnationalities.Where(n => n.Nationid == gd.Nationid).Select(n => n.Nationdesc).FirstOrDefault(),
                    State = _context.MsGueststates.Where(s => s.Gstateid == gd.Stateid).Select(s => s.Gstatedesc).FirstOrDefault(),
                    Country = _context.MsGuestcountries.Where(c => c.Countryid == gd.Countryid).Select(c => c.Countrydesc).FirstOrDefault(),
                    LastVistedDate = gd.Lastvisitdte < new DateTime(1990, 1, 1) ? null : gd.Lastvisitdte,
                    VisitCount = gd.Visitcount
                })
            .ToList();


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
                GuestInfos = guestList,
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

        public void SaveGuests(string[][] guests, string checkInId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                // Delete previous Pmscheckinroomguest Data First
                var oldCheckInGuest = _context.PmsCheckinroomguests
                    .Where(rg => rg.Checkinid == checkInId)
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

                    // Insert new PmsCheckinroomguest
                    var roomGuest = new PmsCheckinroomguest()
                    {
                        Checkinid = checkInId,
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
                    PrincipleFlg = chkIn.Principleflg,
                    LastVistedDate = guest.Lastvisitdte,
                    VisitCount = guest.Visitcount
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
