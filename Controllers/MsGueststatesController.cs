using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsGueststatesController : Controller
    {
        private readonly HotelCoreMvcContext _context;


        #region // Main methods //
        public MsGueststatesController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            var guestStateModel = await _context.MsGueststates.
                Select(msUsers => new GuestStateModel()
                {
                    Gstateid = msUsers.Gstateid,
                    Gstatedesc = msUsers.Gstatedesc,
                    Country = _context.MsGuestcountries.Where(c => c.Countryid == msUsers.Countryid).Select(c => c.Countrydesc).First(),
                })
                .ToListAsync();

            return View(guestStateModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGueststates == null)
            {
                return NotFound();
            }

            var msGueststate = await _context.MsGueststates
                .FirstOrDefaultAsync(m => m.Gstateid == id);
            if (msGueststate == null)
            {
                return NotFound();
            }

            return View(msGueststate);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            ViewBag.CountryId = new SelectList(_context.MsGuestcountries, "Countryid", "Countrydesc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Gstateid,Gstatedesc,Countryid")] MsGueststate msGueststate)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                msGueststate.Userid = GetUserId();
                msGueststate.Revdtetime = DateTime.Now;
                _context.Add(msGueststate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msGueststate);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGueststates == null)
            {
                return NotFound();
            }

            var msGueststate = await _context.MsGueststates.FindAsync(id);
            if (msGueststate == null)
            {
                return NotFound();
            }

            ViewBag.CountryId = new SelectList(_context.MsGuestcountries, "Countryid", "Countrydesc");
            return View(msGueststate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Gstateid,Gstatedesc,Countryid")] MsGueststate msGueststate)
        {
            SetLayOutData();
            if (id != msGueststate.Gstateid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msGueststate.Userid = GetUserId();
                    msGueststate.Revdtetime = DateTime.Now;
                    _context.Update(msGueststate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsGueststateExists(msGueststate.Gstateid))
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
            return View(msGueststate);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGueststates == null)
            {
                return NotFound();
            }

            var msGueststate = await _context.MsGueststates
                .FirstOrDefaultAsync(m => m.Gstateid == id);
            if (msGueststate == null)
            {
                return NotFound();
            }

            return View(msGueststate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
            if (_context.MsGueststates == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsGueststates'  is null.");
            }
            var msGueststate = await _context.MsGueststates.FindAsync(id);
            if (msGueststate != null)
            {
                _context.MsGueststates.Remove(msGueststate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsGueststateExists(int id)
        {
            return (_context.MsGueststates?.Any(e => e.Gstateid == id)).GetValueOrDefault();
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
