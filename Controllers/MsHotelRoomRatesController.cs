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
    public class MsHotelRoomRatesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelRoomRatesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }


        #region // Main methods //
        // GET: MsHotelRoomRates
        public IActionResult Index()
        {
            SetLayOutData();
            var rawList = _context.MsHotelRoomRates.AsEnumerable().Select(x => new HotelRoomRateModel
            {
                Cmpyid = x.Cmpyid,
                Daymonthend = x.Daymonthend,
                Daymonthstart = x.Daymonthstart,
                Price = x.Price,
                Rmratecde = x.Rmratecde,
                Rmratedesc = x.Rmratedesc,
                Rmrateid = x.Rmrateid,
                Rmtypcde = _context.MsHotelRoomTypes.Where(j => j.Rmtypid == x.Rmtypid).Select(j => j.Rmtypcde).FirstOrDefault(),
                Rmtypid = x.Rmtypid
            }).ToList();
            return View(rawList);
            //return _context.MsHotelRoomRates != null ? 
            //              View(await _context.MsHotelRoomRates.ToListAsync()) :
            //              Problem("Entity set 'HotelCoreMvcContext.MsHotelRoomRates'  is null.");
        }

        // GET: MsHotelRoomRates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelRoomRates == null)
            {
                return NotFound();
            }

            var msHotelRoomRate = await _context.MsHotelRoomRates
                .FirstOrDefaultAsync(m => m.Rmrateid == id);
            if (msHotelRoomRate == null)
            {
                return NotFound();
            }
            //HotelRoomRateModel model = new HotelRoomRateModel();
            //model.Rmtypid = msHotelRoom.Rmtypid;
            //model.Cmpyid = msHotelRoom.Cmpyid;
            //model.Revdtetime = msHotelRoom.Revdtetime;
            //model.Userid = msHotelRoom.Userid;
            //model.Locid = msHotelRoom.Locid;
            //model.Loccde = _context.MsHotellocations.Where(x => x.Locid == msHotelRoom.Locid).Select(x => x.Loccde).FirstOrDefault();
            //model.Isautoapplyrate = msHotelRoom.Isautoapplyrate;
            //model.Usefixprice = msHotelRoom.Usefixprice;
            //model.Bedid = msHotelRoom.Bedid;
            //model.bedcde = _context.MsHotelroombeddings.Where(x => x.Bedid == msHotelRoom.Bedid).Select(x => x.Bedcde).FirstOrDefault();
            //model.Guestactivemsg = msHotelRoom.Guestactivemsg;
            //model.Isdnd = msHotelRoom.Isdnd;
            //model.Isguestin = msHotelRoom.Isguestin;
            //model.Paxno = msHotelRoom.Paxno;
            //model.Rmtypid = msHotelRoom.Rmtypid;
            //model.rmtypcde = _context.MsHotelRoomTypes.Where(x => x.Rmtypid == msHotelRoom.Rmtypid).Select(x => x.Rmtypcde).FirstOrDefault();
            //model.Roomno = msHotelRoom.Roomno;
            //model.Roomtelno = msHotelRoom.Roomtelno;
            return View(msHotelRoomRate);
        }

        // GET: MsHotelRoomRates/Create
        public IActionResult Create()
        {
            SetLayOutData();
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");
            return View();
        }

        // POST: MsHotelRoomRates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rmrateid,Rmratecde,Rmratedesc,Daymonthstart,Daymonthend,Rmtypid,Price,Cmpyid,Revdtetime,Userid")] MsHotelRoomRate msHotelRoomRate)
        {
            SetLayOutData();
            if (ModelState.IsValid)
            {
                msHotelRoomRate.Cmpyid = funcs.currentCompanyID();
                msHotelRoomRate.Revdtetime = funcs.CurrentDatetime();
                msHotelRoomRate.Userid = funcs.currentUserID();

                _context.Add(msHotelRoomRate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");
            return View(msHotelRoomRate);
        }

        // GET: MsHotelRoomRates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelRoomRates == null)
            {
                return NotFound();
            }

            var msHotelRoomRate = await _context.MsHotelRoomRates.FindAsync(id);
            if (msHotelRoomRate == null)
            {
                return NotFound();
            }
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");
            return View(msHotelRoomRate);
        }

        // POST: MsHotelRoomRates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rmrateid,Rmratecde,Rmratedesc,Daymonthstart,Daymonthend,Rmtypid,Price,Cmpyid,Revdtetime,Userid")] MsHotelRoomRate msHotelRoomRate)
        {
            SetLayOutData();
            if (id != msHotelRoomRate.Rmrateid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msHotelRoomRate.Cmpyid = funcs.currentCompanyID();
                    msHotelRoomRate.Revdtetime = funcs.CurrentDatetime();
                    msHotelRoomRate.Userid = funcs.currentUserID();

                    _context.Update(msHotelRoomRate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsHotelRoomRateExists(msHotelRoomRate.Rmrateid))
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
            ViewData["Rmtypid"] = new SelectList(_context.MsHotelRoomTypes, "Rmtypid", "Rmtypcde");
            return View(msHotelRoomRate);
        }

        // GET: MsHotelRoomRates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelRoomRates == null)
            {
                return NotFound();
            }

            var msHotelRoomRate = await _context.MsHotelRoomRates
                .FirstOrDefaultAsync(m => m.Rmrateid == id);
            if (msHotelRoomRate == null)
            {
                return NotFound();
            }

            return View(msHotelRoomRate);
        }

        // POST: MsHotelRoomRates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
            if (_context.MsHotelRoomRates == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelRoomRates'  is null.");
            }
            var msHotelRoomRate = await _context.MsHotelRoomRates.FindAsync(id);
            if (msHotelRoomRate != null)
            {
                _context.MsHotelRoomRates.Remove(msHotelRoomRate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelRoomRateExists(int id)
        {
            return (_context.MsHotelRoomRates?.Any(e => e.Rmrateid == id)).GetValueOrDefault();
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
