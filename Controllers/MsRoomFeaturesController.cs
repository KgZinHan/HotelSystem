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
    public class MsRoomFeaturesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsRoomFeaturesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        // GET: MsRoomFeatures
        public async Task<IActionResult> Index()
        {
              return _context.MsRoomFeatures != null ? 
                          View(await _context.MsRoomFeatures.ToListAsync()) :
                          Problem("Entity set 'HotelCoreMvcContext.MsRoomFeatures'  is null.");
        }

        // GET: MsRoomFeatures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MsRoomFeatures == null)
            {
                return NotFound();
            }

            var msRoomFeature = await _context.MsRoomFeatures
                .FirstOrDefaultAsync(m => m.Rmfeatureid == id);
            if (msRoomFeature == null)
            {
                return NotFound();
            }

            return View(msRoomFeature);
        }

        // GET: MsRoomFeatures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MsRoomFeatures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rmfeatureid,Rmfeaturecde,Rmfeaturedesc,Revdtetime,Userid")] MsRoomFeature msRoomFeature)
        {
            if (ModelState.IsValid)
            {
                msRoomFeature.Revdtetime = funcs.CurrentDatetime();
                msRoomFeature.Userid = funcs.currentUserID();
                _context.Add(msRoomFeature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msRoomFeature);
        }

        // GET: MsRoomFeatures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MsRoomFeatures == null)
            {
                return NotFound();
            }

            var msRoomFeature = await _context.MsRoomFeatures.FindAsync(id);
            if (msRoomFeature == null)
            {
                return NotFound();
            }
            return View(msRoomFeature);
        }

        // POST: MsRoomFeatures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rmfeatureid,Rmfeaturecde,Rmfeaturedesc,Revdtetime,Userid")] MsRoomFeature msRoomFeature)
        {
            if (id != msRoomFeature.Rmfeatureid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msRoomFeature.Revdtetime = funcs.CurrentDatetime();
                    msRoomFeature.Userid = funcs.currentUserID();

                    _context.Update(msRoomFeature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsRoomFeatureExists(msRoomFeature.Rmfeatureid))
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
            return View(msRoomFeature);
        }

        // GET: MsRoomFeatures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MsRoomFeatures == null)
            {
                return NotFound();
            }

            var msRoomFeature = await _context.MsRoomFeatures
                .FirstOrDefaultAsync(m => m.Rmfeatureid == id);
            if (msRoomFeature == null)
            {
                return NotFound();
            }

            return View(msRoomFeature);
        }

        // POST: MsRoomFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MsRoomFeatures == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsRoomFeatures'  is null.");
            }
            var msRoomFeature = await _context.MsRoomFeatures.FindAsync(id);
            if (msRoomFeature != null)
            {
                _context.MsRoomFeatures.Remove(msRoomFeature);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsRoomFeatureExists(int id)
        {
          return (_context.MsRoomFeatures?.Any(e => e.Rmfeatureid == id)).GetValueOrDefault();
        }
    }
}
