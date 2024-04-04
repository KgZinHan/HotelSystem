using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Hotel_Core_MVC_V1.Common.DefaultValues;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsGuestdataController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MsGuestdataController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //
        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            var guestList = await _context.MsGuestdata
                .Take(1000)
                .Select(g => new GuestInfo
                {
                    Guestid = g.Guestid,
                    Salutation = _context.MsGuestsalutations.Where(s => s.Saluteid == g.Saluteid).Select(s => s.Salutedesc).First(),
                    Guestfullnme = g.Guestfullnme,
                    Idppno = g.Idppno,
                    Gender = g.Gender,
                    Nationality = _context.MsGuestnationalities.Where(n => n.Nationid == g.Nationid).Select(n => n.Nationdesc).First(),
                    State = _context.MsGueststates.Where(s => s.Gstateid == g.Stateid).Select(s => s.Gstatedesc).First(),
                    Country = _context.MsGuestcountries.Where(c => c.Countryid == g.Countryid).Select(c => c.Countrydesc).First(),
                    LastVistedDate = g.Lastvisitdte,
                    VisitCount = g.Visitcount
                })
                .OrderByDescending(g => g.Guestid)
                .ToListAsync();

            foreach (var guest in guestList)
            {
                if (guest.Gender == 0)
                {
                    guest.GenderString = "male";
                }
                else if (guest.Gender == 1)
                {
                    guest.GenderString = "female";
                }
            }

            return View(guestList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MsGuestdata == null)
            {
                return NotFound();
            }

            var msGuestdatum = await _context.MsGuestdata
                .FirstOrDefaultAsync(m => m.Guestid == id);
            if (msGuestdatum == null)
            {
                return NotFound();
            }

            return View(msGuestdatum);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            var guest = new MsGuestdatum()
            {
                Nationid = GlobalDefault.NationId,
                Stateid = GlobalDefault.StateId,
                Saluteid = GlobalDefault.SaluteId,
                Countryid = GlobalDefault.CountryId
            };

            ViewData["GuestSalutations"] = new SelectList(_context.MsGuestsalutations, "Saluteid", "Salutedesc");
            ViewData["GuestCountries"] = new SelectList(_context.MsGuestcountries, "Countryid", "Countrydesc");
            ViewData["GuestStates"] = new SelectList(_context.MsGueststates, "Gstateid", "Gstatedesc");
            ViewData["GuestNationalities"] = new SelectList(_context.MsGuestnationalities, "Nationid", "Nationdesc");

            return View(guest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Saluteid,Guestfullnme,Guestlastnme,Idppno,Idissuedte,Gender,Dob,Stateid,Countryid,Nationid,Vipflg,Emailaddr,Phone1,Phone2,Crlimitamt,Remark")] MsGuestdatum msGuestdatum)
        {
            if (ModelState.IsValid)
            {
                msGuestdatum.Visitcount = 0;
                msGuestdatum.Cmpyid = GetCmpyId();
                msGuestdatum.Userid = GetUserId();
                msGuestdatum.Revdtetime = DateTime.Now;
                _context.Add(msGuestdatum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(msGuestdatum);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGuestdata == null)
            {
                return NotFound();
            }

            var msGuestdatum = await _context.MsGuestdata.FindAsync(id);
            if (msGuestdatum == null)
            {
                return NotFound();
            }

            ViewData["GuestSalutations"] = new SelectList(_context.MsGuestsalutations, "Saluteid", "Salutedesc");
            ViewData["GuestCountries"] = new SelectList(_context.MsGuestcountries, "Countryid", "Countrydesc");
            ViewData["GuestStates"] = new SelectList(_context.MsGueststates, "Gstateid", "Gstatedesc");
            ViewData["GuestNationalities"] = new SelectList(_context.MsGuestnationalities, "Nationid", "Nationdesc");
            return View(msGuestdatum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Guestid,Saluteid,Guestfullnme,Guestlastnme,Idppno,Idissuedte,Gender,Dob,Stateid,Countryid,Nationid,Vipflg,Emailaddr,Phone1,Phone2,Crlimitamt,Remark,Lastvisitdte,Visitcount")] MsGuestdatum msGuestdatum)
        {
            SetLayOutData();
            if (id != msGuestdatum.Guestid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msGuestdatum.Userid = GetUserId();
                    msGuestdatum.Revdtetime = DateTime.Now;
                    _context.Update(msGuestdatum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsGuestdatumExists(msGuestdatum.Guestid))
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

            ViewData["GuestSalutations"] = new SelectList(_context.MsGuestsalutations, "Saluteid", "Salutedesc");
            ViewData["GuestCountries"] = new SelectList(_context.MsGuestcountries, "Countryid", "Countrydesc");
            ViewData["GuestStates"] = new SelectList(_context.MsGueststates, "Gstateid", "Gstatedesc");
            ViewData["GuestNationalities"] = new SelectList(_context.MsGuestnationalities, "Nationid", "Nationdesc");

            return View(msGuestdatum);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsGuestdata == null)
            {
                return NotFound();
            }

            var msGuestdatum = await _context.MsGuestdata
                .FirstOrDefaultAsync(m => m.Guestid == id);
            if (msGuestdatum == null)
            {
                return NotFound();
            }

            return View(msGuestdatum);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
            if (_context.MsGuestdata == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsGuestdata'  is null.");
            }
            var msGuestdatum = await _context.MsGuestdata.FindAsync(id);
            if (msGuestdatum != null)
            {
                _context.MsGuestdata.Remove(msGuestdatum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsGuestdatumExists(int id)
        {
            return (_context.MsGuestdata?.Any(e => e.Guestid == id)).GetValueOrDefault();
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
