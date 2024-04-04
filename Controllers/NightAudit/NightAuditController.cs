using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Hotel_Core_MVC_V1.Controllers.NightAudit
{
    [Authorize]
    public class NightAuditController : Controller
    {
        private readonly HotelCoreMvcContext _context;

        public NightAuditController(HotelCoreMvcContext context)
        {
            _context = context;
        }


        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();
            var hotelDte = GetHotelDate();
            var flag = false;

            var lastDtetime = _context.Sysconfigs
                .Where(config => config.Keycde == CommonItems.CommonStrings.DEFAULT_NIGHT_AUDIT_CODE)
                .Select(config => config.Keyvalue)
                .FirstOrDefault();

            if (lastDtetime == null)
            {
                return NotFound();
            }

            var currentDateTime = DateTime.Now;
            var storeDtetime = DateTime.Parse(lastDtetime);
            var gapTime = currentDateTime - storeDtetime;
            var timeLeft = TimeSpan.FromHours(20) - gapTime;

            if (gapTime >= TimeSpan.FromHours(20))
            {
                flag = true;
                timeLeft = new TimeSpan(0);
            }



            var model = new NightAuditFormModel()
            {
                LastNightAuditDateTime = storeDtetime.ToString("dd MMM yyyy hh:mm:ss tt"),
                NightDate = GetHotelDate(),
                ShiftNo = GetShiftNo(),
                ButtonFlag = flag,
                TimeLeft = timeLeft.ToString("hh\\:mm\\:ss")
            };

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult DoNightAudit(DateTime nightdate)
        {
            var shiftNo = GetShiftNo();
            var userId = GetUserId();
            var cmpyId = GetCmpyId();

            // Load Data from sp_nightaudit
            var nightAuditList = _context.NightAuditDBSet
                    .FromSqlRaw("EXEC sp_nightaudit @nightdte={0}, @shiftno = {1}, @userid = {2}, @cmpyid = {3}", nightdate, shiftNo, userId, cmpyId)
                    .AsEnumerable()
                    .Select(x => new NightAuditDBModel
                    {
                        CheckInId = x.CheckInId,
                        BillNo = x.BillNo,
                        FolioCde = x.FolioCde,
                        BillHDesp = x.BillHDesp,
                        BizDte = x.BizDte,
                        RoomNo = x.RoomNo,
                        CustFullNme = x.CustFullNme,
                        BillAmt = x.BillAmt,
                        BillDiscAmt = x.BillDiscAmt,
                        CurrCde = x.CurrCde,
                        CurrRate = x.CurrRate,
                        TaxAmt = x.TaxAmt,
                        SvrcCde = x.SvrcCde,
                        ChrgAccCde = x.ChrgAccCde,
                        PosId = x.PosId,
                        DepCde = x.DepCde,
                        ShiftNo = x.ShiftNo,
                        Remark = x.Remark,
                        Userid = x.Userid,
                        Cmpyid = x.Cmpyid,
                        Revdtetime = x.Revdtetime
                    })
                    .ToList();

            // Update sysconfig for nightaudit
            var nightAuditSysConfig = _context.Sysconfigs.Where(config => config.Keycde == CommonItems.CommonStrings.DEFAULT_NIGHT_AUDIT_CODE).FirstOrDefault();
            if (nightAuditSysConfig != null)
            {
                var newNightAuditDtetime = DateTime.Now;
                nightAuditSysConfig.Keyvalue = newNightAuditDtetime.ToString();
                _context.Sysconfigs.Update(nightAuditSysConfig);
                _context.SaveChanges();
            }

            // Insert pmsguestbilling
            foreach (var nightAudit in nightAuditList)
            {
                SaveGuestBilling(nightAudit);
            }

            // Update HotelInfo
            var hotel = _context.MsHotelinfos.Where(hotel => hotel.Cmpyid == GetCmpyId()).FirstOrDefault();
            if (hotel != null && hotel.Hoteldte != null)
            {
                var newDate = new DateTime(hotel.Hoteldte.Value.Date.Year, hotel.Hoteldte.Value.Date.Month, hotel.Hoteldte.Value.Date.Day);
                newDate = newDate.AddDays(1);
                hotel.Hoteldte = newDate;
                _context.MsHotelinfos.Update(hotel);
                _context.SaveChanges();
            }

            // Insert GlobalActionLog
            var globalAction = new PmsGlobalactionlog()
            {
                Formnme = "NightAudit",
                Btnaction = "NightAudit",
                Actiondetail = nightAuditList.Count + " rows are audited at night.", // Manually scripted
                Revdtetime = DateTime.Now,
                Userid = userId
            };
            _context.PmsGlobalactionlogs.Add(globalAction);

            return RedirectToAction("Index");
        }

        public void SaveGuestBilling(NightAuditDBModel nightAudit)
        {
            var generateRefNo = GenerateRefNo("BILL");

            var guestBilling = new PmsGuestbilling()
            {
                Checkinid = nightAudit.CheckInId,
                Billno = generateRefNo,
                Bizdte = nightAudit.BizDte,
                Roomno = nightAudit.RoomNo,
                Custfullnme = nightAudit.CustFullNme ?? "",
                Billdiscamt = nightAudit.BillDiscAmt,
                Currcde = nightAudit.CurrCde,
                Currrate = nightAudit.CurrRate ?? 1,
                Shiftno = nightAudit.ShiftNo,
                Userid = nightAudit.Userid,
                Cmpyid = nightAudit.Cmpyid,
                Revdtetime = DateTime.Now,
                Billdesp = nightAudit.BillHDesp,
                Srvccde = nightAudit.SvrcCde ?? "",
                Deptcde = nightAudit.DepCde ?? "",
                Foliocde = nightAudit.FolioCde ?? "",
                Qty = 1,
                Pricebill = nightAudit.BillAmt - nightAudit.BillDiscAmt,
                Remark = nightAudit.Remark
            };

            _context.PmsGuestbillings.Add(guestBilling);

            var autoNumber = _context.MsAutonumbers.FirstOrDefault(no => no.Posid == "BILL" && no.Cmpyid == GetCmpyId());
            if (autoNumber != null)
            {
                autoNumber.Lastusedno += 1;
                _context.Update(autoNumber);
            }

            _context.SaveChanges();
        }

        #endregion


        #region // Other spin-off methods //

        protected string GenerateRefNo(string posId)
        {
            var autoNumber = _context.MsAutonumbers.FirstOrDefault(no => no.Posid == posId && no.Cmpyid == GetCmpyId());
            if (autoNumber == null)
                return "";

            var generateNo = (autoNumber.Lastusedno + 1).ToString();
            if (autoNumber.Zeroleading != null && autoNumber.Zeroleading == true)
            {
                var totalWidth = autoNumber.Runningno - autoNumber.Billprefix.Length - generateNo.Length;
                string paddedString = new string('0', (int)totalWidth) + generateNo;
                return autoNumber.Billprefix + paddedString;
            }
            else
            {
                return autoNumber.Billprefix + generateNo;
            }
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
