using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsHotelroomstatesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelroomstatesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        #region // Main methods //

        // GET: MsHotelroomstates
        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsHotelroomstates != null ?
                        View(await _context.MsHotelroomstates.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsHotelroomstates'  is null.");
        }

        // GET: MsHotelroomstates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelroomstates == null)
            {
                return NotFound();
            }

            var msHotelroomstate = await _context.MsHotelroomstates
                .FirstOrDefaultAsync(m => m.Rmstateid == id);
            if (msHotelroomstate == null)
            {
                return NotFound();
            }

            return View(msHotelroomstate);
        }

        // GET: MsHotelroomstates/Create
        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        // POST: MsHotelroomstates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rmstateid,Rmstatecde,Rmstatedesc,Syscolor")] MsHotelroomstate msHotelroomstate)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                msHotelroomstate.Cmpyid = funcs.currentCompanyID();
                msHotelroomstate.Revdtetime = funcs.CurrentDatetime();
                msHotelroomstate.Userid = funcs.currentUserID();
                _context.Add(msHotelroomstate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msHotelroomstate);
        }

        // GET: MsHotelroomstates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelroomstates == null)
            {
                return NotFound();
            }

            var msHotelroomstate = await _context.MsHotelroomstates.FindAsync(id);
            if (msHotelroomstate == null)
            {
                return NotFound();
            }
            return View(msHotelroomstate);
        }

        // POST: MsHotelroomstates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rmstateid,Rmstatecde,Rmstatedesc,Syscolor,Cmpyid,Revdtetime,Userid")] MsHotelroomstate msHotelroomstate)
        {
            SetLayOutData();
            if (id != msHotelroomstate.Rmstateid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msHotelroomstate.Cmpyid = funcs.currentCompanyID();
                    msHotelroomstate.Revdtetime = funcs.CurrentDatetime();
                    msHotelroomstate.Userid = funcs.currentUserID();
                    _context.Update(msHotelroomstate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotelroomstateExists(msHotelroomstate.Rmstateid))
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
            return View(msHotelroomstate);
        }

        // GET: MsHotelroomstates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelroomstates == null)
            {
                return NotFound();
            }

            var msHotelroomstate = await _context.MsHotelroomstates
                .FirstOrDefaultAsync(m => m.Rmstateid == id);
            if (msHotelroomstate == null)
            {
                return NotFound();
            }

            return View(msHotelroomstate);
        }

        // POST: MsHotelroomstates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
            if (_context.MsHotelroomstates == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelroomstates'  is null.");
            }
            var msHotelroomstate = await _context.MsHotelroomstates.FindAsync(id);
            if (msHotelroomstate != null)
            {
                _context.MsHotelroomstates.Remove(msHotelroomstate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelroomstateExists(int id)
        {
            return (_context.MsHotelroomstates?.Any(e => e.Rmstateid == id)).GetValueOrDefault();
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
