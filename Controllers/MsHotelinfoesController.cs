using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Core_MVC_V1.Models;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    public class MsHotelinfoesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelinfoesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        // GET: MsHotelinfoes
        public async Task<IActionResult> Index()
        {
            var rawList=_context.MsHotelinfos.Select(x=> new HotelInfoModel() { 
            address = x.Address,
            area=_context.MsAreas.Where(j=>j.Areaid==x.Areaid).Select(j=>j.Areacde).FirstOrDefault(),
            autopostflg=x.Autopostflg,
            areaid=x.Areaid,
            autoposttime = x.Autoposttime.HasValue ? x.Autoposttime.Value.ToString("hh\\:mm") : "" ,
            checkintime= (x.Checkintime.HasValue? x.Checkintime.Value.ToString("hh\\:mm"):"")+ " / "+( x.Checkouttime.HasValue ? x.Checkouttime.Value.ToString("hh\\:mm") : ""),
            //checkouttime=x.Checkouttime,
            cmpyid=x.Cmpyid,
            email=x.Email,
            hoteldte=x.Hoteldte,
            hotelnme=x.Hotelnme,
            latecheckintime= x.Latecheckintime.HasValue ? x.Latecheckintime.Value.ToString("hh\\:mm") : "",
            phone1= String.Join(", ", x.Phone1,x.Phone2 ,x.Phone3) +" "+x.Website,
            tsp= _context.MsTownships.Where(j => j.Tspid == x.Tspid).Select(j => j.Tspcde).FirstOrDefault(),
            website=x.Website,
            revdtetime= x.Revdtetime,
            userid=x.Userid,
            tspid= x.Tspid
            }).ToListAsync();
              return _context.MsHotelinfos != null ? 
                          View(await rawList) :
                          Problem("Entity set 'HotelCoreMvcContext.MsHotelinfos'  is null.");
        }

        // GET: MsHotelinfoes/Details/5
        public async Task<IActionResult> Details(short? id)
        {
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
            model.cmpyid= msHotelinfo.Cmpyid;
            model.email= msHotelinfo.Email;
            model.checkintime = (msHotelinfo.Checkintime.HasValue ? msHotelinfo.Checkintime.Value.ToString("hh\\:mm") : "") + " / " + (msHotelinfo.Checkouttime.HasValue ? msHotelinfo.Checkouttime.Value.ToString("hh\\:mm") : "");
            model.phone1 = String.Join(", ", msHotelinfo.Phone1, msHotelinfo.Phone2, msHotelinfo.Phone3) +" | "+ msHotelinfo.Website;
            model.address = msHotelinfo.Address;
            model.area = _context.MsAreas.Where(j => j.Areaid == msHotelinfo.Areaid).Select(j => j.Areacde).FirstOrDefault();
            model.tsp = _context.MsTownships.Where(j => j.Tspid == msHotelinfo.Tspid).Select(j => j.Tspcde).FirstOrDefault();
            model.autoposttime = msHotelinfo.Autoposttime.HasValue ? msHotelinfo.Autoposttime.Value.ToString("hh\\:mm") : "";
            model.autopostflg = msHotelinfo.Autopostflg;
            model.hoteldte=msHotelinfo.Hoteldte;
            model.hotelnme=msHotelinfo.Hotelnme;
            return View(model);
        }

        // GET: MsHotelinfoes/Create
        public IActionResult Create()
        {
            ViewData["Areaid"] = new SelectList(_context.MsAreas, "Areaid", "Areacde");
            ViewData["Tspid"] = new SelectList(_context.MsTownships, "Tspid", "Tspcde");

            return View();
        }

        // POST: MsHotelinfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cmpyid,Hotelnme,Areaid,Tspid,Address,Hoteldte,Phone1,Phone2,Phone3,Email,Website,Checkintime,Checkouttime,Latecheckintime,Autopostflg,Autoposttime,Revdtetime,Userid")] MsHotelinfo msHotelinfo)
        {
            if (ModelState.IsValid)
            {
               // msHotelinfo.Cmpyid = funcs.currentCompanyID();
                msHotelinfo.Revdtetime = funcs.CurrentDatetime();
                msHotelinfo.Userid = funcs.currentUserID();

                _context.Add(msHotelinfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Areaid"] = new SelectList(_context.MsAreas, "Areaid", "Areacde");
            ViewData["Tspid"] = new SelectList(_context.MsTownships, "Tspid", "Tspcde");

            return View(msHotelinfo);
        }

        // GET: MsHotelinfoes/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
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

        // POST: MsHotelinfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("Cmpyid,Hotelnme,Areaid,Tspid,Address,Hoteldte,Phone1,Phone2,Phone3,Email,Website,Checkintime,Checkouttime,Latecheckintime,Autopostflg,Autoposttime,Revdtetime,Userid")] MsHotelinfo msHotelinfo)
        {
            if (id != msHotelinfo.Cmpyid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //msHotelinfo.Cmpyid = funcs.currentCompanyID();
                    msHotelinfo.Revdtetime = funcs.CurrentDatetime();
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

        // GET: MsHotelinfoes/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
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

        // POST: MsHotelinfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
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
    }
}
