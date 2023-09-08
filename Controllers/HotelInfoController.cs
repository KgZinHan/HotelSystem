using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Core_MVC_V1.Models;

namespace Hotel_Core_MVC_V1.Controllers
{
    public class MsHotelRoomsController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MsHotelRoomsController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        // GET: MsHotelRooms
        public async Task<IActionResult> Index()
        {
            var hotelCoreMvcContext = _context.MsHotelRooms.Include(m => m.Bed).Include(m => m.Loc).Include(m => m.Rmamty).Include(m => m.Rmfeature).Include(m => m.Rmtyp);
            return View(await hotelCoreMvcContext.ToListAsync());
        }

        // GET: MsHotelRooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MsHotelRooms == null)
            {
                return NotFound();
            }

            var msHotelRoom = await _context.MsHotelRooms
                .Include(m => m.Bed)
                .Include(m => m.Loc)
                .Include(m => m.Rmamty)
                .Include(m => m.Rmfeature)
                .Include(m => m.Rmtyp)
                .FirstOrDefaultAsync(m => m.Roomid == id);
            if (msHotelRoom == null)
            {
                return NotFound();
            }

            return View(msHotelRoom);
        }

        // GET: MsHotelRooms/Create
        public IActionResult Create()
        {
            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Beddesc");
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Locdesc");
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtyid");
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeatureid");
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypid");
            return View();
        }

        // POST: MsHotelRooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Roomid,Roomno,Locid,Rmtypid,Bedid,Rmamtyid,Rmfeatureid,Isautoapplyrate,Usefixprice,Paxno,Roomtelno,Isguestin,Guestactivemsg,Isdnd,Cmpyid,Revdtetime,Userid")] MsHotelRoom msHotelRoom)
        {
            _context.Add(msHotelRoom);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            if (ModelState.IsValid)
            {

            }
            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedid", msHotelRoom.Bedid);
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Locid", msHotelRoom.Locid);
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtyid", msHotelRoom.Rmamtyid);
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeatureid", msHotelRoom.Rmfeatureid);
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypid", msHotelRoom.Rmtypid);
            return View(msHotelRoom);
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
            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedid", msHotelRoom.Bedid);
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Locid", msHotelRoom.Locid);
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtyid", msHotelRoom.Rmamtyid);
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeatureid", msHotelRoom.Rmfeatureid);
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypid", msHotelRoom.Rmtypid);
            return View(msHotelRoom);
        }

        // POST: MsHotelRooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Roomid,Roomno,Locid,Rmtypid,Bedid,Rmamtyid,Rmfeatureid,Isautoapplyrate,Usefixprice,Paxno,Roomtelno,Isguestin,Guestactivemsg,Isdnd,Cmpyid,Revdtetime,Userid")] MsHotelRoom msHotelRoom)
        {
            if (id != msHotelRoom.Roomid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(msHotelRoom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotelRoomExists(msHotelRoom.Roomid))
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
            ViewData["Bedid"] = new SelectList(_context.MsHotelroombeddings, "Bedid", "Bedid", msHotelRoom.Bedid);
            ViewData["Locid"] = new SelectList(_context.MsHotellocations, "Locid", "Locid", msHotelRoom.Locid);
            ViewData["Rmamtyid"] = new SelectList(_context.MsRoomAmenities, "Rmamtyid", "Rmamtyid", msHotelRoom.Rmamtyid);
            ViewData["Rmfeatureid"] = new SelectList(_context.MsRoomFeatures, "Rmfeatureid", "Rmfeatureid", msHotelRoom.Rmfeatureid);
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypid", msHotelRoom.Rmtypid);
            return View(msHotelRoom);
        }

        // GET: MsHotelRooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MsHotelRooms == null)
            {
                return NotFound();
            }

            var msHotelRoom = await _context.MsHotelRooms
                .Include(m => m.Bed)
                .Include(m => m.Loc)
                .Include(m => m.Rmamty)
                .Include(m => m.Rmfeature)
                .Include(m => m.Rmtyp)
                .FirstOrDefaultAsync(m => m.Roomid == id);
            if (msHotelRoom == null)
            {
                return NotFound();
            }

            return View(msHotelRoom);
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
