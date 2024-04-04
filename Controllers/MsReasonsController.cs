using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsReasonsController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MsReasonsController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsReasons != null ?
                        View(await _context.MsReasons.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsReasons'  is null.");
        }

        public async Task<IActionResult> Details(short? id)
        {
            SetLayOutData();
            if (id == null || _context.MsReasons == null)
            {
                return NotFound();
            }

            var msReason = await _context.MsReasons
                .FirstOrDefaultAsync(m => m.Reasonid == id);
            if (msReason == null)
            {
                return NotFound();
            }

            return View(msReason);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Reasonid,Reasondesc,Guestuseflg")] MsReason msReason)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                msReason.Userid = GetUserId();
                msReason.Revdtetime = DateTime.Now;
                _context.Add(msReason);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msReason);
        }

        public async Task<IActionResult> Edit(short? id)
        {
            SetLayOutData();
            if (id == null || _context.MsReasons == null)
            {
                return NotFound();
            }

            var msReason = await _context.MsReasons.FindAsync(id);
            if (msReason == null)
            {
                return NotFound();
            }
            return View(msReason);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("Reasonid,Reasondesc,Guestuseflg")] MsReason msReason)
        {
            SetLayOutData();
            if (id != msReason.Reasonid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msReason.Userid = GetUserId();
                    msReason.Revdtetime = DateTime.Now;
                    _context.Update(msReason);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsReasonExists(msReason.Reasonid))
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
            return View(msReason);
        }

        public async Task<IActionResult> Delete(short? id)
        {
            SetLayOutData();
            if (id == null || _context.MsReasons == null)
            {
                return NotFound();
            }

            var msReason = await _context.MsReasons
                .FirstOrDefaultAsync(m => m.Reasonid == id);
            if (msReason == null)
            {
                return NotFound();
            }

            return View(msReason);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            SetLayOutData();
            if (_context.MsReasons == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsReasons'  is null.");
            }
            var msReason = await _context.MsReasons.FindAsync(id);
            if (msReason != null)
            {
                _context.MsReasons.Remove(msReason);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsReasonExists(short id)
        {
            return (_context.MsReasons?.Any(e => e.Reasonid == id)).GetValueOrDefault();
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
