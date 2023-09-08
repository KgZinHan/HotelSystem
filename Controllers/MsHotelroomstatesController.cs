using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Core_MVC_V1.Models;
using Hotel_Core_MVC_V1.Common;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    public class MsHotelroomstatesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelroomstatesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        // GET: MsHotelroomstates
        public async Task<IActionResult> Index()
        {
              return _context.MsHotelroomstates != null ? 
                          View(await _context.MsHotelroomstates.ToListAsync()) :
                          Problem("Entity set 'HotelCoreMvcContext.MsHotelroomstates'  is null.");
        }

        // GET: MsHotelroomstates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MsHotelroomstates == null)
            {
                return NotFound();
            }

            var msHotelroomstate = await _context.MsHotelroomstates
                .FirstOrDefaultAsync(m => m.Rmstateid == id);
            if (msHotelroomstate == null)
            {
                return NotFound();
            }

            return View(msHotelroomstate);
        }

        // GET: MsHotelroomstates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MsHotelroomstates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rmstateid,Rmstatecde,Rmstatedesc,Syscolor")] MsHotelroomstate msHotelroomstate)
        {
            if (ModelState.IsValid)
            {
                msHotelroomstate.Cmpyid = funcs.currentCompanyID();
                msHotelroomstate.Revdtetime= funcs.CurrentDatetime();
                msHotelroomstate.Userid = funcs.currentUserID();
                _context.Add(msHotelroomstate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msHotelroomstate);
        }

        // GET: MsHotelroomstates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MsHotelroomstates == null)
            {
                return NotFound();
            }

            var msHotelroomstate = await _context.MsHotelroomstates.FindAsync(id);
            if (msHotelroomstate == null)
            {
                return NotFound();
            }
            return View(msHotelroomstate);
        }

        // POST: MsHotelroomstates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rmstateid,Rmstatecde,Rmstatedesc,Syscolor,Cmpyid,Revdtetime,Userid")] MsHotelroomstate msHotelroomstate)
        {
            if (id != msHotelroomstate.Rmstateid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msHotelroomstate.Cmpyid = funcs.currentCompanyID();
                    msHotelroomstate.Revdtetime = funcs.CurrentDatetime();
                    msHotelroomstate.Userid = funcs.currentUserID();
                    _context.Update(msHotelroomstate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotelroomstateExists(msHotelroomstate.Rmstateid))
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
            return View(msHotelroomstate);
        }

        // GET: MsHotelroomstates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MsHotelroomstates == null)
            {
                return NotFound();
            }

            var msHotelroomstate = await _context.MsHotelroomstates
                .FirstOrDefaultAsync(m => m.Rmstateid == id);
            if (msHotelroomstate == null)
            {
                return NotFound();
            }

            return View(msHotelroomstate);
        }

        // POST: MsHotelroomstates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MsHotelroomstates == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelroomstates'  is null.");
            }
            var msHotelroomstate = await _context.MsHotelroomstates.FindAsync(id);
            if (msHotelroomstate != null)
            {
                _context.MsHotelroomstates.Remove(msHotelroomstate);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelroomstateExists(int id)
        {
          return (_context.MsHotelroomstates?.Any(e => e.Rmstateid == id)).GetValueOrDefault();
        }
    }
}
