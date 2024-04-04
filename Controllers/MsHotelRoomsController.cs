using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsHotelRoomsController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelRoomsController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }


        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var rawList = _context.HotelRoomModelDBSet.FromSqlRaw("EXEC GetHotelRooms").AsEnumerable().Select(x => new HotelRoomModel
            {
                Roomid = x.Roomid,
                bedcde = x.bedcde,
                Loccde = x.Loccde,
                RoomAmenities = x.RoomAmenities,
                RoomFeatures = x.RoomFeatures,
                rmtypcde = x.rmtypcde,
                Guestactivemsg = x.Guestactivemsg,
                Isautoapplyrate = x.Isautoapplyrate,
                Isdnd = x.Isdnd,
                Isguestin = x.Isguestin,
                Paxno = x.Paxno,
                Roomno = x.Roomno,
                Roomtelno = x.Roomtelno,
                Usefixprice = x.Usefixprice
            }).ToList();
            return View(rawList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelRooms == null)
            {
                return NotFound();
            }

            var msHotelRoom = await _context.MsHotelRooms
                .FirstOrDefaultAsync(m => m.Roomid == id);
            if (msHotelRoom == null)
            {
                return NotFound();
            }
            HotelRoomModel model = new HotelRoomModel();
            model.Roomid = msHotelRoom.Roomid;
            model.Cmpyid = msHotelRoom.Cmpyid;
            model.Revdtetime = msHotelRoom.Revdtetime;
            model.Userid = msHotelRoom.Userid;
            model.Locid = msHotelRoom.Locid;
            model.Loccde = _context.MsHotellocations.Where(x => x.Locid == msHotelRoom.Locid).Select(x => x.Loccde).FirstOrDefault();
            model.Isautoapplyrate = msHotelRoom.Isautoapplyrate;
            model.Usefixprice = msHotelRoom.Usefixprice;
            model.Bedid = msHotelRoom.Bedid;
            model.bedcde = _context.MsHotelroombeddings.Where(x => x.Bedid == msHotelRoom.Bedid).Select(x => x.Bedcde).FirstOrDefault();
            model.Guestactivemsg = msHotelRoom.Guestactivemsg;
            model.Isdnd = msHotelRoom.Isdnd;
            model.Isguestin = msHotelRoom.Isguestin;
            model.Paxno = msHotelRoom.Paxno;
            model.Rmtypid = msHotelRoom.Rmtypid;
            model.rmtypcde = _context.MsHotelRoomTypes.Where(x => x.Rmtypid == msHotelRoom.Rmtypid).Select(x => x.Rmtypcde).FirstOrDefault();
            model.Roomno = msHotelRoom.Roomno;
            model.Roomtelno = msHotelRoom.Roomtelno;

            model.RoomAmenities = String.Join(",", _context.LinkHotelRoomRoomAmenities.Join(_context.MsRoomAmenities, l => l.Rmamtyid, rm => rm.Rmamtyid, (l, rm) => new { l, rm }).Where(x => x.l.Roomid == msHotelRoom.Roomid).Select(x => x.rm.Rmamtycde).ToList());
            model.RoomFeatures = String.Join(",", _context.LinkHotelRoomRoomFeatures.Join(_context.MsRoomFeatures, l => l.Rmfeatureid, rf => rf.Rmfeatureid, (l, rf) => new { l, rf }).Where(x => x.l.Roomid == msHotelRoom.Roomid).Select(x => x.rf.Rmfeaturecde).ToList());

            return View(model);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedcde");
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Loccde");
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtydesc");
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeaturedesc");
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Roomid,Roomno,Locid,Rmtypid,Bedid,Rmamtyid,Rmfeatureid,Isautoapplyrate,Usefixprice,Paxno,Roomtelno,Isguestin,Guestactivemsg,Isdnd,CurrShift,NoOfShift")] HotelRoomModel model)
        {
            SetLayOutData();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (ModelState.IsValid)
                {
                    var msHotelRoom = new MsHotelRoom();
                    msHotelRoom.Cmpyid = funcs.currentCompanyID();
                    msHotelRoom.Revdtetime = funcs.CurrentDatetime();
                    msHotelRoom.Userid = funcs.currentUserID();
                    msHotelRoom.Locid = model.Locid;
                    msHotelRoom.Isautoapplyrate = model.Isautoapplyrate;
                    msHotelRoom.Usefixprice = model.Usefixprice;
                    msHotelRoom.Bedid = model.Bedid;
                    msHotelRoom.Guestactivemsg = model.Guestactivemsg;
                    msHotelRoom.Isdnd = model.Isdnd;
                    msHotelRoom.Isguestin = model.Isguestin;
                    msHotelRoom.Paxno = model.Paxno;
                    msHotelRoom.Rmtypid = model.Rmtypid;
                    msHotelRoom.Roomno = model.Roomno;
                    msHotelRoom.Occuflg = false; // Default Value
                    msHotelRoom.Roomtelno = model.Roomtelno;

                    _context.Add(msHotelRoom);
                    await _context.SaveChangesAsync();

                    if (model.Rmfeatureid != null)
                    {
                        foreach (var i in model.Rmfeatureid)
                        {
                            var link = new LinkHotelRoomRoomFeature()
                            {
                                Rmfeatureid = i,
                                Roomid = msHotelRoom.Roomid
                            };
                            _context.Add(link);
                        }
                    }

                    if (model.Rmamtyid != null)
                    {
                        foreach (var i in model.Rmamtyid)
                        {
                            var link = new LinkHotelRoomRoomAmenity()
                            {
                                Rmamtyid = i,
                                Roomid = msHotelRoom.Roomid
                            };
                            _context.Add(link);
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedcde");
                ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Loccde");
                ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtydesc");
                ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeaturedesc");
                ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");

                return View(model);

            }
            catch
            {
                await transaction.RollbackAsync();
                ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedcde");
                ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Loccde");
                ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtydesc");
                ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeaturedesc");
                ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelRooms == null)
            {
                return NotFound();
            }

            var msHotelRoom = await _context.MsHotelRooms.FindAsync(id);
            if (msHotelRoom == null)
            {
                return NotFound();
            }
            var model = new HotelRoomModel();
            model.Roomid = msHotelRoom.Roomid;
            model.Cmpyid = msHotelRoom.Cmpyid;
            model.Revdtetime = msHotelRoom.Revdtetime;
            model.Userid = msHotelRoom.Userid;
            model.Locid = msHotelRoom.Locid;
            model.Isautoapplyrate = msHotelRoom.Isautoapplyrate;
            model.Usefixprice = msHotelRoom.Usefixprice;
            model.Bedid = msHotelRoom.Bedid;
            model.Guestactivemsg = msHotelRoom.Guestactivemsg;
            model.Isdnd = msHotelRoom.Isdnd;
            model.Isguestin = msHotelRoom.Isguestin;
            model.Paxno = msHotelRoom.Paxno;
            model.Rmtypid = msHotelRoom.Rmtypid;
            model.Roomno = msHotelRoom.Roomno;
            model.Roomtelno = msHotelRoom.Roomtelno;

            model.Rmamtyid = _context.LinkHotelRoomRoomAmenities.Where(x => x.Roomid == msHotelRoom.Roomid).Select(x => x.Rmamtyid).ToList();
            model.Rmfeatureid = _context.LinkHotelRoomRoomFeatures.Where(x => x.Roomid == msHotelRoom.Roomid).Select(x => x.Rmfeatureid).ToList();

            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedcde");
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Loccde");
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtydesc");
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeaturedesc");
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Roomid,Roomno,Locid,Rmtypid,Bedid,Rmamtyid,Rmfeatureid,Isautoapplyrate,Usefixprice,Paxno,Roomtelno,Isguestin,Guestactivemsg,Isdnd,Cmpyid,Revdtetime,Userid")] HotelRoomModel model)
        {
            SetLayOutData();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (id != model.Roomid)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var msHotelRoom = new MsHotelRoom();
                        msHotelRoom.Roomid = id;
                        msHotelRoom.Cmpyid = funcs.currentCompanyID();
                        msHotelRoom.Revdtetime = funcs.CurrentDatetime();
                        msHotelRoom.Userid = funcs.currentUserID();
                        msHotelRoom.Locid = model.Locid;
                        msHotelRoom.Isautoapplyrate = model.Isautoapplyrate;
                        msHotelRoom.Usefixprice = model.Usefixprice;
                        msHotelRoom.Bedid = model.Bedid;
                        msHotelRoom.Guestactivemsg = model.Guestactivemsg;
                        msHotelRoom.Isdnd = model.Isdnd;
                        msHotelRoom.Isguestin = model.Isguestin;
                        msHotelRoom.Paxno = model.Paxno;
                        msHotelRoom.Rmtypid = model.Rmtypid;
                        msHotelRoom.Roomno = model.Roomno;
                        msHotelRoom.Roomtelno = model.Roomtelno;
                        _context.Update(msHotelRoom);
                        await _context.SaveChangesAsync();

                        var delrm = _context.LinkHotelRoomRoomAmenities.Where(x => x.Roomid == model.Roomid).ToList();
                        _context.LinkHotelRoomRoomAmenities.RemoveRange(delrm);
                        var delrf = _context.LinkHotelRoomRoomFeatures.Where(x => x.Roomid == model.Roomid).ToList();
                        _context.LinkHotelRoomRoomFeatures.RemoveRange(delrf);

                        if (model.Rmfeatureid != null)
                        {
                            foreach (var i in model.Rmfeatureid)
                            {
                                var link = new LinkHotelRoomRoomFeature()
                                {
                                    Rmfeatureid = i,
                                    Roomid = msHotelRoom.Roomid
                                };
                                _context.Add(link);
                            }
                        }
                        if (model.Rmamtyid != null)
                        {
                            foreach (var i in model.Rmamtyid)
                            {
                                var link = new LinkHotelRoomRoomAmenity()
                                {
                                    Rmamtyid = i,
                                    Roomid = msHotelRoom.Roomid
                                };
                                _context.Add(link);
                            }
                        }
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MsHotelRoomExists(model.Roomid))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

            }
            catch
            {
                transaction.Rollback();
            }

            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedcde");
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Loccde");
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtydesc");
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeaturedesc");
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();

            if (id == null || _context.MsHotelRooms == null)
            {
                return NotFound();
            }

            var msHotelRoom = await _context.MsHotelRooms.FirstOrDefaultAsync(m => m.Roomid == id);

            if (msHotelRoom == null)
            {
                return NotFound();
            }

            var model = new HotelRoomModel()
            {
                Roomid = msHotelRoom.Roomid,
                Cmpyid = msHotelRoom.Cmpyid,
                Revdtetime = msHotelRoom.Revdtetime,
                Userid = msHotelRoom.Userid,
                Locid = msHotelRoom.Locid,
                Loccde = _context.MsHotellocations.FirstOrDefault(x => x.Locid == msHotelRoom.Locid)?.Loccde,
                Isautoapplyrate = msHotelRoom.Isautoapplyrate,
                Usefixprice = msHotelRoom.Usefixprice,
                Bedid = msHotelRoom.Bedid,
                bedcde = _context.MsHotelroombeddings.FirstOrDefault(x => x.Bedid == msHotelRoom.Bedid)?.Bedcde,
                Guestactivemsg = msHotelRoom.Guestactivemsg,
                Isdnd = msHotelRoom.Isdnd,
                Isguestin = msHotelRoom.Isguestin,
                Paxno = msHotelRoom.Paxno,
                Rmtypid = msHotelRoom.Rmtypid,
                rmtypcde = _context.MsHotelRoomTypes.FirstOrDefault(x => x.Rmtypid == msHotelRoom.Rmtypid)?.Rmtypcde,
                Roomno = msHotelRoom.Roomno,
                Roomtelno = msHotelRoom.Roomtelno,
                RoomAmenities = string.Join(",", _context.LinkHotelRoomRoomAmenities
                    .Join(_context.MsRoomAmenities, l => l.Rmamtyid, rm => rm.Rmamtyid, (l, rm) => new { l, rm })
                    .Where(x => x.l.Roomid == msHotelRoom.Roomid)
                    .Select(x => x.rm.Rmamtycde)
                    .ToList()),
                RoomFeatures = string.Join(",", _context.LinkHotelRoomRoomFeatures
                    .Join(_context.MsRoomFeatures, l => l.Rmfeatureid, rf => rf.Rmfeatureid, (l, rf) => new { l, rf })
                    .Where(x => x.l.Roomid == msHotelRoom.Roomid)
                    .Select(x => x.rf.Rmfeaturecde)
                    .ToList())
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
            if (_context.MsHotelRooms == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelRooms'  is null.");
            }
            var msHotelRoom = await _context.MsHotelRooms.FindAsync(id);
            if (msHotelRoom != null)
            {
                var delrm = _context.LinkHotelRoomRoomAmenities.Where(x => x.Roomid == id).ToList();
                _context.LinkHotelRoomRoomAmenities.RemoveRange(delrm);
                var delrf = _context.LinkHotelRoomRoomFeatures.Where(x => x.Roomid == id).ToList();
                _context.LinkHotelRoomRoomFeatures.RemoveRange(delrf);

                _context.MsHotelRooms.Remove(msHotelRoom);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelRoomExists(int id)
        {
            return (_context.MsHotelRooms?.Any(e => e.Roomid == id)).GetValueOrDefault();
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
