using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsAgenciesController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MsAgenciesController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();

            return _context.MsAgencies != null ?
                        View(await _context.MsAgencies.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsAgencies'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsAgencies == null)
            {
                return NotFound();
            }

            var msAgency = await _context.MsAgencies
                .FirstOrDefaultAsync(m => m.Agencyid == id);
            if (msAgency == null)
            {
                return NotFound();
            }

            return View(msAgency);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Agencynme,Email,Phone,Contperson,Address,Website,Creditlimit")] MsAgency msAgency)
        {
            if (ModelState.IsValid)
            {
                msAgency.Cmpyid = GetCmpyId();
                msAgency.Userid = GetUserId();
                msAgency.Revdtetime = DateTime.Now;
                _context.Add(msAgency);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msAgency);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsAgencies == null)
            {
                return NotFound();
            }

            var msAgency = await _context.MsAgencies.FindAsync(id);
            if (msAgency == null)
            {
                return NotFound();
            }
            return View(msAgency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Agencyid,Agencynme,Email,Phone,Contperson,Address,Website,Creditlimit")] MsAgency msAgency)
        {
            if (id != msAgency.Agencyid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msAgency.Cmpyid = GetCmpyId();
                    msAgency.Userid = GetUserId();
                    msAgency.Revdtetime = DateTime.Now;

                    _context.Update(msAgency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsAgencyExists(msAgency.Agencyid))
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
            return View(msAgency);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();

            if (id == null || _context.MsAgencies == null)
            {
                return NotFound();
            }

            var msAgency = await _context.MsAgencies
                .FirstOrDefaultAsync(m => m.Agencyid == id);
            if (msAgency == null)
            {
                return NotFound();
            }

            return View(msAgency);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MsAgencies == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsAgencies'  is null.");
            }
            var msAgency = await _context.MsAgencies.FindAsync(id);
            if (msAgency != null)
            {
                _context.MsAgencies.Remove(msAgency);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsAgencyExists(int id)
        {
            return (_context.MsAgencies?.Any(e => e.Agencyid == id)).GetValueOrDefault();
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
