using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsHotellocationsController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotellocationsController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        #region // Main methods //

        // GET: MsHotellocations
        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsHotellocations != null ?
                        View(await _context.MsHotellocations.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsHotellocations'  is null.");
        }

        // GET: MsHotellocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotellocations == null)
            {
                return NotFound();
            }

            var msHotellocation = await _context.MsHotellocations
                .FirstOrDefaultAsync(m => m.Locid == id);
            if (msHotellocation == null)
            {
                return NotFound();
            }

            return View(msHotellocation);
        }

        // GET: MsHotellocations/Create
        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        // POST: MsHotellocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Locid,Loccde,Locdesc,Cmpyid,Isoutlet,Revdtetime,Userid")] MsHotellocation msHotellocation)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                msHotellocation.Cmpyid = GetCmpyId();
                msHotellocation.Revdtetime = DateTime.Now;
                msHotellocation.Userid = GetUserId();
                _context.Add(msHotellocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msHotellocation);
        }

        // GET: MsHotellocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotellocations == null)
            {
                return NotFound();
            }

            var msHotellocation = await _context.MsHotellocations.FindAsync(id);
            if (msHotellocation == null)
            {
                return NotFound();
            }
            return View(msHotellocation);
        }

        // POST: MsHotellocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Locid,Loccde,Locdesc,Cmpyid,Isoutlet,Revdtetime,Userid")] MsHotellocation msHotellocation)
        {
            SetLayOutData();
            if (id != msHotellocation.Locid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msHotellocation.Cmpyid = GetCmpyId();
                    msHotellocation.Revdtetime = DateTime.Now;
                    msHotellocation.Userid = GetUserId();
                    _context.Update(msHotellocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotellocationExists(msHotellocation.Locid))
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
            return View(msHotellocation);
        }

        // GET: MsHotellocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotellocations == null)
            {
                return NotFound();
            }

            var msHotellocation = await _context.MsHotellocations
                .FirstOrDefaultAsync(m => m.Locid == id);
            if (msHotellocation == null)
            {
                return NotFound();
            }

            return View(msHotellocation);
        }

        // POST: MsHotellocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
            if (_context.MsHotellocations == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotellocations'  is null.");
            }
            var msHotellocation = await _context.MsHotellocations.FindAsync(id);
            if (msHotellocation != null)
            {
                _context.MsHotellocations.Remove(msHotellocation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotellocationExists(int id)
        {
            return (_context.MsHotellocations?.Any(e => e.Locid == id)).GetValueOrDefault();
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
