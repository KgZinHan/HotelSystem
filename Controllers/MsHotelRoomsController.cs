using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Core_MVC_V1.Models;
using static Hotel_Core_MVC_V1.Common.CommonItems;
using Microsoft.CodeAnalysis;

namespace Hotel_Core_MVC_V1.Controllers
{
    public class MsHotelRoomsController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelRoomsController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        // GET: MsHotelRooms
        public async Task<IActionResult> Index()
        {
            //var list = _context.MsHotelRooms.Select(x=>new HotelRoomModel
            //{
            //    Roomid = x.Roomid,
            //    Bedding=_context.MsHotelroombeddings.Where(j=>j.Bedid==x.Bedid).Select(j=>j.Bedcde).FirstOrDefault(),
            //    Location=_context.MsHotellocations.Where(j => j.Locid == x.Locid).Select(j => j.Loccde).FirstOrDefault(),
            //    RoomAmenities=string.Join("", _context.LinkHotelRoomRoomAmenities.Join(_context.MsRoomAmenities, l => l.Rmamtyid, rm => rm.Rmamtyid, (l, rm) => new { l, rm }).AsEnumerable().Where(j => j.l.Roomid == x.Roomid).Select(j => j.rm.Rmamtycde).ToArray()),
            //    RoomFeatures =string.Join("", _context.LinkHotelRoomRoomFeatures.Join(_context.MsRoomFeatures, l => l.Rmfeatureid, rf => rf.Rmfeatureid, (l, rf) => new {l,rf}).AsEnumerable().Where(j=>j.l.Roomid==x.Roomid).Select(j=>j.rf.Rmfeaturecde).ToArray()),
            //    RoomType= _context.MsHotelRoomTypes.Where(j => j.Rmtypid == x.Rmtypid).Select(j => j.Rmtypcde).FirstOrDefault(),
            //    Guestactivemsg =x.Guestactivemsg,
            //    Isautoapplyrate=x.Isautoapplyrate,
            //    Isdnd=x.Isdnd,
            //    Isguestin = x.Isguestin,
            //    Paxno = x.Paxno,
            //    Roomno=x.Roomno,
            //    Roomtelno=x.Roomtelno,
            //    Usefixprice=x.Usefixprice

            //}).ToListAsync(); 
            var rawList =  _context.HotelRoomModelDBSet.FromSqlRaw("EXEC GetHotelRooms").AsEnumerable().Select(x=> new HotelRoomModel
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
            //return _context.MsHotelRooms != null ?
            //            View(rawList) :
            //            Problem("Entity set 'HotelCoreMvcContext.MsHotelRooms'  is null.");
        }

        // GET: MsHotelRooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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

        // GET: MsHotelRooms/Create
        public IActionResult Create()
        {
            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedcde");
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Loccde");
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtycde");
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeaturecde");
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");

            return View();
        }

        // POST: MsHotelRooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Roomid,Roomno,Locid,Rmtypid,Bedid,Rmamtyid,Rmfeatureid,Isautoapplyrate,Usefixprice,Paxno,Roomtelno,Isguestin,Guestactivemsg,Isdnd,Cmpyid,Revdtetime,Userid")] HotelRoomModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (ModelState.IsValid)
                {
                    MsHotelRoom msHotelRoom = new MsHotelRoom();

                    msHotelRoom.Cmpyid = funcs.currentCompanyID();
                    msHotelRoom.Revdtetime = funcs.CurrentDatetime();
                    msHotelRoom.Userid = funcs.currentUserID();
                    msHotelRoom.Locid = model.Locid;
                    msHotelRoom.Isautoapplyrate=model.Isautoapplyrate;
                    msHotelRoom.Usefixprice = model.Usefixprice;
                    msHotelRoom.Bedid=model.Bedid;
                    msHotelRoom.Guestactivemsg = model.Guestactivemsg;
                    msHotelRoom.Isdnd = model.Isdnd;
                    msHotelRoom.Isguestin=model.Isguestin;
                    msHotelRoom.Paxno=model.Paxno;
                    msHotelRoom.Rmtypid = model.Rmtypid;
                    msHotelRoom.Roomno= model.Roomno;
                    msHotelRoom.Roomtelno= model.Roomtelno;

                    _context.Add(msHotelRoom);
                    await _context.SaveChangesAsync();

                    if (model.Rmfeatureid != null)
                    {
                        foreach (var i in model.Rmfeatureid)
                        {
                            LinkHotelRoomRoomFeature link = new LinkHotelRoomRoomFeature();
                            link.Rmfeatureid = i;
                            link.Roomid = msHotelRoom.Roomid;
                            _context.Add(link);
                        }
                    }
                    if (model.Rmamtyid != null)
                    {
                        foreach (var i in model.Rmamtyid)
                        {
                            LinkHotelRoomRoomAmenity link = new LinkHotelRoomRoomAmenity();
                            link.Rmamtyid = i;
                            link.Roomid = msHotelRoom.Roomid;
                            _context.Add(link);
                        }
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedcde");
                ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Loccde");
                ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtycde");
                ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeaturecde");
                ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");

                return View(model);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        // GET: MsHotelRooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MsHotelRooms == null)
            {
                return NotFound();
            }

            var msHotelRoom = await _context.MsHotelRooms.FindAsync(id);
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

            model.Rmamtyid = _context.LinkHotelRoomRoomAmenities.Where(x=>x.Roomid== msHotelRoom.Roomid).Select(x=>x.Rmamtyid).ToList();
            model.Rmfeatureid = _context.LinkHotelRoomRoomFeatures.Where(x => x.Roomid == msHotelRoom.Roomid).Select(x => x.Rmfeatureid).ToList();

            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedcde");
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Loccde");
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtycde");
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeaturecde");
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");

            return View(model);
        }

        // POST: MsHotelRooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Roomid,Roomno,Locid,Rmtypid,Bedid,Rmamtyid,Rmfeatureid,Isautoapplyrate,Usefixprice,Paxno,Roomtelno,Isguestin,Guestactivemsg,Isdnd,Cmpyid,Revdtetime,Userid")] HotelRoomModel model)
        {
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
                        MsHotelRoom msHotelRoom = new MsHotelRoom();
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
                                LinkHotelRoomRoomFeature link = new LinkHotelRoomRoomFeature();
                                link.Rmfeatureid = i;
                                link.Roomid = msHotelRoom.Roomid;
                                _context.Add(link);
                            }
                        }
                        if (model.Rmamtyid != null)
                        {
                            foreach (var i in model.Rmamtyid)
                            {
                                LinkHotelRoomRoomAmenity link = new LinkHotelRoomRoomAmenity();
                                link.Rmamtyid = i;
                                link.Roomid = msHotelRoom.Roomid;
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
            catch (Exception ex)
            {
                transaction.Rollback();
            }

            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedcde");
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Loccde");
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtycde");
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeaturecde");
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");

            return View(model);
        }

        // GET: MsHotelRooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
            model.bedcde= _context.MsHotelroombeddings.Where(x => x.Bedid == msHotelRoom.Bedid).Select(x => x.Bedcde).FirstOrDefault();
            model.Guestactivemsg = msHotelRoom.Guestactivemsg;
            model.Isdnd = msHotelRoom.Isdnd;
            model.Isguestin = msHotelRoom.Isguestin;
            model.Paxno = msHotelRoom.Paxno;
            model.Rmtypid = msHotelRoom.Rmtypid;
            model.rmtypcde= _context.MsHotelRoomTypes.Where(x => x.Rmtypid == msHotelRoom.Rmtypid).Select(x => x.Rmtypcde).FirstOrDefault();
            model.Roomno = msHotelRoom.Roomno;
            model.Roomtelno = msHotelRoom.Roomtelno;

            model.RoomAmenities = String.Join(",", _context.LinkHotelRoomRoomAmenities.Join(_context.MsRoomAmenities,l=>l.Rmamtyid, rm=>rm.Rmamtyid,(l,rm)=>new { l, rm }).Where(x => x.l.Roomid == msHotelRoom.Roomid).Select(x => x.rm.Rmamtycde).ToList());
            model.RoomFeatures = String.Join(",", _context.LinkHotelRoomRoomFeatures.Join(_context.MsRoomFeatures, l => l.Rmfeatureid, rf => rf.Rmfeatureid, (l, rf) => new {l,rf}).Where(x => x.l.Roomid == msHotelRoom.Roomid).Select(x => x.rf.Rmfeaturecde).ToList());

            return View(model);
        }

        // POST: MsHotelRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
    }
}
