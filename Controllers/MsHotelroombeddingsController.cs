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
    public class MsHotelroombeddingsController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelroombeddingsController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        // GET: MsHotelroombeddings
        public async Task<IActionResult> Index()
        {
              return _context.MsHotelroombeddings != null ? 
                          View(await _context.MsHotelroombeddings.ToListAsync()) :
                          Problem("Entity set 'HotelCoreMvcContext.MsHotelroombeddings'  is null.");
        }

        // GET: MsHotelroombeddings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MsHotelroombeddings == null)
            {
                return NotFound();
            }

            var msHotelroombedding = await _context.MsHotelroombeddings
                .FirstOrDefaultAsync(m => m.Bedid == id);
            if (msHotelroombedding == null)
            {
                return NotFound();
            }

            return View(msHotelroombedding);
        }

        // GET: MsHotelroombeddings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MsHotelroombeddings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Bedid,Bedcde,Beddesc,Cmpyid,Revdtetime,Userid")] MsHotelroombedding msHotelroombedding)
        {
            if (ModelState.IsValid)
            {
                msHotelroombedding.Cmpyid = funcs.currentCompanyID();
                msHotelroombedding.Revdtetime = funcs.CurrentDatetime();
                msHotelroombedding.Userid = funcs.currentUserID();
                _context.Add(msHotelroombedding);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msHotelroombedding);
        }

        // GET: MsHotelroombeddings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MsHotelroombeddings == null)
            {
                return NotFound();
            }

            var msHotelroombedding = await _context.MsHotelroombeddings.FindAsync(id);
            if (msHotelroombedding == null)
            {
                return NotFound();
            }
            return View(msHotelroombedding);
        }

        // POST: MsHotelroombeddings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Bedid,Bedcde,Beddesc,Cmpyid,Revdtetime,Userid")] MsHotelroombedding msHotelroombedding)
        {
            if (id != msHotelroombedding.Bedid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msHotelroombedding.Cmpyid = funcs.currentCompanyID();
                    msHotelroombedding.Revdtetime = funcs.CurrentDatetime();
                    msHotelroombedding.Userid = funcs.currentUserID();
                    _context.Update(msHotelroombedding);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotelroombeddingExists(msHotelroombedding.Bedid))
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
            return View(msHotelroombedding);
        }

        // GET: MsHotelroombeddings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MsHotelroombeddings == null)
            {
                return NotFound();
            }

            var msHotelroombedding = await _context.MsHotelroombeddings
                .FirstOrDefaultAsync(m => m.Bedid == id);
            if (msHotelroombedding == null)
            {
                return NotFound();
            }

            return View(msHotelroombedding);
        }

        // POST: MsHotelroombeddings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MsHotelroombeddings == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelroombeddings'  is null.");
            }
            var msHotelroombedding = await _context.MsHotelroombeddings.FindAsync(id);
            if (msHotelroombedding != null)
            {
                _context.MsHotelroombeddings.Remove(msHotelroombedding);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelroombeddingExists(int id)
        {
          return (_context.MsHotelroombeddings?.Any(e => e.Bedid == id)).GetValueOrDefault();
        }
    }
}
