using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel_Core_MVC_V1.Controllers.Messages
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public MessagesController(HotelCoreMvcContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            ViewData["Departments"] = new SelectList(_context.MsDepartments.Where(dept => dept.Cmpyid == cmpyId), "Deptcde", "Deptcde");
            ViewData["Users"] = new SelectList(_context.MsUsers.Where(u => u.Cmpyid == cmpyId), "Usernme", "Usernme");

            // all level Msgs
            var publicMsgs = _context.MsMessageeditors
                .Where(me => me.Msgtodept == CommonItems.CommonStrings.DEFAULT_LEVEL && me.Msgtoperson == null)
                .Select(me => new MessageModel
                {
                    MsgTypId = me.Msgtypid,
                    MsgTypCde = me.Msgtypcde,
                    Msgdetail = me.Msgdetail,
                    MsgTo = me.Msgtodept ?? "",
                    Priority = me.Priority,
                    Takedtetime = me.Takedtetime.ToString("dd MMM hh:mm tt"),
                    ResolvedFlg = me.Resolveflg,
                    UserName = _context.MsUsers.Where(u => u.Userid == me.Userid).Select(u => u.Usernme).FirstOrDefault()
                }).ToList();

            // department level Msgs
            var deptCde = _context.MsUsers.Where(u => u.Userid == userId).Select(u => u.Deptcde).FirstOrDefault();
            var deptMsgs = _context.MsMessageeditors
                .Where(me => me.Msgtodept == deptCde && me.Msgtoperson == null)
                .Select(me => new MessageModel
                {
                    MsgTypId = me.Msgtypid,
                    MsgTypCde = me.Msgtypcde,
                    Msgdetail = me.Msgdetail,
                    MsgTo = me.Msgtodept ?? "",
                    Priority = me.Priority,
                    ResolvedFlg = me.Resolveflg,
                    Takedtetime = me.Takedtetime.ToString("dd MMM hh:mm tt"),
                    UserName = _context.MsUsers.Where(u => u.Userid == me.Userid).Select(u => u.Usernme).FirstOrDefault()
                }).ToList();


            // private Msgs
            var userName = _context.MsUsers.Where(u => u.Userid == userId).Select(u => u.Usernme).FirstOrDefault();
            var privateMsgs = _context.MsMessageeditors
                .Where(me => me.Msgtoperson == userName)
                .Select(me => new MessageModel
                {
                    MsgTypId = me.Msgtypid,
                    MsgTypCde = me.Msgtypcde,
                    Msgdetail = me.Msgdetail,
                    MsgTo = "you",
                    Priority = me.Priority,
                    ResolvedFlg = me.Resolveflg,
                    Takedtetime = me.Takedtetime.ToString("dd MMM hh:mm tt"),
                    UserName = _context.MsUsers.Where(u => u.Userid == me.Userid).Select(u => u.Usernme).FirstOrDefault()
                }).ToList();

            // user msgs
            var myMsgs = _context.MsMessageeditors
                .Where(me => me.Msgtoperson != null && me.Userid == userId)
                .Select(me => new MessageModel
                {
                    MsgTypId = me.Msgtypid,
                    MsgTypCde = me.Msgtypcde,
                    Msgdetail = me.Msgdetail,
                    MsgTo = me.Msgtoperson,
                    Priority = me.Priority,
                    ResolvedFlg = me.Resolveflg,
                    Takedtetime = me.Takedtetime.ToString("dd MMM hh:mm tt"),
                    UserName = _context.MsUsers.Where(u => u.Userid == me.Userid).Select(u => u.Usernme).FirstOrDefault()
                }).ToList();


            var messages = publicMsgs.Union(deptMsgs).Union(privateMsgs).Union(myMsgs).OrderByDescending(gp => gp.MsgTypId);

            foreach (var message in messages)
            {
                if (message.ResolvedFlg == true)
                {
                    message.Resolved = "[Resolved]";
                }
            }

            var msgModels = new MessageModels()
            {
                Messages = messages,
                MessagesEditor = new MsMessageeditor(),
                totalMessage = _context.MsMessageeditors.Count(me => me.Takedtetime.Date == GetHotelDate().Date)
            };

            return View(msgModels);
        }

        public IActionResult Edit(int id)
        {

            SetLayOutData();

            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            ViewData["Departments"] = new SelectList(_context.MsDepartments.Where(dept => dept.Cmpyid == cmpyId), "Deptcde", "Deptcde");
            ViewData["Users"] = new SelectList(_context.MsUsers.Where(u => u.Cmpyid == cmpyId), "Usernme", "Usernme");

            // all level Msgs
            var publicMsgs = _context.MsMessageeditors
                .Where(me => me.Msgtodept == CommonItems.CommonStrings.DEFAULT_LEVEL && me.Msgtoperson == null)
                .Select(me => new MessageModel
                {
                    MsgTypId = me.Msgtypid,
                    MsgTypCde = me.Msgtypcde,
                    Msgdetail = me.Msgdetail,
                    MsgTo = me.Msgtodept ?? "",
                    Priority = me.Priority,
                    ResolvedFlg = me.Resolveflg,
                    Takedtetime = me.Takedtetime.ToString("dd MMM hh:mm tt"),
                    UserName = _context.MsUsers.Where(u => u.Userid == me.Userid).Select(u => u.Usernme).FirstOrDefault()
                }).ToList();

            // department level Msgs
            var deptCde = _context.MsUsers.Where(u => u.Userid == userId).Select(u => u.Deptcde).FirstOrDefault();
            var deptMsgs = _context.MsMessageeditors
                .Where(me => me.Msgtodept == deptCde && me.Msgtoperson == null)
                .Select(me => new MessageModel
                {
                    MsgTypId = me.Msgtypid,
                    MsgTypCde = me.Msgtypcde,
                    Msgdetail = me.Msgdetail,
                    MsgTo = me.Msgtodept ?? "",
                    Priority = me.Priority,
                    ResolvedFlg = me.Resolveflg,
                    Takedtetime = me.Takedtetime.ToString("dd MMM hh:mm tt"),
                    UserName = _context.MsUsers.Where(u => u.Userid == me.Userid).Select(u => u.Usernme).FirstOrDefault()
                }).ToList();


            // private Msgs
            var userName = _context.MsUsers.Where(u => u.Userid == userId).Select(u => u.Usernme).FirstOrDefault();
            var privateMsgs = _context.MsMessageeditors
                .Where(me => me.Msgtoperson == userName)
                .Select(me => new MessageModel
                {
                    MsgTypId = me.Msgtypid,
                    MsgTypCde = me.Msgtypcde,
                    Msgdetail = me.Msgdetail,
                    MsgTo = "you",
                    Priority = me.Priority,
                    ResolvedFlg = me.Resolveflg,
                    Takedtetime = me.Takedtetime.ToString("dd MMM hh:mm tt"),
                    UserName = _context.MsUsers.Where(u => u.Userid == me.Userid).Select(u => u.Usernme).FirstOrDefault()
                }).ToList();

            // user msgs
            var myMsgs = _context.MsMessageeditors
                .Where(me => me.Msgtoperson != null && me.Userid == userId)
                .Select(me => new MessageModel
                {
                    MsgTypId = me.Msgtypid,
                    MsgTypCde = me.Msgtypcde,
                    Msgdetail = me.Msgdetail,
                    MsgTo = me.Msgtoperson,
                    Priority = me.Priority,
                    ResolvedFlg = me.Resolveflg,
                    Takedtetime = me.Takedtetime.ToString("dd MMM hh:mm tt"),
                    UserName = _context.MsUsers.Where(u => u.Userid == me.Userid).Select(u => u.Usernme).FirstOrDefault()
                }).ToList();


            var messages = publicMsgs.Union(deptMsgs).Union(privateMsgs).Union(myMsgs).OrderByDescending(gp => gp.MsgTypId);

            foreach (var message in messages)
            {
                if (message.ResolvedFlg == true)
                {
                    message.Resolved = "Resolved";
                }
            }

            var roomLg = _context.PmsRoomledgers.Where(ledg => ledg.Roomlgid == id && ledg.Cmpyid == cmpyId).FirstOrDefault();
            var checkIn = _context.PmsCheckins.Where(chkIn => chkIn.Checkinid == roomLg.Checkinid && chkIn.Cmpyid == cmpyId).FirstOrDefault();
            var guestId = _context.PmsCheckinroomguests.Where(crg => crg.Checkinid == checkIn.Checkinid && crg.Principleflg == true).Select(crg => crg.Guestid).FirstOrDefault();
            var guest = _context.MsGuestdata.Where(gd => gd.Guestid == guestId && gd.Cmpyid == GetCmpyId()).FirstOrDefault();

            var msgModels = new MessageModels()
            {
                Messages = messages,
                MessagesEditor = new MsMessageeditor()
                {
                    Guestid = guest.Guestid,
                    Checkinid = checkIn.Checkinid,
                    Raisebynme = guest.Guestfullnme
                },
                totalMessage = _context.MsMessageeditors.Count(me => me.Takedtetime.Date == GetHotelDate().Date)
            };

            return View("~/Views/Messages/Index.cshtml", msgModels);
        }

        public MsMessageeditor GetMsg(int msgTypId)
        {
            var msg = _context.MsMessageeditors
                .FirstOrDefault(me => me.Msgtypid == msgTypId);

            return msg ?? new MsMessageeditor();

        }

        [HttpPost]
        public IActionResult SendMessage(MessageModels msgModels)
        {

            var msgEditor = msgModels.MessagesEditor;

            if (msgEditor != null)
            {
                msgEditor.Userid = GetUserId();
                msgEditor.Takedtetime = DateTime.Now;

                if (msgEditor.Msgtypid == 0)
                {
                    _context.MsMessageeditors.Add(msgEditor);
                }
                else
                {
                    if (msgEditor.Resolveflg == true)
                    {
                        msgEditor.Resolvedtetime = DateTime.Now;
                    }

                    _context.MsMessageeditors.Update(msgEditor);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");


        }


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
