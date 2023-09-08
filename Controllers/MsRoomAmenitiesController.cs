using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Core_MVC_V1.Models;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    public class MsRoomAmenitiesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsRoomAmenitiesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        // GET: MsRoomAmenities
        public async Task<IActionResult> Index()
        {
              return _context.MsRoomAmenities != null ? 
                          View(await _context.MsRoomAmenities.ToListAsync()) :
                          Problem("Entity set 'HotelCoreMvcContext.MsRoomAmenities'  is null.");
        }

        // GET: MsRoomAmenities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MsRoomAmenities == null)
            {
                return NotFound();
            }

            var msRoomAmenity = await _context.MsRoomAmenities
                .FirstOrDefaultAsync(m => m.Rmamtyid == id);
            if (msRoomAmenity == null)
            {
                return NotFound();
            }

            return View(msRoomAmenity);
        }

        // GET: MsRoomAmenities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MsRoomAmenities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rmamtyid,Rmamtycde,Rmamtydesc,Revdtetime,Userid")] MsRoomAmenity msRoomAmenity)
        {
            if (ModelState.IsValid)
            {
                msRoomAmenity.Revdtetime = funcs.CurrentDatetime();
                msRoomAmenity.Userid = funcs.currentUserID();
                _context.Add(msRoomAmenity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msRoomAmenity);
        }

        // GET: MsRoomAmenities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MsRoomAmenities == null)
            {
                return NotFound();
            }

            var msRoomAmenity = await _context.MsRoomAmenities.FindAsync(id);
            if (msRoomAmenity == null)
            {
                return NotFound();
            }
            return View(msRoomAmenity);
        }

        // POST: MsRoomAmenities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rmamtyid,Rmamtycde,Rmamtydesc,Revdtetime,Userid")] MsRoomAmenity msRoomAmenity)
        {
            if (id != msRoomAmenity.Rmamtyid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msRoomAmenity.Revdtetime = funcs.CurrentDatetime();
                    msRoomAmenity.Userid = funcs.currentUserID();
                    _context.Update(msRoomAmenity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsRoomAmenityExists(msRoomAmenity.Rmamtyid))
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
            return View(msRoomAmenity);
        }

        // GET: MsRoomAmenities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MsRoomAmenities == null)
            {
                return NotFound();
            }

            var msRoomAmenity = await _context.MsRoomAmenities
                .FirstOrDefaultAsync(m => m.Rmamtyid == id);
            if (msRoomAmenity == null)
            {
                return NotFound();
            }

            return View(msRoomAmenity);
        }

        // POST: MsRoomAmenities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MsRoomAmenities == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsRoomAmenities'  is null.");
            }
            var msRoomAmenity = await _context.MsRoomAmenities.FindAsync(id);
            if (msRoomAmenity != null)
            {
                _context.MsRoomAmenities.Remove(msRoomAmenity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsRoomAmenityExists(int id)
        {
          return (_context.MsRoomAmenities?.Any(e => e.Rmamtyid == id)).GetValueOrDefault();
        }
    }
}
