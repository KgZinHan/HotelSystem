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
    public class MsHotelRoomTypesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelRoomTypesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        // GET: MsHotelRoomTypes
        public async Task<IActionResult> Index()
        {
              return _context.MsHotelRoomTypes != null ? 
                          View(await _context.MsHotelRoomTypes.ToListAsync()) :
                          Problem("Entity set 'HotelCoreMvcContext.MsHotelRoomTypes'  is null.");
        }

        // GET: MsHotelRoomTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MsHotelRoomTypes == null)
            {
                return NotFound();
            }

            var msHotelRoomType = await _context.MsHotelRoomTypes
                .FirstOrDefaultAsync(m => m.Rmtypid == id);
            if (msHotelRoomType == null)
            {
                return NotFound();
            }

            return View(msHotelRoomType);
        }

        // GET: MsHotelRoomTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MsHotelRoomTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rmtypid,Rmtypcde,Rmtypdesc,Paxno,Extrabedprice,Cmpyid,Revdtetime,Userid")] MsHotelRoomType msHotelRoomType)
        {
            if (ModelState.IsValid)
            {
                if (!DuplicateDataExists(msHotelRoomType.Rmtypcde, msHotelRoomType.Rmtypid))
                {
                    msHotelRoomType.Cmpyid = funcs.currentCompanyID();
                    msHotelRoomType.Revdtetime = funcs.CurrentDatetime();
                    msHotelRoomType.Userid = funcs.currentUserID();
                    _context.Add(msHotelRoomType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Msg = CommonStrings.duplicateDataMessage;
            }
            return View(msHotelRoomType);
        }

        // GET: MsHotelRoomTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MsHotelRoomTypes == null)
            {
                return NotFound();
            }

            var msHotelRoomType = await _context.MsHotelRoomTypes.FindAsync(id);
            if (msHotelRoomType == null)
            {
                return NotFound();
            }
            return View(msHotelRoomType);
        }

            return View(hotelRoomType);
        }

        // POST: MsHotelRoomTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rmtypid,Rmtypcde,Rmtypdesc,Paxno,Extrabedprice,Cmpyid,Revdtetime,Userid")] MsHotelRoomType msHotelRoomType)
        {
            if (id != msHotelRoomType.Rmtypid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!DuplicateDataExists(msHotelRoomType.Rmtypcde, msHotelRoomType.Rmtypid))
                    {
                        msHotelRoomType.Cmpyid = funcs.currentCompanyID();
                        msHotelRoomType.Revdtetime = funcs.CurrentDatetime();
                        msHotelRoomType.Userid = funcs.currentUserID();

                        _context.Update(msHotelRoomType);
                        await _context.SaveChangesAsync();
                    }
                    ViewBag.Msg = CommonStrings.duplicateDataMessage;
                    return View(msHotelRoomType);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotelRoomTypeExists(msHotelRoomType.Rmtypid))
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
            return View(msHotelRoomType);
        }

        // GET: MsHotelRoomTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MsHotelRoomTypes == null)
            {
                return NotFound();
            }

            var msHotelRoomType = await _context.MsHotelRoomTypes
                .FirstOrDefaultAsync(m => m.Rmtypid == id);
            if (msHotelRoomType == null)
            {
                return NotFound();
            }

            return View(msHotelRoomType);
        }

        // POST: MsHotelRoomTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MsHotelRoomTypes == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelRoomTypes'  is null.");
            }
            var msHotelRoomType = await _context.MsHotelRoomTypes.FindAsync(id);
            if (msHotelRoomType != null)
            {
                _context.MsHotelRoomTypes.Remove(msHotelRoomType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelRoomTypeExists(int id)
        {
          return (_context.MsHotelRoomTypes?.Any(e => e.Rmtypid == id)).GetValueOrDefault();
        }
        private bool DuplicateDataExists(string code,int? id)
        {
            return (_context.MsHotelRoomTypes?.Any(e => e.Rmtypcde == code && (id==null || e.Rmtypid!=id))).GetValueOrDefault();
        }
    }
}
