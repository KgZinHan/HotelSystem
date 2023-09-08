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
    public class MsHotellocationsController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotellocationsController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        // GET: MsHotellocations
        public async Task<IActionResult> Index()
        {
              return _context.MsHotellocations != null ? 
                          View(await _context.MsHotellocations.ToListAsync()) :
                          Problem("Entity set 'HotelCoreMvcContext.MsHotellocations'  is null.");
        }

        // GET: MsHotellocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MsHotellocations == null)
            {
                return NotFound();
            }

            var msHotellocation = await _context.MsHotellocations
                .FirstOrDefaultAsync(m => m.Locid == id);
            if (msHotellocation == null)
            {
                return NotFound();
            }

            return View(msHotellocation);
        }

        // GET: MsHotellocations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MsHotellocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Locid,Loccde,Locdesc,Cmpyid,Isoutlet,Revdtetime,Userid")] MsHotellocation msHotellocation)
        {
            if (ModelState.IsValid)
            {
                msHotellocation.Cmpyid = funcs.currentCompanyID();
                msHotellocation.Revdtetime = funcs.CurrentDatetime();
                msHotellocation.Userid = funcs.currentUserID();
                _context.Add(msHotellocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msHotellocation);
        }

        // GET: MsHotellocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MsHotellocations == null)
            {
                return NotFound();
            }

            var msHotellocation = await _context.MsHotellocations.FindAsync(id);
            if (msHotellocation == null)
            {
                return NotFound();
            }
            return View(msHotellocation);
        }

        // POST: MsHotellocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Locid,Loccde,Locdesc,Cmpyid,Isoutlet,Revdtetime,Userid")] MsHotellocation msHotellocation)
        {
            if (id != msHotellocation.Locid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msHotellocation.Cmpyid = funcs.currentCompanyID();
                    msHotellocation.Revdtetime = funcs.CurrentDatetime();
                    msHotellocation.Userid = funcs.currentUserID();
                    _context.Update(msHotellocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotellocationExists(msHotellocation.Locid))
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
            return View(msHotellocation);
        }

        // GET: MsHotellocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MsHotellocations == null)
            {
                return NotFound();
            }

            var msHotellocation = await _context.MsHotellocations
                .FirstOrDefaultAsync(m => m.Locid == id);
            if (msHotellocation == null)
            {
                return NotFound();
            }

            return View(msHotellocation);
        }

        // POST: MsHotellocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MsHotellocations == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotellocations'  is null.");
            }
            var msHotellocation = await _context.MsHotellocations.FindAsync(id);
            if (msHotellocation != null)
            {
                _context.MsHotellocations.Remove(msHotellocation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotellocationExists(int id)
        {
          return (_context.MsHotellocations?.Any(e => e.Locid == id)).GetValueOrDefault();
        }
    }
}
