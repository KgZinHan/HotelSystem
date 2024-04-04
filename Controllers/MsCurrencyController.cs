using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsCurrencyController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MsCurrencyController(HotelCoreMvcContext context)
        {
            _context = context;

        }

        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsCurrencies != null ?
                        View(await _context.MsCurrencies.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsCurrencies'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();

            if (id == null || _context.MsCurrencies == null)
            {
                return NotFound();
            }

            var msCurrencyCode = await _context.MsCurrencies
                .FirstOrDefaultAsync(m => m.Currid == id);
            if (msCurrencyCode == null)
            {
                return NotFound();
            }

            return View(msCurrencyCode);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Currid,Currtyp,Currcde,Currrate")] MsCurrency msCurrency)
        {
            SetLayOutData();

            if (ModelState.IsValid)
            {
                msCurrency.Userid = GetUserId();
                msCurrency.Revdtetime = DateTime.Now;
                _context.Add(msCurrency);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msCurrency);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();

            if (id == null || _context.MsCurrencies == null)
            {
                return NotFound();
            }

            var msCurrencyCode = await _context.MsCurrencies.FindAsync(id);
            if (msCurrencyCode == null)
            {
                return NotFound();
            }
            return View(msCurrencyCode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Currid,Currtyp,Currcde,Currrate")] MsCurrency msCurrency)
        {
            SetLayOutData();

            if (id != msCurrency.Currid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msCurrency.Userid = GetUserId();
                    msCurrency.Revdtetime = DateTime.Now;
                    _context.Update(msCurrency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsCurrencyCodeExists(msCurrency.Currid))
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
            return View(msCurrency);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();

            if (id == null || _context.MsCurrencies == null)
            {
                return NotFound();
            }

            var msCurrency = await _context.MsCurrencies
                .FirstOrDefaultAsync(m => m.Currid == id);
            if (msCurrency == null)
            {
                return NotFound();
            }

            return View(msCurrency);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();

            if (_context.MsCurrencies == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsCurrencies'  is null.");
            }
            var msCurrency = await _context.MsCurrencies.FindAsync(id);
            if (msCurrency != null)
            {
                _context.MsCurrencies.Remove(msCurrency);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsCurrencyCodeExists(int id)
        {
            return (_context.MsCurrencies?.Any(e => e.Currid == id)).GetValueOrDefault();
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
