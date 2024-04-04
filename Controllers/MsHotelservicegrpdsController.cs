using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsHotelservicegrpdsController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MsHotelservicegrpdsController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //
        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsHotelservicegrpds != null ?
                          View(await _context.MsHotelservicegrpds.ToListAsync()) :
                          Problem("Entity set 'HotelCoreMvcContext.MsHotelservicegrpds'  is null.");
        }

        public async Task<IActionResult> Details(string id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelservicegrpds == null)
            {
                return NotFound();
            }

            var msHotelservicegrpd = await _context.MsHotelservicegrpds
                .FirstOrDefaultAsync(m => m.Srvccde == id);
            if (msHotelservicegrpd == null)
            {
                return NotFound();
            }

            return View(msHotelservicegrpd);
        }


        public IActionResult Create()
        {
            SetLayOutData();
            ViewBag.DeptcdeList = new SelectList(_context.MsDepartments, "Deptcde", "Deptcde");
            ViewBag.SrvcgrpcdeList = new SelectList(_context.MsHotelservicegrps, "Srvcgrpcde", "Srvcgrpcde");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Srvccde,Srvcdesc,Deptcde,Srvcgrpcde,Trantyp,Srvcamt,Srvcaccde,Sysdefine")] MsHotelservicegrpd msHotelservicegrpd)
        {
            if (ModelState.IsValid)
            {
                msHotelservicegrpd.Cmpyid = GetCmpyId();
                msHotelservicegrpd.Userid = GetUserId();
                msHotelservicegrpd.Revdtetime = DateTime.Now;
                _context.Add(msHotelservicegrpd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DeptcdeList = new SelectList(_context.MsDepartments, "Deptcde", "Deptcde");
            ViewBag.SrvcgrpcdeList = new SelectList(_context.MsHotelservicegrps, "Srvcgrpcde", "Srvcgrpcde");
            return View(msHotelservicegrpd);
        }

        public async Task<IActionResult> Edit(string id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelservicegrpds == null)
            {
                return NotFound();
            }

            var msHotelservicegrpd = await _context.MsHotelservicegrpds.FindAsync(id);
            if (msHotelservicegrpd == null)
            {
                return NotFound();
            }

            ViewBag.DeptcdeList = new SelectList(_context.MsDepartments, "Deptcde", "Deptcde");
            ViewBag.SrvcgrpcdeList = new SelectList(_context.MsHotelservicegrps, "Srvcgrpcde", "Srvcgrpcde");
            return View(msHotelservicegrpd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Srvccde,Srvcdesc,Deptcde,Srvcgrpcde,Trantyp,Srvcamt,Srvcaccde,Sysdefine")] MsHotelservicegrpd msHotelservicegrpd)
        {
            if (id != msHotelservicegrpd.Srvccde)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msHotelservicegrpd.Cmpyid = GetCmpyId();
                    msHotelservicegrpd.Userid = GetUserId();
                    msHotelservicegrpd.Revdtetime = DateTime.Now;
                    _context.Update(msHotelservicegrpd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotelservicegrpdExists(msHotelservicegrpd.Srvccde))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                ViewBag.DeptcdeList = new SelectList(_context.MsDepartments, "Deptcde", "Deptcde");
                ViewBag.SrvcgrpcdeList = new SelectList(_context.MsHotelservicegrps, "Srvcgrpcde", "Srvcgrpcde");
                return RedirectToAction(nameof(Index));
            }
            return View(msHotelservicegrpd);
        }

        public async Task<IActionResult> Delete(string id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelservicegrpds == null)
            {
                return NotFound();
            }

            var msHotelservicegrpd = await _context.MsHotelservicegrpds
                .FirstOrDefaultAsync(m => m.Srvccde == id);
            if (msHotelservicegrpd == null)
            {
                return NotFound();
            }

            return View(msHotelservicegrpd);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.MsHotelservicegrpds == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelservicegrpds'  is null.");
            }
            var msHotelservicegrpd = await _context.MsHotelservicegrpds.FindAsync(id);
            if (msHotelservicegrpd != null)
            {
                _context.MsHotelservicegrpds.Remove(msHotelservicegrpd);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelservicegrpdExists(string id)
        {
            return (_context.MsHotelservicegrpds?.Any(e => e.Srvccde == id)).GetValueOrDefault();
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
