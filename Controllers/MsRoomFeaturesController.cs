using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsRoomFeaturesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsRoomFeaturesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        #region // Main methods //

        // GET: MsRoomFeatures
        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsRoomFeatures != null ?
                        View(await _context.MsRoomFeatures.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsRoomFeatures'  is null.");
        }

        // GET: MsRoomFeatures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsRoomFeatures == null)
            {
                return NotFound();
            }

            var msRoomFeature = await _context.MsRoomFeatures
                .FirstOrDefaultAsync(m => m.Rmfeatureid == id);
            if (msRoomFeature == null)
            {
                return NotFound();
            }

            return View(msRoomFeature);
        }

        // GET: MsRoomFeatures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MsRoomFeatures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rmfeatureid,Rmfeaturecde,Rmfeaturedesc,Revdtetime,Userid")] MsRoomFeature msRoomFeature)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                msRoomFeature.Revdtetime = funcs.CurrentDatetime();
                msRoomFeature.Userid = funcs.currentUserID();
                _context.Add(msRoomFeature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msRoomFeature);
        }

        // GET: MsRoomFeatures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsRoomFeatures == null)
            {
                return NotFound();
            }

            var msRoomFeature = await _context.MsRoomFeatures.FindAsync(id);
            if (msRoomFeature == null)
            {
                return NotFound();
            }
            return View(msRoomFeature);
        }

        // POST: MsRoomFeatures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rmfeatureid,Rmfeaturecde,Rmfeaturedesc,Revdtetime,Userid")] MsRoomFeature msRoomFeature)
        {
            SetLayOutData();
            if (id != msRoomFeature.Rmfeatureid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msRoomFeature.Revdtetime = funcs.CurrentDatetime();
                    msRoomFeature.Userid = funcs.currentUserID();

                    _context.Update(msRoomFeature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsRoomFeatureExists(msRoomFeature.Rmfeatureid))
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
            return View(msRoomFeature);
        }

        // GET: MsRoomFeatures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsRoomFeatures == null)
            {
                return NotFound();
            }

            var msRoomFeature = await _context.MsRoomFeatures
                .FirstOrDefaultAsync(m => m.Rmfeatureid == id);
            if (msRoomFeature == null)
            {
                return NotFound();
            }

            return View(msRoomFeature);
        }

        // POST: MsRoomFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
            if (_context.MsRoomFeatures == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsRoomFeatures'  is null.");
            }
            var msRoomFeature = await _context.MsRoomFeatures.FindAsync(id);
            if (msRoomFeature != null)
            {
                _context.MsRoomFeatures.Remove(msRoomFeature);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsRoomFeatureExists(int id)
        {
            return (_context.MsRoomFeatures?.Any(e => e.Rmfeatureid == id)).GetValueOrDefault();
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
