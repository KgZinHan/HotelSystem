using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsHotelservicegrpsController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MsHotelservicegrpsController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsHotelservicegrps != null ?
                        View(await _context.MsHotelservicegrps.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsHotelservicegrps'  is null.");
        }

        public async Task<IActionResult> Details(string id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelservicegrps == null)
            {
                return NotFound();
            }

            var msHotelservicegrp = await _context.MsHotelservicegrps
                .FirstOrDefaultAsync(m => m.Srvcgrpcde == id);
            if (msHotelservicegrp == null)
            {
                return NotFound();
            }

            return View(msHotelservicegrp);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Srvcgrpcde,Srvcgrpdesc")] MsHotelservicegrp msHotelservicegrp)
        {
            if (ModelState.IsValid)
            {
                msHotelservicegrp.Cmpyid = GetCmpyId();
                msHotelservicegrp.Userid = GetUserId();
                msHotelservicegrp.Revdtetime = DateTime.Now;

                _context.Add(msHotelservicegrp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(msHotelservicegrp);
        }

        public async Task<IActionResult> Edit(string id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelservicegrps == null)
            {
                return NotFound();
            }

            var msHotelservicegrp = await _context.MsHotelservicegrps.FindAsync(id);
            if (msHotelservicegrp == null)
            {
                return NotFound();
            }

            return View(msHotelservicegrp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Srvcgrpcde,Srvcgrpdesc")] MsHotelservicegrp msHotelservicegrp)
        {
            if (id != msHotelservicegrp.Srvcgrpcde)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msHotelservicegrp.Cmpyid = GetCmpyId();
                    msHotelservicegrp.Userid = GetUserId();
                    msHotelservicegrp.Revdtetime = DateTime.Now;
                    _context.Update(msHotelservicegrp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotelservicegrpExists(msHotelservicegrp.Srvcgrpcde))
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

            return View(msHotelservicegrp);
        }

        public async Task<IActionResult> Delete(string id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelservicegrps == null)
            {
                return NotFound();
            }

            var msHotelservicegrp = await _context.MsHotelservicegrps
                .FirstOrDefaultAsync(m => m.Srvcgrpcde == id);
            if (msHotelservicegrp == null)
            {
                return NotFound();
            }

            return View(msHotelservicegrp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.MsHotelservicegrps == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelservicegrps'  is null.");
            }
            var msHotelservicegrp = await _context.MsHotelservicegrps.FindAsync(id);
            if (msHotelservicegrp != null)
            {
                _context.MsHotelservicegrps.Remove(msHotelservicegrp);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelservicegrpExists(string id)
        {
            return (_context.MsHotelservicegrps?.Any(e => e.Srvcgrpcde == id)).GetValueOrDefault();
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
