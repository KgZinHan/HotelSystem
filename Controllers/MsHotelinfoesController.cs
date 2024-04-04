using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsHotelinfoesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelinfoesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }


        #region // Main methods //
        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            var rawList = _context.MsHotelinfos.Select(x => new HotelInfoModel()
            {
                address = x.Address,
                area = _context.MsAreas.Where(j => j.Areaid == x.Areaid).Select(j => j.Areacde).FirstOrDefault(),
                autopostflg = x.Autopostflg,
                areaid = x.Areaid,
                autoposttime = x.Autoposttime.HasValue ? x.Autoposttime.Value.ToString("hh\\:mm") : "",
                checkintime = (x.Checkintime.HasValue ? x.Checkintime.Value.ToString("hh\\:mm") : "") + " / " + (x.Checkouttime.HasValue ? x.Checkouttime.Value.ToString("hh\\:mm") : ""),
                //checkouttime=x.Checkouttime,
                cmpyid = x.Cmpyid,
                email = x.Email,
                hoteldte = x.Hoteldte,
                hotelnme = x.Hotelnme,
                latecheckintime = x.Latecheckintime.HasValue ? x.Latecheckintime.Value.ToString("hh\\:mm") : "",
                phone1 = String.Join(", ", x.Phone1, x.Phone2, x.Phone3) + " " + x.Website,
                tsp = _context.MsTownships.Where(j => j.Tspid == x.Tspid).Select(j => j.Tspcde).FirstOrDefault(),
                website = x.Website,
                revdtetime = x.Revdtetime,
                userid = x.Userid,
                tspid = x.Tspid
            }).ToListAsync();
            return _context.MsHotelinfos != null ?
                        View(await rawList) :
                        Problem("Entity set 'HotelCoreMvcContext.MsHotelinfos'  is null.");
        }

        public async Task<IActionResult> Details(short? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelinfos == null)
            {
                return NotFound();
            }

            var msHotelinfo = await _context.MsHotelinfos
                .FirstOrDefaultAsync(m => m.Cmpyid == id);
            if (msHotelinfo == null)
            {
                return NotFound();
            }
            HotelInfoModel model = new HotelInfoModel();
            model.cmpyid = msHotelinfo.Cmpyid;
            model.email = msHotelinfo.Email;
            model.checkintime = (msHotelinfo.Checkintime.HasValue ? msHotelinfo.Checkintime.Value.ToString("hh\\:mm") : "") + " / " + (msHotelinfo.Checkouttime.HasValue ? msHotelinfo.Checkouttime.Value.ToString("hh\\:mm") : "");
            model.phone1 = String.Join(", ", msHotelinfo.Phone1, msHotelinfo.Phone2, msHotelinfo.Phone3) + " | " + msHotelinfo.Website;
            model.address = msHotelinfo.Address;
            model.area = _context.MsAreas.Where(j => j.Areaid == msHotelinfo.Areaid).Select(j => j.Areacde).FirstOrDefault();
            model.tsp = _context.MsTownships.Where(j => j.Tspid == msHotelinfo.Tspid).Select(j => j.Tspcde).FirstOrDefault();
            model.autoposttime = msHotelinfo.Autoposttime.HasValue ? msHotelinfo.Autoposttime.Value.ToString("hh\\:mm") : "";
            model.autopostflg = msHotelinfo.Autopostflg;
            model.hoteldte = msHotelinfo.Hoteldte;
            model.hotelnme = msHotelinfo.Hotelnme;
            return View(model);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            ViewData["Areaid"] = new SelectList(_context.MsAreas, "Areaid", "Areacde");
            ViewData["Tspid"] = new SelectList(_context.MsTownships, "Tspid", "Tspcde");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cmpyid,Hotelnme,Areaid,Tspid,Address,Hoteldte,Phone1,Phone2,Phone3,Email,Website,Checkintime,Checkouttime,Latecheckintime,Autopostflg,Autoposttime,Noofshift,Curshift")] MsHotelinfo msHotelinfo)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                msHotelinfo.Revdtetime = DateTime.Now;
                msHotelinfo.Userid = GetUserId();

                _context.Add(msHotelinfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Areaid"] = new SelectList(_context.MsAreas, "Areaid", "Areacde");
            ViewData["Tspid"] = new SelectList(_context.MsTownships, "Tspid", "Tspcde");

            return View(msHotelinfo);
        }

        public async Task<IActionResult> Edit(short? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelinfos == null)
            {
                return NotFound();
            }

            var msHotelinfo = await _context.MsHotelinfos.FindAsync(id);
            if (msHotelinfo == null)
            {
                return NotFound();
            }
            ViewData["Areaid"] = new SelectList(_context.MsAreas, "Areaid", "Areacde");
            ViewData["Tspid"] = new SelectList(_context.MsTownships, "Tspid", "Tspcde");

            return View(msHotelinfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("Cmpyid,Hotelnme,Areaid,Tspid,Address,Hoteldte,Phone1,Phone2,Phone3,Email,Website,Checkintime,Checkouttime,Latecheckintime,Autopostflg,Autoposttime,Noofshift,Curshift")] MsHotelinfo msHotelinfo)
        {
            SetLayOutData();
            if (id != msHotelinfo.Cmpyid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msHotelinfo.Revdtetime = DateTime.Now;
                    msHotelinfo.Userid = funcs.currentUserID();

                    _context.Update(msHotelinfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotelinfoExists(msHotelinfo.Cmpyid))
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
            ViewData["Areaid"] = new SelectList(_context.MsAreas, "Areaid", "Areacde");
            ViewData["Tspid"] = new SelectList(_context.MsTownships, "Tspid", "Tspcde");

            return View(msHotelinfo);
        }

        public async Task<IActionResult> Delete(short? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelinfos == null)
            {
                return NotFound();
            }

            var msHotelinfo = await _context.MsHotelinfos
                .FirstOrDefaultAsync(m => m.Cmpyid == id);
            if (msHotelinfo == null)
            {
                return NotFound();
            }
            HotelInfoModel model = new HotelInfoModel();
            model.cmpyid = msHotelinfo.Cmpyid;
            model.email = msHotelinfo.Email;
            model.checkintime = (msHotelinfo.Checkintime.HasValue ? msHotelinfo.Checkintime.Value.ToString("hh\\:mm") : "") + " / " + (msHotelinfo.Checkouttime.HasValue ? msHotelinfo.Checkouttime.Value.ToString("hh\\:mm") : "");
            model.phone1 = String.Join(", ", msHotelinfo.Phone1, msHotelinfo.Phone2, msHotelinfo.Phone3) + " | " + msHotelinfo.Website;
            model.address = msHotelinfo.Address;
            model.area = _context.MsAreas.Where(j => j.Areaid == msHotelinfo.Areaid).Select(j => j.Areacde).FirstOrDefault();
            model.tsp = _context.MsTownships.Where(j => j.Tspid == msHotelinfo.Tspid).Select(j => j.Tspcde).FirstOrDefault();
            model.autoposttime = msHotelinfo.Autoposttime.HasValue ? msHotelinfo.Autoposttime.Value.ToString("hh\\:mm") : "";
            model.autopostflg = msHotelinfo.Autopostflg;
            model.hoteldte = msHotelinfo.Hoteldte;
            model.hotelnme = msHotelinfo.Hotelnme;

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            SetLayOutData();
            if (_context.MsHotelinfos == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelinfos'  is null.");
            }
            var msHotelinfo = await _context.MsHotelinfos.FindAsync(id);
            if (msHotelinfo != null)
            {
                _context.MsHotelinfos.Remove(msHotelinfo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelinfoExists(short id)
        {
            return (_context.MsHotelinfos?.Any(e => e.Cmpyid == id)).GetValueOrDefault();
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
