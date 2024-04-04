using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsGuestnationalitiesController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MsGuestnationalitiesController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsGuestnationalities != null ?
                        View(await _context.MsGuestnationalities.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsGuestnationalities'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGuestnationalities == null)
            {
                return NotFound();
            }

            var msGuestnationality = await _context.MsGuestnationalities
                .FirstOrDefaultAsync(m => m.Nationid == id);
            if (msGuestnationality == null)
            {
                return NotFound();
            }

            return View(msGuestnationality);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nationid,Nationdesc")] MsGuestnationality msGuestnationality)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                msGuestnationality.Userid = GetUserId();
                msGuestnationality.Revdtetime = DateTime.Now;
                _context.Add(msGuestnationality);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msGuestnationality);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGuestnationalities == null)
            {
                return NotFound();
            }

            var msGuestnationality = await _context.MsGuestnationalities.FindAsync(id);
            if (msGuestnationality == null)
            {
                return NotFound();
            }
            return View(msGuestnationality);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Nationid,Nationdesc")] MsGuestnationality msGuestnationality)
        {
            SetLayOutData();
            if (id != msGuestnationality.Nationid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msGuestnationality.Userid = GetUserId();
                    msGuestnationality.Revdtetime = DateTime.Now;
                    _context.Update(msGuestnationality);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsGuestnationalityExists(msGuestnationality.Nationid))
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
            return View(msGuestnationality);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGuestnationalities == null)
            {
                return NotFound();
            }

            var msGuestnationality = await _context.MsGuestnationalities
                .FirstOrDefaultAsync(m => m.Nationid == id);
            if (msGuestnationality == null)
            {
                return NotFound();
            }

            return View(msGuestnationality);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
            if (_context.MsGuestnationalities == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsGuestnationalities'  is null.");
            }
            var msGuestnationality = await _context.MsGuestnationalities.FindAsync(id);
            if (msGuestnationality != null)
            {
                _context.MsGuestnationalities.Remove(msGuestnationality);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsGuestnationalityExists(int id)
        {
            return (_context.MsGuestnationalities?.Any(e => e.Nationid == id)).GetValueOrDefault();
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
