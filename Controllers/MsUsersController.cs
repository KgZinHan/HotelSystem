using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using System.Security.Claims;
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
            encryptDecryptService = new EncryptDecryptService();
        }

        #region // Log In methods //


        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity != null && claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["message"] = "Hello";

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login([Bind("Usercde,Pwd")] LoginModel model)
        {
            try
            {
                var logInUser = await _context.MsUsers.FirstOrDefaultAsync(x => x.Usercde == model.Usercde);

                if (logInUser != null && logInUser.Pwd != null)
                {
                    string strbytes = Encoding.UTF8.GetString(logInUser.Pwd);
                    string decryptedText = encryptDecryptService.DecryptString(strbytes);

                    if (decryptedText == model.Pwd)
                    {
                        var claims = new List<Claim>() {
                                new Claim(ClaimTypes.NameIdentifier, logInUser.Usercde)
                            };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var properties = new AuthenticationProperties()
                        {
                            AllowRefresh = true
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                        return RedirectToAction("Index", "Home");
                    }
                    ViewData["message"] = "decrypted error.";
                    return View();
                }
                ViewData["message"] = "null.";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<JsonResult> AjaxLogin(string uc, string p)
        {
            if ((!string.IsNullOrEmpty(uc)) && (!string.IsNullOrEmpty(p)))
            {
                var logInUser = await _context.MsUsers.Where(x => x.Usercde == uc).Select(x => x).FirstOrDefaultAsync();

                if (logInUser != null && logInUser.Pwd != null)
                {
                    string strbytes = Encoding.UTF8.GetString(logInUser.Pwd);
                    string decryptedText = encryptDecryptService.DecryptString(strbytes);

                    if (decryptedText == p)
                    {
                        try
                        {
                            var claims = new List<Claim>() {
                                new Claim(ClaimTypes.NameIdentifier, logInUser.Usercde)
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var properties = new AuthenticationProperties()
                            {
                                AllowRefresh = true
                            };

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);


                            var cmpy = await _context.MsHotelinfos
                            .Where(x => x.Cmpyid == logInUser.Cmpyid)
                            .Select(x => x.Hotelnme)
                            .FirstOrDefaultAsync();

                            return Json(cmpy);

                        }
                        catch (Exception ex)
                        {
                            return Json(ex.Message);
                        }
                    }
                    return Json("#Error.Password is incorrect!");
                }
                return Json("#Error.User code is incorrect!");
            }
            return Json("#Error.User Name or Password should not be empty!");
        }

        public async Task<IActionResult> ResetPassword(int id)
        {
            if (!_context.MsUsers.Any(x => x.Userid == id))
            {
                return NotFound();
            }
            var msUser = _context.MsUsers.FirstOrDefault(x => x.Userid == id);
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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "MsUsers");
        }

        #endregion


        #region // Users methods //

        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (!(claimUser.Identity != null && claimUser.Identity.IsAuthenticated))
            {
                return RedirectToAction("LogIn", "MsUsers"); // Use RedirectToAction instead of RedirectToActionResult
            }

            SetLayOutData();
            var rawList = _context.MsUsers.Select(msUsers => new UserModel()
            {
                Cmpy = _context.MsHotelinfos.Where(x => x.Cmpyid == msUsers.Cmpyid).Select(x => x.Hotelnme).First(),
                Usercde = msUsers.Usercde,
                Usernme = msUsers.Usernme,
                Cmpyid = msUsers.Cmpyid,
                Userid = msUsers.Userid,
                Deptcde = msUsers.Deptcde
            }).ToListAsync();
            return _context.MsUsers != null ?
                        View(await rawList) :
                        Problem("Entity set 'HotelCoreMvcContext.MsUsers'  is null.");
        }

        public IActionResult Create()
        {
            SetLayOutData();

            ViewData["Cmpyid"] = new SelectList(_context.MsHotelinfos, "Cmpyid", "Hotelnme");
            ViewData["Deptcdeid"] = new SelectList(_context.MsDepartments, "Deptcde", "Deptcde");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,Usercde,Usernme,Pwd,ConfirmPassword,Mnugrpid,Deptcde,Cmpyid")] UserModel user)
        {
            SetLayOutData();

            if (ModelState.IsValid)
            {
                if (user.ConfirmPassword != user.Pwd) // Password and Confirm Password is not same.
                {
                    ViewData["Cmpyid"] = new SelectList(_context.MsHotelinfos, "Cmpyid", "Hotelnme");
                    ViewData["Deptcdeid"] = new SelectList(_context.MsDepartments, "Deptcde", "Deptcde");

                    return View(user);
                }

                user.Pwd ??= "User@123"; // Give Default Password

                string encodedString = encryptDecryptService.EncryptString(user.Pwd);

                var msUser = new MsUser()
                {
                    Usercde = user.Usercde,
                    Usernme = user.Usernme,
                    Pwd = Encoding.UTF8.GetBytes(encodedString),
                    Mnugrpid = 1,
                    Deptcde = user.Deptcde,
                    Cmpyid = user.Cmpyid,
                    Revdtetime = DateTime.Now,
                };

                _context.MsUsers.Add(msUser);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["Cmpyid"] = new SelectList(_context.MsHotelinfos, "Cmpyid", "Hotelnme");
            ViewData["Deptcdeid"] = new SelectList(_context.MsDepartments, "Deptcde", "Deptcde");

            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsUsers == null)
            {
                return NotFound();
            }

            var user = await _context.MsUsers
                .Where(u => u.Userid == id)
                .Select(u => new UserModel
                {
                    Userid = u.Userid,
                    Usercde = u.Usercde,
                    Usernme = u.Usernme,
                    Cmpyid = u.Cmpyid,
                    Deptcde = u.Deptcde
                })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            ViewData["Cmpyid"] = new SelectList(_context.MsHotelinfos, "Cmpyid", "Hotelnme");
            ViewData["Deptcdeid"] = new SelectList(_context.MsDepartments, "Deptcde", "Deptcde");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Userid,Usercde,Usernme,Mnugrpid,Deptcde,Cmpyid")] UserModel user)
        {
            SetLayOutData();

            ViewData["Cmpyid"] = new SelectList(_context.MsHotelinfos, "Cmpyid", "Hotelnme");
            ViewData["Deptcdeid"] = new SelectList(_context.MsDepartments, "Deptcde", "Deptcde");

            if (id != user.Userid)
            {
                return NotFound();
            }

            user.NewPassword ??= "User@123";

            if (ModelState.IsValid)
            {
                try
                {
                    var msUser = await _context.MsUsers.FirstOrDefaultAsync(u => u.Userid == user.Userid);

                    if (msUser != null)
                    {
                        msUser.Usercde = user.Usercde;
                        msUser.Usernme = user.Usernme;
                        msUser.Deptcde = user.Deptcde;
                        msUser.Cmpyid = user.Cmpyid;
                        msUser.Revdtetime = funcs.CurrentDatetime();

                        _context.Update(msUser);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MsUserExists(user.Userid))
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

            return View(user);
        }

        public async Task<IActionResult> ChangePassword(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsUsers == null)
            {
                return NotFound();
            }

            var user = await _context.MsUsers
                .Where(u => u.Userid == id)
                .Select(u => new UserModel
                {
                    Userid = u.Userid,
                    Usercde = u.Usercde,
                    Usernme = u.Usernme
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int id, [Bind("Userid,Usercde,Usernme,Pwd,NewPassword,ConfirmPassword")] UserModel user)
        {
            SetLayOutData();

            var encryptPwd = _context.MsUsers
                .Where(u => u.Userid == user.Userid)
                .Select(u => u.Pwd)
                .FirstOrDefault();

            if (encryptPwd != null)
            {
                string strbytes = Encoding.UTF8.GetString(encryptPwd);
                if (user.Pwd != encryptDecryptService.DecryptString(strbytes))
                {
                    return View(user);
                }
            }

            if (user.NewPassword != user.ConfirmPassword)
            {
                return View(user);
            }

            string encodedNewPwd = encryptDecryptService.EncryptString(user.NewPassword);

            if (ModelState.IsValid)
            {
                try
                {
                    var msUser = await _context.MsUsers.FirstOrDefaultAsync(u => u.Userid == user.Userid);

                    if (msUser != null)
                    {
                        msUser.Pwd = Encoding.UTF8.GetBytes(encodedNewPwd);
                        msUser.Revdtetime = funcs.CurrentDatetime();
                        _context.Update(msUser);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();
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
            var model = new UserModel()
            {
                Cmpy = _context.MsHotelinfos.Where(x => x.Cmpyid == msUser.Cmpyid).Select(x => x.Hotelnme).First(),
                Usercde = msUser.Usercde,
                Usernme = msUser.Usernme,
                Deptcde = msUser.Deptcde,
                Userid = msUser.Userid
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
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
