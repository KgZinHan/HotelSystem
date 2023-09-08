using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Core_MVC_V1.Models;
using Hotel_Core_MVC_V1.Common;
using System.Text;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    public class MsUsersController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        //private readonly string key = "tkbH1omfiqg13aqVusoCialf7pE6whfU";
        private readonly AllCommonFunctions funcs;
        private readonly EncryptDecryptService encryptDecryptService;
        public MsUsersController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
            encryptDecryptService=new EncryptDecryptService();
        }
        public async Task<IActionResult> ResetPassword(int id)
        {
            if (!_context.MsUsers.Any(x=>x.Userid==id))
            {
                return NotFound();
            }
            var msUser= _context.MsUsers.FirstOrDefault(x=>x.Userid==id);
            if (msUser == null)
            {
                return NotFound();
            }
            string plainText = "User@123";
            string encryptedText = encryptDecryptService.EncryptString(plainText);
            msUser.Pwd = Encoding.UTF8.GetBytes(encryptedText);
            msUser.Revdtetime = funcs.CurrentDatetime();

            _context.Update(msUser);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login([Bind("Usernme,Usercde,Pwd,Cmpyid")] LoginModel model)
        {
            return View(model);
        }
        [HttpGet]
        public JsonResult AjaxLogin(string uc, string p)
        {
            if ((!string.IsNullOrEmpty(uc)) && (!string.IsNullOrEmpty(p)))
            {
                var v=_context.MsUsers.Where(x=>x.Usercde==uc).Select(x=>x).FirstOrDefault();
                if (v!=null && v.Pwd != null)
                {
                    string strbytes = Encoding.UTF8.GetString(v.Pwd);
                    string decryptedText = encryptDecryptService.DecryptString(strbytes);
                    if (decryptedText == p)
                    {
                        var cmpy=_context.MsHotelinfos.Where(x=>x.Cmpyid==v.Cmpyid).Select(x=>x.Hotelnme).FirstOrDefault();
                        return Json(cmpy);
                    }
                }
               
            }
            return Json("User Name or Password incorrect.");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult ChangePassword([Bind("Oldpwd,NewPwd,Confirmpwd")] ChangePasswordModel model)
        {
            return View(model);
        }
        // GET: MsUsers
        public async Task<IActionResult> Index()
        {
            var rawList = _context.MsUsers.Select(msUsers => new UserModel() {
                Cmpy = _context.MsHotelinfos.Where(x => x.Cmpyid == msUsers.Cmpyid).Select(x => x.Hotelnme).First(),
                Usercde = msUsers.Usercde,
                Usernme= msUsers.Usernme,
                Cmpyid= msUsers.Cmpyid,
                Userid=msUsers.Userid,
                Mnugrpid=msUsers.Mnugrpid
            }).ToListAsync();
              return _context.MsUsers != null ? 
                          View(await rawList) :
                          Problem("Entity set 'HotelCoreMvcContext.MsUsers'  is null.");
        }

        // GET: MsUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MsUsers == null)
            {
                return NotFound();
            }

            var msUser = await _context.MsUsers
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (msUser == null)
            {
                return NotFound();
            }
            UserModel model = new UserModel();
            model.Cmpy = _context.MsHotelinfos.Where(x => x.Cmpyid == msUser.Cmpyid).Select(x => x.Hotelnme).First();
            model.Usercde = msUser.Usercde;
            model.Usernme = msUser.Usernme;
            //model.Cmpyid = msUser.Cmpyid,
            model.Userid = msUser.Userid;
            //model.Mnugrpid = msUser.Mnugrpid

            return View(model);
        }

        // GET: MsUsers/Create
        public IActionResult Create()
        {

            ViewData["Cmpyid"] = new SelectList(_context.MsHotelinfos, "Cmpyid", "Hotelnme");
            return View();
        }

        // POST: MsUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,Usercde,Usernme,Pwd,Mnugrpid,Revdtetime,Cmpyid")] MsUser msUser)
        {
            if (ModelState.IsValid)
            {
                string plainText = "User@123";
                string encryptedText = encryptDecryptService.EncryptString(plainText);
                msUser.Pwd = Encoding.UTF8.GetBytes(encryptedText);

                //string decryptedText = encryptDecryptService.DecryptString(encryptedText);

                //MsUser msUser = new MsUser();

                //msUser.Usercde = model.Usercde;
                //msUser.Userid = model.Userid;
                //msUser.Usernme = model.Usernme;
                msUser.Mnugrpid = 1;
                //msUser.Cmpyid = model.Cmpyid;
                msUser.Revdtetime= funcs.CurrentDatetime();

                _context.Add(msUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cmpyid"] = new SelectList(_context.MsHotelinfos, "Cmpyid", "Hotelnme");
            return View(msUser);
        }

        // GET: MsUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MsUsers == null)
            {
                return NotFound();
            }

            var msUser = await _context.MsUsers.FindAsync(id);
            if (msUser == null)
            {
                return NotFound();
            }
            ViewData["Cmpyid"] = new SelectList(_context.MsHotelinfos, "Cmpyid", "Hotelnme");

            return View(msUser);
        }

        // POST: MsUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Userid,Usercde,Usernme,Pwd,Mnugrpid,Revdtetime,Cmpyid")] MsUser msUser)
        {
            if (id != msUser.Userid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    msUser.Revdtetime = funcs.CurrentDatetime();

                    _context.Update(msUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsUserExists(msUser.Userid))
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
            ViewData["Cmpyid"] = new SelectList(_context.MsHotelinfos, "Cmpyid", "Hotelnme");

            return View(msUser);
        }

        // GET: MsUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MsUsers == null)
            {
                return NotFound();
            }

            var msUser = await _context.MsUsers
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (msUser == null)
            {
                return NotFound();
            }
            UserModel model = new UserModel();
            model.Cmpy = _context.MsHotelinfos.Where(x => x.Cmpyid == msUser.Cmpyid).Select(x => x.Hotelnme).First();
            model.Usercde = msUser.Usercde;
            model.Usernme = msUser.Usernme;
            //model.Cmpyid = msUser.Cmpyid,
            model.Userid = msUser.Userid;
            //model.Mnugrpid = msUser.Mnugrpid

            return View(model);
        }

        // POST: MsUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MsUsers == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsUsers'  is null.");
            }
            var msUser = await _context.MsUsers.FindAsync(id);
            if (msUser != null)
            {
                _context.MsUsers.Remove(msUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsUserExists(int id)
        {
          return (_context.MsUsers?.Any(e => e.Userid == id)).GetValueOrDefault();
        }
    }
}
