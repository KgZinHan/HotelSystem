using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Hotel_Core_MVC_V1.Controllers.RoomEnquiry
{
    [Authorize]
    public class RoomEnquiryController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public RoomEnquiryController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var hotelDate = GetHotelDate();
            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var defNightCnt = 7; // Assign default night count

            // main data source
            var rawList = _context.RoomEnquiryDBSet.FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @noofday = {2}, @cmpyid = {3},@departureDate = {4}", 2, hotelDate, defNightCnt, cmpyId, hotelDate)
                    .AsEnumerable()
                    .Select(x => new RoomEnquiryDB
                    {
                        ThisDate = x.theDate,
                        RoomType = x.rmtypcde,
                        RoomQty = x.roomqty,
                        BookQty = x.bookqty,
                        OccuState = x.occustate
                    })
                    .ToList();

            // for table header date list
            var dateList = new List<DateTime>();

            for (var i = 0; i < defNightCnt; i++)
            {
                var date = hotelDate.AddDays(i);
                dateList.Add(date);
            }

            // for room info list
            var rmInfoList = new List<RoomEnquiryInfo>();
            var roomTypes = _context.MsHotelRoomTypes.ToList();

            foreach (var roomType in roomTypes)
            {
                var roomInfo = new RoomEnquiryInfo();

                for (var i = 0; i < dateList.Count; i++)
                {
                    roomInfo.RoomType = roomType.Rmtypcde;
                    roomInfo.RmTypDesc = roomType.Rmtypdesc ?? "";
                    var propertyInfo = typeof(RoomEnquiryInfo).GetProperty($"Info{i + 1}");

                    if (propertyInfo != null)
                    {
                        var info = rawList.Where(raw => raw.ThisDate.Date == dateList[i].Date && raw.RoomType == roomType.Rmtypcde).Select(raw => raw.RoomQty - raw.BookQty).FirstOrDefault();
                        roomInfo.GetType().GetProperty($"Info{i + 1}")?.SetValue(roomInfo, info);
                    }
                }

                rmInfoList.Add(roomInfo);
            }

            // other data
            var totalRoom = rawList.Where(rl => rl.OccuState != CommonItems.CommonStrings.LEDGER_RESERVE && rl.OccuState != CommonItems.CommonStrings.LEDGER_MAINTENANCE).Sum(rl => rl.RoomQty);
            var occuRooms = rawList.Where(rl => rl.OccuState == CommonItems.CommonStrings.LEDGER_OCCUPIED).Sum(rl => rl.BookQty);
            var mteRooms = rawList.Where(rl => rl.OccuState == CommonItems.CommonStrings.LEDGER_MAINTENANCE).Sum(rl => rl.BookQty);
            var resvRooms = rawList.Where(rl => rl.OccuState == CommonItems.CommonStrings.LEDGER_RESERVE).Sum(rl => rl.BookQty);
            float occupancy = ((float)occuRooms + mteRooms + resvRooms) * 100 / totalRoom;
            occupancy = (float)Math.Round(occupancy, 2);

            var roomEnquiryModels = new RoomEnquiryModels()
            {
                DateList = dateList,
                RoomInfoList = rmInfoList,
                NightCount = defNightCnt,
                TotalRoom = totalRoom,
                Occupied = occuRooms,
                Maintenance = mteRooms,
                Reserved = resvRooms,
                Occupancy = occupancy
            };

            ViewData["RmTypes"] = new SelectList(_context.MsHotelRoomTypes.Where(typ => typ.Cmpyid == cmpyId), "Rmtypcde", "Rmtypdesc");

            return View(roomEnquiryModels);
        }

        public IActionResult RoomEnquiry([Bind("rmTypCde")] string rmTypCde, [Bind("count")] int count)
        {
            SetLayOutData();

            var hotelDate = GetHotelDate();
            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            // main data source
            var rawList = _context.RoomEnquiryDBSet.FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @noofday = {2}, @cmpyid = {3},@departureDate = {4}", 2, hotelDate, count, cmpyId, hotelDate)
                    .AsEnumerable()
                    .Select(x => new RoomEnquiryDB
                    {
                        ThisDate = x.theDate,
                        RoomType = x.rmtypcde,
                        RoomQty = x.roomqty,
                        BookQty = x.bookqty,
                        OccuState = x.occustate
                    })
                    .ToList();

            // for table header date list
            var dateList = new List<DateTime>();

            for (var i = 0; i < count; i++)
            {
                var date = hotelDate.AddDays(i);
                dateList.Add(date);
            }

            // for room info list
            var rmInfoList = new List<RoomEnquiryInfo>();

            int totalRoom = 0;
            int occuRooms = 0;
            int mteRooms = 0;
            int resvRooms = 0;
            float occupancy = 0;

            if (rmTypCde.IsNullOrEmpty())
            {
                var roomTypes = _context.MsHotelRoomTypes.ToList();

                foreach (var roomType in roomTypes)
                {
                    var roomInfo = new RoomEnquiryInfo();

                    for (var i = 0; i < dateList.Count; i++)
                    {
                        roomInfo.RoomType = roomType.Rmtypcde;
                        roomInfo.RmTypDesc = roomType.Rmtypdesc ?? "";
                        var propertyInfo = typeof(RoomEnquiryInfo).GetProperty($"Info{i + 1}");

                        if (propertyInfo != null)
                        {
                            var info = rawList.Where(raw => raw.ThisDate.Date == dateList[i].Date && raw.RoomType == roomType.Rmtypcde).Select(raw => raw.RoomQty - raw.BookQty).FirstOrDefault();
                            roomInfo.GetType().GetProperty($"Info{i + 1}")?.SetValue(roomInfo, info);
                        }
                    }
                    rmInfoList.Add(roomInfo);
                }

                // other data
                totalRoom = rawList.Where(rl => rl.OccuState != CommonItems.CommonStrings.LEDGER_RESERVE && rl.OccuState != CommonItems.CommonStrings.LEDGER_MAINTENANCE).Sum(rl => rl.RoomQty);
                occuRooms = rawList.Where(rl => rl.OccuState == CommonItems.CommonStrings.LEDGER_OCCUPIED).Sum(rl => rl.BookQty);
                mteRooms = rawList.Where(rl => rl.OccuState == CommonItems.CommonStrings.LEDGER_MAINTENANCE).Sum(rl => rl.BookQty);
                resvRooms = rawList.Where(rl => rl.OccuState == CommonItems.CommonStrings.LEDGER_RESERVE).Sum(rl => rl.BookQty);
                occupancy = ((float)occuRooms + mteRooms + resvRooms) * 100 / totalRoom;
                occupancy = (float)Math.Round(occupancy, 2);
            }
            else
            {
                var roomInfo = new RoomEnquiryInfo();
                var rmTypDesc = _context.MsHotelRoomTypes.Where(typ => typ.Rmtypcde == rmTypCde && typ.Cmpyid == cmpyId).Select(typ => typ.Rmtypdesc).FirstOrDefault();

                for (var i = 0; i < dateList.Count; i++)
                {
                    roomInfo.RoomType = rmTypCde;
                    roomInfo.RmTypDesc = rmTypDesc ?? "";
                    var propertyInfo = typeof(RoomEnquiryInfo).GetProperty($"Info{i + 1}");

                    if (propertyInfo != null)
                    {
                        var info = rawList.Where(raw => raw.ThisDate.Date == dateList[i].Date && raw.RoomType == rmTypCde).Select(raw => raw.RoomQty - raw.BookQty).FirstOrDefault();
                        roomInfo.GetType().GetProperty($"Info{i + 1}")?.SetValue(roomInfo, info);
                    }
                }
                rmInfoList.Add(roomInfo);

                // other data
                totalRoom = rawList.Where(rl => rl.RoomType == rmTypCde).Sum(rl => rl.RoomQty);
                occuRooms = rawList.Where(rl => rl.OccuState == CommonItems.CommonStrings.LEDGER_OCCUPIED && rl.RoomType == rmTypCde).Sum(rl => rl.BookQty);
                mteRooms = rawList.Where(rl => rl.OccuState == CommonItems.CommonStrings.LEDGER_MAINTENANCE && rl.RoomType == rmTypCde).Sum(rl => rl.BookQty);
                resvRooms = rawList.Where(rl => rl.OccuState == CommonItems.CommonStrings.LEDGER_RESERVE && rl.RoomType == rmTypCde).Sum(rl => rl.BookQty);
                occupancy = (occuRooms + mteRooms + resvRooms) * 100 / totalRoom;
            }

            var roomEnquiryModels = new RoomEnquiryModels()
            {
                DateList = dateList,
                RoomInfoList = rmInfoList,
                NightCount = count,
                TotalRoom = totalRoom,
                Occupied = occuRooms,
                Maintenance = mteRooms,
                Reserved = resvRooms,
                Occupancy = occupancy
            };

            ViewData["RmTypes"] = new SelectList(_context.MsHotelRoomTypes.Where(typ => typ.Cmpyid == cmpyId), "Rmtypcde", "Rmtypdesc");

            return View("~/Views/RoomEnquiry/Index.cshtml", roomEnquiryModels);
        }

        public IActionResult RoomOccupancy(string rmTypCde)
        {
            SetLayOutData();
            var hotelDate = GetHotelDate();
            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var defNightCnt = 7; // Assign default night count

            var rawList = new List<RoomNoEnquiryDB>();
            var dateList = new List<DateTime>();
            var roomNos = new List<string>();

            if (rmTypCde == "ALL")
            {
                // main data source
                rawList = _context.RoomNoEnquiryDBSet.FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @noofday = {2}, @cmpyid = {3},@departureDate = {4}", 3, hotelDate, defNightCnt, cmpyId, hotelDate)
                        .AsEnumerable()
                        .Select(x => new RoomNoEnquiryDB
                        {
                            ThisDate = x.theDate,
                            RoomNo = x.roomno,
                            RoomType = x.rmtypcde,
                            RmTypId = x.rmtypid,
                            OccuDate = x.occudte,
                            GuestFullName = x.guestfullnme
                        })
                        .ToList();

                roomNos = _context.MsHotelRooms.Where(rm => rm.Cmpyid == cmpyId).Select(rm => rm.Roomno).ToList();
            }
            else
            {
                // main data source
                rawList = _context.RoomNoEnquiryDBSet.FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @noofday = {2}, @cmpyid = {3},@departureDate = {4}", 3, hotelDate, defNightCnt, cmpyId, hotelDate)
                        .AsEnumerable()
                        .Select(x => new RoomNoEnquiryDB
                        {
                            ThisDate = x.theDate,
                            RoomNo = x.roomno,
                            RoomType = x.rmtypcde,
                            RmTypId = x.rmtypid,
                            OccuDate = x.occudte,
                            GuestFullName = x.guestfullnme
                        })
                        .Where(rl => rl.RoomType == rmTypCde)
                        .ToList();

                var rmTypId = rawList.Select(rl => rl.RmTypId).FirstOrDefault();
                roomNos = _context.MsHotelRooms.Where(rm => rm.Rmtypid == rmTypId && rm.Cmpyid == cmpyId).Select(rm => rm.Roomno).ToList();
            }

            // for table header date list
            for (var i = 0; i < defNightCnt; i++)
            {
                var date = hotelDate.AddDays(i);
                dateList.Add(date);
            }

            // for room number info list
            var rmNoInfoList = new List<RoomNoEnquiryInfo>();

            foreach (var roomNo in roomNos)
            {
                var roomNoInfo = new RoomNoEnquiryInfo()
                {
                    RoomNo = roomNo
                };

                for (var i = 0; i < dateList.Count; i++)
                {
                    var propertyInfo = typeof(RoomNoEnquiryInfo).GetProperty($"Info{i + 1}");
                    if (propertyInfo != null)
                    {
                        var info = rawList.Where(raw => raw.ThisDate.Date == dateList[i].Date && raw.RoomNo == roomNo).Select(raw => raw.GuestFullName).FirstOrDefault();
                        roomNoInfo.GetType().GetProperty($"Info{i + 1}")?.SetValue(roomNoInfo, info);
                    }
                }
                rmNoInfoList.Add(roomNoInfo);
            }

            var roomNoEnquiryModels = new RoomNoEnquiryModels()
            {
                DateList = dateList,
                RoomNoInfoList = rmNoInfoList.OrderBy(info => info.RoomNo),
                RoomType = rmTypCde,
                NightCount = defNightCnt,
                Day1 = rawList.Where(rl => rl.ThisDate.Date == dateList[0].Date).Count(rl => rl.GuestFullName.IsNullOrEmpty()),
                Day2 = rawList.Where(rl => rl.ThisDate.Date == dateList[1].Date).Count(rl => rl.GuestFullName.IsNullOrEmpty()),
                Day3 = rawList.Where(rl => rl.ThisDate.Date == dateList[2].Date).Count(rl => rl.GuestFullName.IsNullOrEmpty()),
                Day4 = rawList.Where(rl => rl.ThisDate.Date == dateList[3].Date).Count(rl => rl.GuestFullName.IsNullOrEmpty()),
                Day5 = rawList.Where(rl => rl.ThisDate.Date == dateList[4].Date).Count(rl => rl.GuestFullName.IsNullOrEmpty()),
                Day6 = rawList.Where(rl => rl.ThisDate.Date == dateList[5].Date).Count(rl => rl.GuestFullName.IsNullOrEmpty()),
                Day7 = rawList.Where(rl => rl.ThisDate.Date == dateList[6].Date).Count(rl => rl.GuestFullName.IsNullOrEmpty())
            };

            return View(roomNoEnquiryModels);

        }

        #endregion


        #region // javascript functions //

        [HttpPost]
        public IActionResult GetRoomDetails(string roomNo)
        {
            return new JsonResult(new { redirectTo = Url.Action("Edit", "WalkInGuest", new { roomNo }) });
        }

        public bool CheckRoomIsAvailable(string roomNo)
        {
            var hotelDte = GetHotelDate();
            var cmpyId = GetCmpyId();

            var available = _context.MsHotelRooms.Any(hr => hr.Roomno == roomNo && hr.Occuflg == false && hr.Cmpyid == cmpyId);

            return available;

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
