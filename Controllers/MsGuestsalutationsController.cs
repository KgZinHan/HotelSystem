using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsGuestsalutationsController : Controller
    {
        private readonly HotelCoreMvcContext _context;


        #region // Main methods //
        public MsGuestsalutationsController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsGuestsalutations != null ?
                        View(await _context.MsGuestsalutations.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsGuestsalutations'  is null.");
        }

        public async Task<IActionResult> Details(short? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGuestsalutations == null)
            {
                return NotFound();
            }

            var msGuestsalutation = await _context.MsGuestsalutations
                .FirstOrDefaultAsync(m => m.Saluteid == id);
            if (msGuestsalutation == null)
            {
                return NotFound();
            }

            return View(msGuestsalutation);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Saluteid,Salutedesc")] MsGuestsalutation msGuestsalutation)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                msGuestsalutation.Userid = GetUserId();
                msGuestsalutation.Revdtetime = DateTime.Now;
                _context.Add(msGuestsalutation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msGuestsalutation);
        }

        public async Task<IActionResult> Edit(short? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGuestsalutations == null)
            {
                return NotFound();
            }

            var msGuestsalutation = await _context.MsGuestsalutations.FindAsync(id);
            if (msGuestsalutation == null)
            {
                return NotFound();
            }
            return View(msGuestsalutation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("Saluteid,Salutedesc")] MsGuestsalutation msGuestsalutation)
        {
            SetLayOutData();
            if (id != msGuestsalutation.Saluteid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msGuestsalutation.Userid = GetUserId();
                    msGuestsalutation.Revdtetime = DateTime.Now;
                    _context.Update(msGuestsalutation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsGuestsalutationExists(msGuestsalutation.Saluteid))
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
            return View(msGuestsalutation);
        }

        public async Task<IActionResult> Delete(short? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGuestsalutations == null)
            {
                return NotFound();
            }

            var msGuestsalutation = await _context.MsGuestsalutations
                .FirstOrDefaultAsync(m => m.Saluteid == id);
            if (msGuestsalutation == null)
            {
                return NotFound();
            }

            return View(msGuestsalutation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            SetLayOutData();
            if (_context.MsGuestsalutations == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsGuestsalutations'  is null.");
            }
            var msGuestsalutation = await _context.MsGuestsalutations.FindAsync(id);
            if (msGuestsalutation != null)
            {
                _context.MsGuestsalutations.Remove(msGuestsalutation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsGuestsalutationExists(short id)
        {
            return (_context.MsGuestsalutations?.Any(e => e.Saluteid == id)).GetValueOrDefault();
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
