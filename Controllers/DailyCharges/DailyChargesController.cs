using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Hotel_Core_MVC_V1.Controllers.Folio
{
    [Authorize]
    public class DailyChargesController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public DailyChargesController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            ViewData["inHouseRooms"] = GetInHouseRooms();
            ViewData["SrvcList"] = GetSrvcList();

            return View();
        }

        [HttpPost]
        public async Task<IEnumerable<DailyCharge>> GenerateDailyCharges(string[][] srvcTableData, int roomLgId)
        {
            var list = new List<DailyCharge>();

            var rmLedger = await _context.PmsRoomledgers
                .FirstOrDefaultAsync(ledg => ledg.Roomlgid == roomLgId && ledg.Cmpyid == GetCmpyId());

            if (rmLedger == null) return list;

            var checkIn = await _context.PmsCheckins
                .FirstOrDefaultAsync(chkIn => chkIn.Checkinid == rmLedger.Checkinid && rmLedger.Cmpyid == GetCmpyId());

            if (checkIn == null) return list;

            for (int i = 0; i < checkIn.Nightqty; i++)
            {
                var thisDate = checkIn.Checkindte.AddDays(i).ToString("dd MMM yyyy");

                foreach (var srvc in srvcTableData)
                {
                    if (ParseBool(srvc[2]) == true)
                    {
                        var dailyCharge = new DailyCharge
                        {
                            Date = thisDate,
                            ServiceCode = srvc[0],
                            Amount = ParseDecimal(srvc[1]),
                            Qty = 1,
                            Folio = GetFolio(checkIn.Checkinid, srvc[0])
                        };
                        list.Add(dailyCharge);
                    }
                }
            }

            return list;
        }


        [HttpPost]
        public async Task SaveDailyCharges(string[][] dailyChargesTableData, int roomLgId)
        {
            var checkInId = await _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == roomLgId)
                .Select(ledg => ledg.Checkinid)
                .FirstOrDefaultAsync();

            if (checkInId == null) return;

            // Delete first before new insert
            await _context.PmsRoomdailycharges
                .Where(rdc => rdc.Checkinid == checkInId)
                .ExecuteDeleteAsync();


            foreach (var data in dailyChargesTableData)
            {
                var rmDailyCharge = new PmsRoomdailycharge()
                {
                    Checkinid = checkInId,
                    Occudte = ParseDateTimeExact(data[0]),
                    Srvccde = data[1],
                    Amount = ParseDecimal(data[2]),
                    Qty = ParseInt16(data[3]),
                    Foliohid = GetFolioHId(checkInId, data[4]),
                    Cmpyid = GetCmpyId(),
                    Userid = GetUserId(),
                    Revdtetime = DateTime.Now
                };

                await _context.PmsRoomdailycharges.AddAsync(rmDailyCharge);
            }

            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<DailyCharge>> LoadDailyCharges(int roomLgId)
        {
            var checkInId = await _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == roomLgId)
                .Select(ledg => ledg.Checkinid)
                .FirstOrDefaultAsync();

            if (checkInId == null) return new List<DailyCharge>();

            var list = await _context.PmsRoomdailycharges
                .Where(rdc => rdc.Checkinid == checkInId)
                .Select(rdc => new DailyCharge
                {
                    Date = rdc.Occudte.ToString("dd MMM yyyy"),
                    ServiceCode = rdc.Srvccde,
                    Amount = rdc.Amount,
                    Qty = rdc.Qty,
                    FolioHId = rdc.Foliohid
                })
                .ToListAsync();

            foreach (var data in list)
            {
                data.Folio = GetFolioByFolioCde(data.FolioHId);
            }


            return list;
        }


        #endregion


        #region // In-house Room Modal methods //

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
                    Occudte = chkin.Checkindte,
                    Nightqty = chkin.Nightqty,
                    Roomno = ldg.Roomno,
                }
                )
                .FirstOrDefault();

            if (inHouseRoomInfo != null)
            {
                inHouseRoomInfo.GuestName = GetGuestName(inHouseRoomInfo.CheckInId);
            }

            return inHouseRoomInfo ?? new CheckInModel();
        }

        #endregion


        #region // Service List //

        public IEnumerable<MsHotelservicegrpd> GetSrvcList()
        {
            var list = _context.MsHotelservicegrpds.Where(sgd => sgd.Sysdefine == false).ToList();

            return list;
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

        public IEnumerable<SrvcFolio> GetFolios(int roomLgId) // For Folio Select List
        {
            var checkInId = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == roomLgId && ledg.Cmpyid == GetCmpyId())
                .Select(ledg => ledg.Checkinid)
                .FirstOrDefault();

            var folioList = _context.PmsRoomfoliohs
                .Where(fh => fh.Cmpyid == GetCmpyId() && fh.Checkinid == checkInId)
                .Select(fh => new SrvcFolio
                {
                    FolioCde = fh.Foliocde
                })
                .ToList();

            return folioList;
        }

        public string GetFolio(string checkInId, string srvcCde)
        {
            var folioHId = _context.PmsRoomfoliohs
                        .Where(folio => folio.Checkinid == checkInId && folio.Cmpyid == GetCmpyId())
                        .Join(_context.PmsRoomfoliods,
                        fh => fh.Foliohid,
                        fd => fd.Foliohid,
                        (fh, fd) => new
                        {
                            fh.Foliocde,
                            fd.Srvccde
                        })
                        .Where(gp => gp.Srvccde == srvcCde)
                        .Select(gp => gp.Foliocde)
                        .FirstOrDefault();

            return folioHId ?? CommonItems.CommonStrings.DEFAULT_FOLIO_CODE;
        }

        public string GetFolioByFolioCde(int folioHId)
        {
            var folio = _context.PmsRoomfoliohs
                .Where(fh => fh.Foliohid == folioHId)
                .Select(fh => fh.Foliocde)
                .FirstOrDefault();

            return folio ?? CommonItems.CommonStrings.DEFAULT_FOLIO_CODE;
        }

        public int GetFolioHId(string checkInId, string folioCde)
        {
            var folioHId = _context.PmsRoomfoliohs
                .Where(fh => fh.Checkinid == checkInId && fh.Foliocde == folioCde && fh.Cmpyid == GetCmpyId())
                .Select(fh => fh.Foliohid)
                .FirstOrDefault();

            return folioHId;
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

        static DateTime ParseDateTimeExact(string value)
        {
            string format = "dd MMM yyyy";

            if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
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
