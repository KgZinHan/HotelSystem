using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsDepartmentController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MsDepartmentController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            var rawList = await _context.MsDepartments.Select(dept => new Department()
            {
                Deptcde = dept.Deptcde,
                Cmpyid = _context.MsHotelinfos.Where(x => x.Cmpyid == dept.Cmpyid).Select(x => x.Hotelnme).First()

            }).ToListAsync();

            return View(rawList);
        }

        public async Task<IActionResult> Details(string deptCde)
        {
            SetLayOutData();
            if (deptCde == null || _context.MsDepartments == null)
            {
                return NotFound();
            }

            var department = await _context.MsDepartments
                .FirstOrDefaultAsync(m => m.Deptcde == deptCde);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Deptcde")] MsDepartment department)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                department.Cmpyid = GetCmpyId();
                department.Userid = GetUserId();
                department.Revdtetime = DateTime.Now;

                _context.Add(department);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

        public async Task<IActionResult> Edit(string deptCde)
        {
            SetLayOutData();

            if (deptCde == null || _context.MsDepartments == null)
            {
                return NotFound();
            }

            var department = await _context.MsDepartments.FindAsync(deptCde);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string deptCde, [Bind("Deptcde")] MsDepartment department)
        {
            SetLayOutData();

            if (ModelState.IsValid)
            {
                try
                {
                    department.Cmpyid = GetCmpyId();
                    department.Userid = GetUserId();
                    department.Revdtetime = DateTime.Now;

                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsDepartmentExists(department.Deptcde))
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

            return View(department);
        }

        public async Task<IActionResult> Delete(string deptCde)
        {
            SetLayOutData();
            if (deptCde == null || _context.MsDepartments == null)
            {
                return NotFound();
            }

            var department = await _context.MsDepartments
                .FirstOrDefaultAsync(dept => dept.Deptcde == deptCde);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string deptCde)
        {
            SetLayOutData();

            if (_context.MsReasons == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsDepartment'  is null.");
            }

            var department = await _context.MsDepartments.FindAsync(deptCde);
            if (department != null)
            {
                _context.MsDepartments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsDepartmentExists(string deptCde)
        {
            return (_context.MsDepartments?.Any(dept => dept.Deptcde == deptCde)).GetValueOrDefault();
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
