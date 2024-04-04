using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using static Hotel_Core_MVC_V1.Common.CommonItems;

namespace Hotel_Core_MVC_V1.Controllers
{
    [Authorize]
    public class MsHotelRoomTypesController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly AllCommonFunctions funcs;

        public MsHotelRoomTypesController(HotelCoreMvcContext context)
        {
            _context = context;
            funcs = new AllCommonFunctions();
        }

        #region // Main methods //

        public async Task<IActionResult> Index()
        {
            SetLayOutData();
            return _context.MsHotelRoomTypes != null ?
                        View(await _context.MsHotelRoomTypes.ToListAsync()) :
                        Problem("Entity set 'HotelCoreMvcContext.MsHotelRoomTypes'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelRoomTypes == null)
            {
                return NotFound();
            }

            var msHotelRoomType = await _context.MsHotelRoomTypes
                .FirstOrDefaultAsync(m => m.Rmtypid == id);
            if (msHotelRoomType == null)
            {
                return NotFound();
            }

            return View(msHotelRoomType);
        }

        public IActionResult Create()
        {
            SetLayOutData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rmtypid,Rmtypcde,Rmtypdesc,Paxno,Extrabedprice,Cmpyid,RmTypMainImg,RmTypImgList")] HotelRoomTypeModel msHotelRoomType)
        {
            SetLayOutData();
            if (!ModelState.IsValid)
            {
                return View(msHotelRoomType);
            }

            if (DuplicateDataExists(msHotelRoomType.Rmtypcde, msHotelRoomType.Rmtypid))
            {
                ViewBag.Msg = CommonStrings.duplicateDataMessage;
                return View(msHotelRoomType);
            }

            var hotelRoomType = new MsHotelRoomType()
            {
                Rmtypcde = msHotelRoomType.Rmtypcde,
                Rmtypdesc = msHotelRoomType.Rmtypdesc,
                Paxno = msHotelRoomType.Paxno,
                Extrabedprice = msHotelRoomType.Extrabedprice,
                Cmpyid = funcs.currentCompanyID(),
                Userid = funcs.currentUserID(),
                Revdtetime = funcs.CurrentDatetime()
            };

            _context.Add(hotelRoomType);
            await _context.SaveChangesAsync();

            var generatedId = hotelRoomType.Rmtypid;
            try
            {
                // Main Hotel Room Type Image
                if (msHotelRoomType.RmTypMainImg != null)
                {
                    if (msHotelRoomType.RmTypMainImg.Length > 0 && msHotelRoomType.RmTypMainImg.Length < funcs.getImageSizeLimit())
                    {
                        using var image = SixLabors.ImageSharp.Image.Load(msHotelRoomType.RmTypMainImg.OpenReadStream());
                        using var memoryStream = new MemoryStream();

                        msHotelRoomType.RmTypMainImg.CopyTo(memoryStream);
                        var byteImage = memoryStream.ToArray();

                        var roomTypeImage = new MsHotelRoomTypeImage()
                        {
                            Rmtypimgdesc = "main image",
                            Rmtypmainimg = byteImage,
                            Mainimgflg = true,
                            Revdtetime = funcs.CurrentDatetime(),
                            Userid = funcs.currentUserIDShort(),
                            Rmtypid = generatedId
                        };
                        _context.Add(roomTypeImage);
                    }
                    else
                    {
                        ViewBag.Msg = "Image size needs to be less than 1MB.";
                        return View(msHotelRoomType);
                    }
                }

                // Other Hotel Room Type Images
                if (msHotelRoomType.RmTypImgList != null)
                {
                    foreach (var img in msHotelRoomType.RmTypImgList)
                    {
                        if (img.Length > 0 && img.Length < funcs.getImageSizeLimit())
                        {
                            using var image = SixLabors.ImageSharp.Image.Load(img.OpenReadStream());
                            using var memoryStream = new MemoryStream();

                            img.CopyTo(memoryStream);
                            var byteImage = memoryStream.ToArray();

                            var roomTypeImage = new MsHotelRoomTypeImage()
                            {
                                Rmtypimgdesc = "other images",
                                Rmtypmainimg = byteImage,
                                Mainimgflg = false,
                                Revdtetime = funcs.CurrentDatetime(),
                                Userid = funcs.currentUserIDShort(),
                                Rmtypid = generatedId
                            };

                            _context.Add(roomTypeImage);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                _context.MsHotelRoomTypes.Remove(hotelRoomType);
                await _context.SaveChangesAsync();
                ViewBag.Msg = "Choosed file is not an image type.";
                return View(msHotelRoomType);
            }

        }

        public async Task<IActionResult> Edit(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelRoomTypes == null)
            {
                return NotFound();
            }

            var msHotelRoomType = await _context.MsHotelRoomTypes.FindAsync(id);
            if (msHotelRoomType == null)
            {
                return NotFound();
            }

            var hotelRoomType = new HotelRoomTypeModel()
            {
                Rmtypid = msHotelRoomType.Rmtypid,
                Rmtypcde = msHotelRoomType.Rmtypcde,
                Rmtypdesc = msHotelRoomType.Rmtypdesc,
                Paxno = msHotelRoomType.Paxno,
                Extrabedprice = (decimal)msHotelRoomType.Extrabedprice,
                Cmpyid = msHotelRoomType.Cmpyid,
                Revdtetime = msHotelRoomType.Revdtetime,
                Userid = msHotelRoomType.Userid
            };

            var mainImage = await _context.MsHotelRoomTypeImages
                .Where(img => img.Rmtypid == msHotelRoomType.Rmtypid && img.Mainimgflg == true)
                .Select(img => img.Rmtypmainimg)
                .FirstOrDefaultAsync();

            hotelRoomType.Base64Image = mainImage != null ? Convert.ToBase64String(mainImage) : "";

            var otherImageList = await _context.MsHotelRoomTypeImages
                .Where(img => img.Rmtypid == msHotelRoomType.Rmtypid && img.Mainimgflg == false)
                .Select(img => img.Rmtypmainimg)
                .ToListAsync();

            hotelRoomType.Base64ImageList = new List<string>();

            foreach (var otherImg in otherImageList)
            {
                var otherImgString = otherImg != null ? Convert.ToBase64String(otherImg) : "";
                hotelRoomType.Base64ImageList.Add(otherImgString);
            }

            return View(hotelRoomType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rmtypid,Rmtypcde,Rmtypdesc,Paxno,Extrabedprice,Cmpyid,Revdtetime,Userid,RmTypMainImg,RmTypImgList")] HotelRoomTypeModel hotelRoomTypeModel)
        {
            SetLayOutData();
            if (id != hotelRoomTypeModel.Rmtypid)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(hotelRoomTypeModel);
            }

            if (DuplicateDataExists(hotelRoomTypeModel.Rmtypcde, hotelRoomTypeModel.Rmtypid))
            {
                ViewBag.Msg = CommonStrings.duplicateDataMessage;
                return View(hotelRoomTypeModel);
            }

            var hotelRoomType = new MsHotelRoomType()
            {
                Rmtypid = hotelRoomTypeModel.Rmtypid,
                Rmtypcde = hotelRoomTypeModel.Rmtypcde,
                Rmtypdesc = hotelRoomTypeModel.Rmtypdesc,
                Paxno = hotelRoomTypeModel.Paxno,
                Extrabedprice = hotelRoomTypeModel.Extrabedprice,
                Cmpyid = funcs.currentCompanyID(),
                Userid = funcs.currentUserID(),
                Revdtetime = funcs.CurrentDatetime()
            };

            _context.Update(hotelRoomType);

            try
            {
                // Main Hotel Room Type Image
                if (hotelRoomTypeModel.RmTypMainImg != null)
                {
                    if (hotelRoomTypeModel.RmTypMainImg.Length > 0 && hotelRoomTypeModel.RmTypMainImg.Length < funcs.getImageSizeLimit())
                    {
                        using var image = SixLabors.ImageSharp.Image.Load(hotelRoomTypeModel.RmTypMainImg.OpenReadStream());
                        using var memoryStream = new MemoryStream();

                        hotelRoomTypeModel.RmTypMainImg.CopyTo(memoryStream);
                        var byteImage = memoryStream.ToArray();

                        var mainRoomTypeImage = _context.MsHotelRoomTypeImages
                            .FirstOrDefault(img => img.Rmtypid == hotelRoomTypeModel.Rmtypid && img.Mainimgflg == true);

                        if (mainRoomTypeImage != null)
                        {
                            mainRoomTypeImage.Rmtypmainimg = byteImage;
                            mainRoomTypeImage.Revdtetime = funcs.CurrentDatetime();
                            mainRoomTypeImage.Userid = funcs.currentUserIDShort();
                            _context.Update(mainRoomTypeImage);
                        }
                        else
                        {
                            var roomTypeImage = new MsHotelRoomTypeImage()
                            {
                                Rmtypimgdesc = "main image",
                                Rmtypmainimg = byteImage,
                                Mainimgflg = true,
                                Revdtetime = funcs.CurrentDatetime(),
                                Userid = funcs.currentUserIDShort(),
                                Rmtypid = hotelRoomTypeModel.Rmtypid
                            };
                            _context.Add(roomTypeImage);
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Image size needs to be less than 1MB.";
                        return View(hotelRoomTypeModel);
                    }
                }

                // Other Hotel Room Type Images
                if (hotelRoomTypeModel.RmTypImgList != null)
                {
                    // Delete previous other images first
                    _context.MsHotelRoomTypeImages
                            .Where(img => img.Rmtypid == hotelRoomTypeModel.Rmtypid && img.Mainimgflg == false)
                            .ExecuteDelete();

                    foreach (var img in hotelRoomTypeModel.RmTypImgList)
                    {
                        if (img.Length > 0 && img.Length < funcs.getImageSizeLimit())
                        {
                            using var image = SixLabors.ImageSharp.Image.Load(img.OpenReadStream());
                            using var memoryStream = new MemoryStream();

                            img.CopyTo(memoryStream);
                            var byteImage = memoryStream.ToArray();

                            var roomTypeImage = new MsHotelRoomTypeImage()
                            {
                                Rmtypimgdesc = "other images",
                                Rmtypmainimg = byteImage,
                                Mainimgflg = false,
                                Revdtetime = funcs.CurrentDatetime(),
                                Userid = funcs.currentUserIDShort(),
                                Rmtypid = hotelRoomTypeModel.Rmtypid
                            };

                            _context.Add(roomTypeImage);
                        }
                    }
                }

                await _context.SaveChangesAsync(); // Life of this function
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Msg = "Choosed file is not an image type.";
                return View(hotelRoomTypeModel);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            SetLayOutData();
            if (id == null || _context.MsHotelRoomTypes == null)
            {
                return NotFound();
            }

            var msHotelRoomType = await _context.MsHotelRoomTypes
                .FirstOrDefaultAsync(m => m.Rmtypid == id);
            if (msHotelRoomType == null)
            {
                return NotFound();
            }

            return View(msHotelRoomType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetLayOutData();
            if (_context.MsHotelRoomTypes == null)
            {
                return Problem("Entity set 'HotelCoreMvcContext.MsHotelRoomTypes'  is null.");
            }
            var msHotelRoomType = await _context.MsHotelRoomTypes.FindAsync(id);
            if (msHotelRoomType != null)
            {
                _context.MsHotelRoomTypes.Remove(msHotelRoomType);
                await _context.MsHotelRoomTypeImages.Where(img => img.Rmtypid == id).ExecuteDeleteAsync();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MsHotelRoomTypeExists(int id)
        {
            return (_context.MsHotelRoomTypes?.Any(e => e.Rmtypid == id)).GetValueOrDefault();
        }

        private bool DuplicateDataExists(string code, int? id)
        {
            return (_context.MsHotelRoomTypes?.Any(e => e.Rmtypcde == code && (id == null || e.Rmtypid != id))).GetValueOrDefault();
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
