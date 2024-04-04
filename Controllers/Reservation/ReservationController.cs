using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers.Reservation
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public ReservationController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        #region  // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelIinfo = await _context.MsHotelinfos.FirstOrDefaultAsync(h => h.Cmpyid == cmpyId);
            return View(hotelIinfo);
        }

        [HttpPost]
        public async Task<IActionResult> SearchRooms(string arrivalDate, string checkoutDate)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            var dbRoomList = await _context.MsHotelRoomTypes.ToListAsync();
            var formattedArrivalDate = DateTime.Parse(arrivalDate);
            var formattedCheckOutDate = DateTime.Parse(checkoutDate);
            var cmpyId = GetCmpyId();

            try
            {
                var rawList = _context.HotelRoomInfoDBSet
                .FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @departureDate = {2}, @cmpyid = {3}", 0, formattedArrivalDate, formattedCheckOutDate, cmpyId)
                .AsEnumerable()
                .Select(x => new AvailableRoomTypeModel
                {
                    Price = x.Price,
                    AvilQty = x.AvilQty,
                    CmpyID = x.CmpyID,
                    RateID = x.RateID,
                    rmtypdesc = x.rmtypdesc,
                    RoomType = x.RoomType
                })
                .ToList();

                var roomList = new List<HotelRoomTypeModel>();
                foreach (var dbRoomType in rawList)
                {
                    var room = new HotelRoomTypeModel()
                    {
                        Rmtypid = dbRoomType.RoomType,
                        Rmtypdesc = dbRoomType.rmtypdesc,
                        Paxno = GetPaxNo(dbRoomType.RoomType),
                        Price = dbRoomType.Price,
                        Extrabedprice = GetExtraBedPrice(dbRoomType.RoomType),
                        Quantity = dbRoomType.AvilQty,
                        RoomRateId = dbRoomType.RateID
                    };

                    var mainImage = await _context.MsHotelRoomTypeImages
                        .Where(r => r.Rmtypid == dbRoomType.RoomType && r.Mainimgflg == true)
                        .Select(r => r.Rmtypmainimg)
                        .FirstOrDefaultAsync();
                    room.Base64Image = mainImage != null ? Convert.ToBase64String(mainImage) : "";
                    roomList.Add(room);
                }
                return PartialView("_ReservationPartialView", roomList);
            }
            catch
            {
                return PartialView();
            }
        }

        public async Task<List<string>> ShowImages(int rmTypId)
        {
            var imagesList = new List<string>();
            var images = await _context.MsHotelRoomTypeImages
                .Where(img => img.Rmtypid == rmTypId && img.Mainimgflg == false)
                .Select(img => img.Rmtypmainimg)
                .ToListAsync();

            foreach (var image in images)
            {
                var base64Image = image != null ? Convert.ToBase64String(image) : "";
                imagesList.Add(base64Image);
            }

            return imagesList;
        }

        public IActionResult ShowCheckOut(string arrivalDate, string checkoutDate, string[][] hotelRoomData)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            var roomTypeList = new List<HotelRoomTypeModel>();
            foreach (var room in hotelRoomData)
            {
                var roomType = new HotelRoomTypeModel()
                {
                    Rmtypcde = GetRoomTypeDesc(ParseInt32(room[0])),
                    Price = ParseDecimal(room[1]),
                    Quantity = ParseInt32(room[2]),
                    ExtraBedQty = ParseInt32(room[3]),
                    ArrivalDate = ParseDateTime(arrivalDate),
                    CheckOutDate = ParseDateTime(checkoutDate)
                };
                roomTypeList.Add(roomType);
            }

            return PartialView("_UserInfoPartialView", roomTypeList);
        }

        public async Task<IActionResult> SaveCheckOut([FromBody] ReservationJSList resvList)
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return StatusCode(401);
            }

            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var hotelRoomInfos = resvList.HotelRoomInfos;
            var userInfos = resvList.UserInfos;

            var reservation = BuildReservation(userInfos);
            await _context.PmsReservations.AddAsync(reservation);

            short batchNo = 1;

            try
            {
                foreach (var room in hotelRoomInfos)
                {
                    var rmtypId = ParseInt32(room[0]);
                    var roomPrice = ParseDecimal(room[1]);
                    var totalRooms = ParseInt32(room[2]);
                    var totalExtraBeds = ParseInt32(room[3]);
                    var roomRateId = ParseInt32(room[4]);
                    var bedflg = true;

                    for (var i = 0; i < totalRooms; i++)
                    {
                        short extraBedQty = 0;

                        if (bedflg) // add all extra bed to first room 
                        {
                            extraBedQty = (short)totalExtraBeds;
                            bedflg = false;
                        }

                        var refNo = GenerateRefNo("RSV");

                        // Insert roomLedger
                        for (var j = 0; j < reservation.Nightqty; j++)
                        {
                            var roomLedger = new PmsRoomledger()
                            {
                                Resvno = refNo,
                                Occudte = reservation.Arrivedte.AddDays(j),
                                Occustate = CommonItems.CommonStrings.LEDGER_RESERVE,
                                Hkeepingflg = false,
                                Rmtypid = rmtypId,
                                Rmrateid = roomRateId,
                                Price = roomPrice,
                                Extrabedqty = extraBedQty,
                                Extrabedprice = GetExtraBedPrice(rmtypId),
                                Discountamt = 0,
                                Batchno = batchNo,
                                Cmpyid = cmpyId,
                                Revdtetime = DateTime.Now,
                                Userid = userId
                            };

                            await _context.PmsRoomledgers.AddAsync(roomLedger);
                        }
                        batchNo++;
                    }
                }

                // Update autonumbers
                var resvNo = await _context.MsAutonumbers.FirstOrDefaultAsync(no => no.Posid == "RSV" && no.Cmpyid == cmpyId);
                if (resvNo != null)
                {
                    resvNo.Lastusedno += 1;
                    _context.MsAutonumbers.Update(resvNo);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return RedirectToAction("Index");

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

        protected string GetRoomTypeDesc(int rmtypId)
        {
            var roomTypCde = _context.MsHotelRoomTypes
                .Where(rm => rm.Rmtypid == rmtypId && rm.Cmpyid == GetCmpyId())
                .Select(rm => rm.Rmtypdesc)
                .FirstOrDefault();

            return roomTypCde ?? "";
        }

        protected decimal GetExtraBedPrice(int rmtypId)
        {
            var extraBedPrice = _context.MsHotelRoomTypes
                .Where(r => r.Rmtypid == rmtypId && r.Cmpyid == GetCmpyId())
                .Select(r => r.Extrabedprice)
                .FirstOrDefault();

            if (extraBedPrice != null)
            {
                return (decimal)extraBedPrice;
            }
            else
            {
                return 0;
            }
        }

        protected int GetPaxNo(int rmtypId)
        {
            var paxNo = _context.MsHotelRoomTypes
                .Where(r => r.Rmtypid == rmtypId && r.Cmpyid == GetCmpyId())
                .Select(r => r.Paxno)
                .FirstOrDefault();

            return paxNo;

        }

        private PmsReservation BuildReservation(Dictionary<string, string> userInfo)
        {
            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var reservation = new PmsReservation()
            {
                Resvno = GenerateRefNo("RSV"),
                Resvdtetime = DateTime.Now,
                Resvmadeby = "Website",
                Resvstate = "R",
                Reqpickup = false,
                Revdtetime = DateTime.Now,
                Cmpyid = cmpyId,
                Userid = userId
            };

            foreach (var field in userInfo)
            {
                if (field.Value != null)
                {
                    switch (field.Key)
                    {
                        case "arrivalDate":
                            reservation.Arrivedte = ParseDateTime(field.Value);
                            break;
                        case "nightQty":
                            reservation.Nightqty = ParseInt16(field.Value);
                            break;
                        case "contactName":
                            reservation.Contactnme = field.Value;
                            break;
                        case "contactNo":
                            reservation.Contactno = field.Value;
                            break;
                        case "totalAdult":
                            reservation.Adult = ParseInt8(field.Value);
                            break;
                        case "totalChild":
                            reservation.Child = ParseInt8(field.Value);
                            break;
                        case "specialRemark":
                            reservation.Specialremark = field.Value;
                            break;
                        case "vipStatus":
                            reservation.Vipstatus = ParseBool(field.Value);
                            break;
                    }
                }
            }

            return reservation;
        }

        #endregion


        #region // Parse methods //


        static byte ParseInt8(string value)
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

        static DateTime ParseDateTime(string value)
        {
            if (DateTime.TryParse(value, out DateTime result))
            {
                return result;
            }
            return default;
        }

        static bool ParseBool(string value)
        {
            if (bool.TryParse(value, out bool result))
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
