using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Reporting.NETCore;
using NuGet.Protocol.Core.Types;


namespace Hotel_Core_MVC_V1.Controllers.Reports
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportsController(HotelCoreMvcContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();


            return View();
        }

        public IActionResult ShowReportForm(int reportNo)
        {
            var hotelDate = GetHotelDate();
            var resultReportForm = new ReportsModel();

            if (reportNo == 1)
            {
                resultReportForm.ReportTitle = "Room Available Occupancy Status Report";
            }
            else if (reportNo == 2)
            {
                resultReportForm.ReportTitle = "Out of Order Room Listing Report";
            }
            else if (reportNo == 3)
            {
                resultReportForm.ReportTitle = "In-house Guest Report";
            }
            else if (reportNo == 4)
            {
                resultReportForm.ReportTitle = "Expected Check-In Report";
            }
            else if (reportNo == 5)
            {
                resultReportForm.ReportTitle = "Expected Actual Check-In Report";
            }
            else if (reportNo == 6)
            {
                resultReportForm.ReportTitle = "Expected Check-Out Report";
            }
            else if (reportNo == 7)
            {
                resultReportForm.ReportTitle = "Expected Actual Check-Out Report";
            }
            else if (reportNo == 8)
            {
                resultReportForm.ReportTitle = "Noshow-Cancel Listing Report";
            }
            else if (reportNo == 9)
            {
                resultReportForm.ReportTitle = "Extend Stay Listing Report";
            }
            else if (reportNo == 10)
            {
                resultReportForm.ReportTitle = "Today Transfer/Upgrade Report";
            }
            else if (reportNo == 11)
            {
                resultReportForm.ReportTitle = "All Activities Report";
            }
            else if (reportNo == 12)
            {
                resultReportForm.ReportTitle = "Rooming List Report";
            }

            resultReportForm.FromDate = hotelDate; //Default
            resultReportForm.ToNext = 1; // Default
            resultReportForm.ReportTitleNo = reportNo;
            return PartialView("_ReportsPartialView", resultReportForm);
        }

        #endregion


        #region // Room Available Occupancy Report //

        public IActionResult RoomAvailableOccupancyPrint(DateTime FromDate, int ToNext)
        {
            var cmpyId = GetCmpyId();
            var userId = GetUserId();
            var hotelInfo = _context.MsHotelinfos.FirstOrDefault(hinfo => hinfo.Cmpyid == cmpyId);

            var reportList = new List<OccupancyReportModel>();

            // main data source
            var rawList = _context.RoomEnquiryDBSet.FromSqlRaw("EXEC GetAvailableRoomTypes @action={0}, @arrivalDate = {1}, @noofday = {2}, @cmpyid = {3},@departureDate = {4}", 2, FromDate, ToNext, cmpyId, FromDate)
                    .AsEnumerable()
                    .Select(x => new RoomEnquiryDB
                    {
                        ThisDate = x.theDate,
                        RoomType = x.rmtypcde,
                        RoomQty = x.roomqty,
                        BookQty = x.bookqty,
                        OccuState = x.occustate
                    })
                    .Where(x => x.OccuState != "reserve")
                    .ToList();

            var allRoomTypes = _context.MsHotelRoomTypes.Select(typ => typ.Rmtypcde).ToList();
            var totalRoomCount = new List<string>();
            foreach (var room in allRoomTypes)
            {
                int roomId = _context.MsHotelRoomTypes.Where(rmTyp => rmTyp.Rmtypcde == room).Select(rmTyp => rmTyp.Rmtypid).FirstOrDefault();
                var roomCount = _context.MsHotelRooms.Count(gb => gb.Rmtypid == roomId);
                totalRoomCount.Add(roomCount.ToString());
            }

            var dateList = new List<DateTime>();
            var allTotalRooms = 0;
            var allTotalBooks = 0;

            for (var i = 0; i < ToNext; i++)
            {
                var report = new OccupancyReportModel();
                var date = FromDate.AddDays(i);
                var count = 0;


                foreach (var room in allRoomTypes)
                {
                    var info = rawList.Where(raw => raw.ThisDate.Date == date.Date && raw.RoomType == room).Select(raw => raw.RoomQty - raw.BookQty).FirstOrDefault();
                    switch (count)
                    {
                        case 0:
                            report.billd = info;
                            break;
                        case 1:
                            report.uomrate = info;
                            break;
                        case 2:
                            report.qty = info;
                            break;
                        case 3:
                            report.pricebill = info;
                            break;
                        case 4:
                            report.shiftno = (short)info;
                            break;
                        case 5:
                            report.billdiscamt = info;
                            break;
                        case 6:
                            report.currrate = info;
                            break;
                        case 7:
                            report.cmpyid = (short)info;
                            break;
                    }
                    count++;
                }

                report.billdesp = date.ToString("dd/MM");

                report.userid = (short)rawList.Where(raw => raw.ThisDate.Date == date.Date).Sum(raw => raw.RoomQty - raw.BookQty);

                var totalRoom = rawList.Where(raw => raw.ThisDate.Date == date.Date).Sum(raw => raw.RoomQty);
                allTotalRooms += totalRoom;

                var totalBookRm = rawList.Where(raw => raw.ThisDate.Date == date.Date).Sum(raw => raw.BookQty);
                allTotalBooks += totalBookRm;

                float totalOccupancy = (float)totalBookRm * 100 / totalRoom;
                report.taxamt = (decimal)Math.Round(totalOccupancy, 2);
                reportList.Add(report);
            }

            var totalOccupancyPerc = allTotalBooks * 100 / allTotalRooms;

            try
            {
                var report = new LocalReport();
                var path = $"{_webHostEnvironment.WebRootPath}\\report\\RoomAvailableOccupancyReport.rdlc";
                report.ReportPath = path;
                report.DataSources.Add(new ReportDataSource("RoomOccupancyDataSet", reportList));
                report.SetParameters(new[] {
                    new ReportParameter("RoomType1",allRoomTypes[0] + "(" + totalRoomCount[0] +")"),
                    new ReportParameter("RoomType2",allRoomTypes[1] + "(" + totalRoomCount[1] +")"),
                    new ReportParameter("RoomType3",allRoomTypes[2] + "(" + totalRoomCount[2] +")"),
                    new ReportParameter("RoomType4",allRoomTypes[3] + "(" + totalRoomCount[3] +")"),
                    new ReportParameter("RoomType5",allRoomTypes[4] + "(" + totalRoomCount[4] +")"),
                    new ReportParameter("RoomType6",allRoomTypes[5] + "(" + totalRoomCount[5] +")"),
                    new ReportParameter("RoomType7",allRoomTypes[6] + "(" + totalRoomCount[6] +")"),
                    new ReportParameter("RoomType8",allRoomTypes[7] + "(" + totalRoomCount[7] +")"),
                    new ReportParameter("CompanyName",hotelInfo.Hotelnme.ToString()),
                    new ReportParameter("FromDate",FromDate.ToString("dd-MMM-yyyy")),
                    new ReportParameter("ToNext",ToNext.ToString()),
                    new ReportParameter("TotalRoom", allTotalRooms.ToString()),
                    new ReportParameter("TotalRent",allTotalBooks.ToString()),
                    new ReportParameter("OverAllOccupancy",totalOccupancyPerc.ToString()),
                    new ReportParameter("PrintedDatetime",DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"))
                 });
                var pdfBytes = report.Render("PDF");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion


        #region // InHouse Guest Report //

        public IActionResult InHouseGuestPrint()
        {
            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();

            var reportList = GetInHouseGuestReportList();
            var hotelInfo = _context.MsHotelinfos.FirstOrDefault(hinfo => hinfo.Cmpyid == cmpyId);
            try
            {
                var report = new LocalReport();
                var path = $"{_webHostEnvironment.WebRootPath}\\report\\InHouseGuestReport.rdlc";
                report.ReportPath = path;
                report.DataSources.Add(new ReportDataSource("DataSet1", reportList));
                report.SetParameters(new[] {
                    new ReportParameter("CompanyName",hotelInfo.Hotelnme.ToString()),
                    new ReportParameter("Date",hotelDate.ToString("dd MMM yyyy")),
                    new ReportParameter("PrintedDatetime",DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"))
                 });
                var pdfBytes = report.Render("PDF");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IEnumerable<InHouseGuestReportModel> GetInHouseGuestReportList()
        {
            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();
            var reportList = new List<InHouseGuestReportModel>();

            var inHouseList = _context.PmsCheckins
                .Join(
                    _context.PmsRoomledgers,
                    chkin => chkin.Checkinid,
                    ledg => ledg.Checkinid,
                    (chkin, ledg) => new { Checkin = chkin, Ledger = ledg }
                )
                .Where(joinResult => joinResult.Checkin.Checkoutflg != true)
                .GroupBy(joinResult => new { joinResult.Ledger.Roomno })
                .Select(group => new CheckInModel
                {
                    CheckInId = group.Max(joinResult => joinResult.Checkin.Checkinid),
                    RoomLgId = group.Max(joinResult => joinResult.Ledger.Roomlgid),
                    Roomno = group.Key.Roomno
                })
                .OrderBy(result => result.Roomno)
                .ToList();

            foreach (var inHouse in inHouseList)
            {

                // from roomLedgers
                var ledger = _context.PmsRoomledgers.Where(ledg => ledg.Roomlgid == inHouse.RoomLgId && ledg.Cmpyid == cmpyId).FirstOrDefault();

                // from checkins
                var chkIn = _context.PmsCheckins.Where(chkin => chkin.Checkinid == inHouse.CheckInId && chkin.Cmpyid == cmpyId).FirstOrDefault();

                // from msguestdata
                var chkInGuestData = _context.PmsCheckinroomguests.Where(crg => crg.Checkinid == inHouse.CheckInId).ToList();
                foreach (var chkInGuest in chkInGuestData)
                {
                    var report = new InHouseGuestReportModel();
                    var guestData = _context.MsGuestdata.Where(gd => gd.Guestid == chkInGuest.Guestid).FirstOrDefault();
                    if (guestData != null)
                    {
                        report.chrgacccde = ledger.Roomno ?? "";
                        report.emailaddr = chkIn.Checkindte.ToString("dd-MMM-yyyy");
                        report.remark = chkIn.Checkindte.AddDays(chkIn.Nightqty).ToString("dd-MMM-yyyy");
                        report.guestfullnme = guestData.Guestfullnme;
                        report.idppno = guestData.Idppno;
                        report.guestlastnme = _context.MsGuestcountries.Where(gc => gc.Countryid == guestData.Countryid).Select(gc => gc.Countrydesc).FirstOrDefault() ?? "";
                        report.phone1 = _context.MsGuestnationalities.Where(gc => gc.Nationid == guestData.Nationid).Select(gc => gc.Nationdesc).FirstOrDefault() ?? "";
                        report.phone2 = guestData.Gender == 1 ? "male" : "female";
                        reportList.Add(report);
                    }
                }
            }
            return reportList;
        }

        #endregion


        #region // Expected Check-In Report //

        public IActionResult ExpectedCheckInPrint(DateTime FromDate)
        {
            var cmpyId = GetCmpyId();

            var reportList = GetExpectedCheckInReportList(FromDate);
            var hotelInfo = _context.MsHotelinfos.FirstOrDefault(hinfo => hinfo.Cmpyid == cmpyId);

            try
            {
                var report = new LocalReport();
                var path = $"{_webHostEnvironment.WebRootPath}\\report\\ExpectedCheckInReport.rdlc";
                report.ReportPath = path;
                report.DataSources.Add(new ReportDataSource("DataSet1", reportList));
                report.SetParameters(new[] {
                    new ReportParameter("CompanyName",hotelInfo.Hotelnme.ToString()),
                    new ReportParameter("Date",FromDate.ToString("dd MMM yyyy")),
                    new ReportParameter("PrintedDatetime",DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"))
                 });
                var pdfBytes = report.Render("PDF");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IEnumerable<ExpectedCheckInReportModel> GetExpectedCheckInReportList(DateTime fromDate)
        {
            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var reportList = _context.PmsRoomledgers
                    .Where(ldg => ldg.Occudte.Date == fromDate.Date && ldg.Cmpyid == cmpyId)
                    .Join(_context.PmsReservations,
                    ldg => ldg.Resvno,
                    resv => resv.Resvno,
                    (ldg, resv) => new ExpectedCheckInReportModel
                    {
                        resvno = ldg.Resvno ?? "",
                        contactnme = resv.Contactnme ?? "",
                        contactno = resv.Contactno ?? "",
                        confirmcancelby = resv.Arrivedte.ToString("dd-MMM-yyyy"),
                        confirmdtetime = resv.Arrivedte,
                        resvmadeby = _context.MsHotelRoomTypes.Where(rmtyp => rmtyp.Rmtypid == ldg.Rmtypid).Select(rmtyp => rmtyp.Rmtypcde).FirstOrDefault() ?? "",
                        nightqty = resv.Nightqty,
                        adult = resv.Adult,
                        pickuploc = ldg.Preferroomno ?? "",
                        specialremark = resv.Specialremark ?? "",
                    })
                    .Where(x => x.confirmdtetime.Date == fromDate.Date)
                    .OrderBy(x => x.resvno)
                    .ToList();

            return reportList;
        }


        #endregion


        #region // Expected Actual Check-In Report //

        public IActionResult ExpectedActualCheckInPrint(DateTime FromDate)
        {
            var cmpyId = GetCmpyId();

            var reportList = GetExpectedActualCheckInReportList(FromDate);
            var hotelInfo = _context.MsHotelinfos.FirstOrDefault(hinfo => hinfo.Cmpyid == cmpyId);
            var expectedGuest = _context.PmsReservations.Where(rsv => rsv.Arrivedte.Date == FromDate.Date && rsv.Cmpyid == cmpyId).Count();
            var actualGuest = _context.PmsCheckins.Where(chk => chk.Checkindte.Date == FromDate.Date && chk.Cmpyid == cmpyId).Count();

            try
            {
                var report = new LocalReport();
                var path = $"{_webHostEnvironment.WebRootPath}\\report\\ExpectedActualCheckInReport.rdlc";
                report.ReportPath = path;
                report.DataSources.Add(new ReportDataSource("DataSet1", reportList));
                report.SetParameters(new[] {
                    new ReportParameter("CompanyName",hotelInfo?.Hotelnme.ToString()),
                    new ReportParameter("Date",FromDate.ToString("dd-MMM-yyyy")),
                    new ReportParameter("PrintedDatetime",DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt")),
                    new ReportParameter("Expected",expectedGuest.ToString()),
                    new ReportParameter("Actual",actualGuest.ToString())
                 });
                var pdfBytes = report.Render("PDF");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IEnumerable<ExpectedCheckInReportModel> GetExpectedActualCheckInReportList(DateTime FromDate)
        {
            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var reportList = _context.PmsRoomledgers
                    .Where(ldg => ldg.Occudte.Date == FromDate.Date
                                  && ldg.Occustate.Equals(CommonItems.CommonStrings.LEDGER_OCCUPIED)
                                  && ldg.Cmpyid == cmpyId)
                    .Join(_context.PmsCheckins,
                    ldg => ldg.Checkinid,
                    chkIn => chkIn.Checkinid,
                    (ldg, chkIn) => new ExpectedCheckInReportModel
                    {
                        resvno = ldg.Resvno ?? "",
                        checkinid = ldg.Checkinid ?? "",
                        resvmadeby = ldg.Roomno ?? "",
                        confirmdtetime = _context.PmsReservations.Where(rsv => rsv.Resvno == ldg.Resvno).Select(rsv => rsv.Arrivedte).FirstOrDefault(),
                        canceldtetime = FromDate.Date,
                        checkInDate = chkIn.Checkindte,
                        contactnme = _context.PmsReservations.Where(rsv => rsv.Resvno == ldg.Resvno).Select(rsv => rsv.Arrivedte).FirstOrDefault() == DateTime.MinValue
                        ? "" : _context.PmsReservations.Where(rsv => rsv.Resvno == ldg.Resvno).Select(rsv => rsv.Arrivedte).FirstOrDefault().ToString("dd-MMM-yyyy"),
                        contactno = FromDate.ToString("dd-MMM-yyyy"),
                        confirmcancelby = _context.MsHotelRoomRates.Where(r => r.Rmrateid == ldg.Rmrateid).Select(r => r.Rmratecde).FirstOrDefault() ?? "",
                        pickuploc = _context.PmsReservations.Where(rsv => rsv.Resvno == ldg.Resvno).Select(rsv => rsv.Arrivedte).FirstOrDefault().ToString("hh:mm:ss tt"),
                        specialremark = "",
                        userid = _context.MsUsers.Where(u => u.Userid == ldg.Userid).Select(u => u.Usernme).FirstOrDefault() ?? ""
                    })
                    .Where(x => x.checkInDate.Date == FromDate.Date)
                    .OrderBy(x => x.resvno)
                    .ToList();


            foreach (var report in reportList)
            {
                if (report.confirmdtetime.Date > report.canceldtetime.Date)
                {
                    report.specialremark = "Early";
                }

                if (report.resvno.IsNullOrEmpty())
                {
                    report.specialremark = "Walk-in";
                }

                var guestNameList = _context.PmsCheckinroomguests.Where(crg => crg.Checkinid == report.checkinid)
                    .Join(_context.MsGuestdata,
                    crg => crg.Guestid,
                    gd => gd.Guestid,
                    (crg, gd) => gd.Guestfullnme
                    )
                    .ToList();
                report.arrivedte = string.Join(", ", guestNameList);
            }

            return reportList;
        }


        #endregion


        #region // Expected Check-Out Report //

        public IActionResult ExpectedCheckOutPrint(DateTime FromDate)
        {
            var cmpyId = GetCmpyId();

            var reportList = GetExpectedCheckOutReportList(FromDate);
            var hotelInfo = _context.MsHotelinfos.FirstOrDefault(hinfo => hinfo.Cmpyid == cmpyId);

            try
            {
                var report = new LocalReport();
                var path = $"{_webHostEnvironment.WebRootPath}\\report\\ExpectedCheckOutReport.rdlc";
                report.ReportPath = path;
                report.DataSources.Add(new ReportDataSource("DataSet1", reportList));
                report.SetParameters(new[] {
                    new ReportParameter("CompanyName",hotelInfo.Hotelnme.ToString()),
                    new ReportParameter("Date",FromDate.ToString("dd-MMM-yyyy")),
                    new ReportParameter("PrintedDatetime",DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"))
                 });
                var pdfBytes = report.Render("PDF");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IEnumerable<ExpectedCheckOutReportModel> GetExpectedCheckOutReportList(DateTime fromDate)
        {
            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var reportList = _context.PmsRoomledgers
                    .Where(ldg => ldg.Cmpyid == cmpyId)
                    .Join(_context.PmsCheckins,
                    ledg => ledg.Checkinid,
                    chkin => chkin.Checkinid,
                    (ledg, chkin) => new ExpectedCheckOutReportModel
                    {
                        contactno = ledg.Roomno ?? "",
                        confirmcancelby = _context.MsHotelRoomRates.Where(r => r.Rmrateid == ledg.Rmrateid).Select(r => r.Rmratecde).FirstOrDefault() ?? "",
                        resvmadeby = _context.MsHotelRoomTypes.Where(rmtyp => rmtyp.Rmtypid == ledg.Rmtypid).Select(rmtyp => rmtyp.Rmtypcde).FirstOrDefault() ?? "",
                        arrivedte = chkin.Checkindte.ToString("dd-MMM-yyyy"),
                        confirmdtetime = chkin.Checkindte.AddDays(chkin.Nightqty).ToString("dd-MMM-yyyy"),
                        OccuDate = ledg.Occudte,
                        CheckOutDate = chkin.Checkindte.AddDays(chkin.Nightqty),
                        CheckInId = chkin.Checkinid
                    })
                    .Where(x => x.CheckOutDate.Date == fromDate.Date && x.OccuDate == fromDate.AddDays(-1).Date)
                    .OrderBy(x => x.contactno)
                    .ToList();

            foreach (var report in reportList)
            {
                var guestNameList = _context.PmsCheckinroomguests.Where(crg => crg.Checkinid == report.CheckInId)
                    .Join(_context.MsGuestdata,
                    crg => crg.Guestid,
                    gd => gd.Guestid,
                    (crg, gd) => gd.Guestfullnme
                    )
                    .ToList();

                report.contactnme = string.Join(", ", guestNameList);
            }

            return reportList;
        }


        #endregion


        #region // Expected Actual Check-Out Report //

        public IActionResult ExpectedActualCheckOutPrint(DateTime FromDate)
        {
            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();

            var reportList = GetExpectedActualCheckOutReportList(FromDate);
            var hotelInfo = _context.MsHotelinfos.FirstOrDefault(hinfo => hinfo.Cmpyid == cmpyId);
            var expectedGuest = _context.PmsCheckins.Where(chk => chk.Checkindte.AddDays(chk.Nightqty).Date == FromDate.Date && chk.Cmpyid == cmpyId).Count();
            var actualGuest = _context.PmsCheckins.Where(chk => chk.Checkoutdtetime.Value.Date == FromDate.Date && chk.Cmpyid == cmpyId).Count();

            try
            {
                var report = new LocalReport();
                var path = $"{_webHostEnvironment.WebRootPath}\\report\\ExpectedActualCheckOutReport.rdlc";
                report.ReportPath = path;
                report.DataSources.Add(new ReportDataSource("DataSet1", reportList));
                report.SetParameters(new[] {
                    new ReportParameter("CompanyName",hotelInfo?.Hotelnme.ToString()),
                    new ReportParameter("Date",FromDate.ToString("dd-MMM-yyyy")),
                    new ReportParameter("PrintedDatetime",DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt")),
                    new ReportParameter("Expected",expectedGuest.ToString()),
                    new ReportParameter("Actual",actualGuest.ToString())
                 });
                var pdfBytes = report.Render("PDF");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IEnumerable<ExpectedCheckOutReportModel> GetExpectedActualCheckOutReportList(DateTime FromDate)
        {
            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var reportList = (from ledg in _context.PmsRoomledgers
                              join chkin in _context.PmsCheckins
                              on ledg.Checkinid equals chkin.Checkinid
                              where ledg.Occudte == FromDate.AddDays(-1).Date & chkin.Checkoutflg == false & ledg.Cmpyid == cmpyId
                              select new ExpectedCheckOutReportModel
                              {
                                  CheckInId = chkin.Checkinid,
                                  contactnme = chkin.Checkindte.AddDays(chkin.Nightqty).ToString("dd-MMM-yyyy"),
                                  contactno = "",
                                  resvmadeby = ledg.Roomno ?? "",
                                  confirmcancelby = (from r in _context.MsHotelRoomRates
                                                     where r.Rmrateid == ledg.Rmrateid
                                                     select r.Rmratecde).FirstOrDefault() ?? "",
                                  userid = (from u in _context.MsUsers
                                            where u.Userid == ledg.Userid
                                            select u.Usernme).FirstOrDefault() ?? "",
                                  ActualCheckOutDate = chkin.Checkoutdtetime,
                                  ExpectedCheckOutDate = chkin.Checkindte.AddDays(chkin.Nightqty)
                              })
                              .Where(x => x.ActualCheckOutDate != null)
                              .OrderBy(x => x.contactno)
                              .ToList();

            var secondReportList = (from ledg in _context.PmsRoomledgers
                                    join chkin in _context.PmsCheckins
                                    on ledg.Checkinid equals chkin.Checkinid
                                    where ledg.Occudte == chkin.Checkoutdtetime.Value.Date
                                    select new ExpectedCheckOutReportModel
                                    {
                                        CheckInId = chkin.Checkinid,
                                        contactnme = chkin.Checkindte.AddDays(chkin.Nightqty).ToString("dd-MMM-yyyy"),
                                        contactno = "",
                                        resvmadeby = ledg.Roomno ?? "",
                                        confirmcancelby = (from r in _context.MsHotelRoomRates
                                                           where r.Rmrateid == ledg.Rmrateid
                                                           select r.Rmratecde).FirstOrDefault() ?? "",
                                        userid = (from u in _context.MsUsers
                                                  where u.Userid == ledg.Userid
                                                  select u.Usernme).FirstOrDefault() ?? "",
                                        ActualCheckOutDate = chkin.Checkoutdtetime,
                                        ExpectedCheckOutDate = chkin.Checkindte.AddDays(chkin.Nightqty)
                                    })
                              .Where(x => x.ActualCheckOutDate != null && x.ActualCheckOutDate.Value.Date == FromDate.Date)
                              .OrderBy(x => x.contactno)
                              .ToList();

            var list = reportList.Union(secondReportList);

            foreach (var report in list)
            {
                if (report.ActualCheckOutDate != null && report.ActualCheckOutDate > report.ExpectedCheckOutDate)
                {
                    report.specialremark = "Early";
                }

                var guestNameList = _context.PmsCheckinroomguests.Where(crg => crg.Checkinid == report.CheckInId)
                    .Join(_context.MsGuestdata,
                    crg => crg.Guestid,
                    gd => gd.Guestid,
                    (crg, gd) => gd.Guestfullnme
                    )
                    .ToList();
                report.arrivedte = string.Join(", ", guestNameList);
            }

            return list;
        }


        #endregion


        #region // NoShowCancel Report //

        public IActionResult NoShowCancelListingPrint(DateTime FromDate)
        {
            var cmpyId = GetCmpyId();

            var reportList = GetNoShowCancelReportList(FromDate);
            var hotelInfo = _context.MsHotelinfos.FirstOrDefault(hinfo => hinfo.Cmpyid == cmpyId);

            try
            {
                var report = new LocalReport();
                var path = $"{_webHostEnvironment.WebRootPath}\\report\\NoShowCancelReport.rdlc";
                report.ReportPath = path;
                report.DataSources.Add(new ReportDataSource("DataSet1", reportList));
                report.SetParameters(new[] {
                    new ReportParameter("CompanyName",hotelInfo.Hotelnme.ToString()),
                    new ReportParameter("Date",FromDate.ToString("dd MMM yyyy")),
                    new ReportParameter("PrintedDatetime",DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"))
                 });
                var pdfBytes = report.Render("PDF");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IEnumerable<NoShowCancelReportModel> GetNoShowCancelReportList(DateTime fromDate)
        {
            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var firstReportList = _context.PmsReservations
                    .Where(resv => resv.Canceldtetime.Value.Date == fromDate.Date && resv.Cmpyid == cmpyId)
                    .Join(_context.PmsRoomledgers,
                    resv => resv.Resvno,
                    ldg => ldg.Resvno,
                    (resv, ldg) => new NoShowCancelReportModel
                    {
                        contactnme = resv.Contactnme,
                        contactno = resv.Contactno,
                        resvmadeby = ldg.Roomno ?? "",
                        nightqty = resv.Nightqty,
                        specialremark = "Cancel",
                        canceldtetime = resv.Canceldtetime.Value.ToString("dd-MMM-yyyy"),
                        userid = _context.MsUsers.Where(u => u.Userid == ldg.Userid).Select(u => u.Usernme).FirstOrDefault() ?? "",
                        ResvState = resv.Resvstate,
                        OccuDte = ldg.Occudte
                    })
                    .Where(x => x.ResvState == CommonItems.CommonStrings.RESERVATION_CANCEL && x.OccuDte.Date == fromDate.Date)
                    .ToList();

            var secondReportList = _context.PmsReservations
                    .Where(resv => resv.Arrivedte.Date < fromDate.Date && resv.Cmpyid == cmpyId)
                    .Join(_context.PmsRoomledgers,
                    resv => resv.Resvno,
                    ldg => ldg.Resvno,
                    (resv, ldg) => new NoShowCancelReportModel
                    {
                        contactnme = resv.Contactnme,
                        contactno = resv.Contactno,
                        resvmadeby = ldg.Roomno ?? "",
                        nightqty = resv.Nightqty,
                        specialremark = "No Show",
                        canceldtetime = resv.Canceldtetime.Value.ToString("dd-MMM-yyyy"),
                        userid = _context.MsUsers.Where(u => u.Userid == ldg.Userid).Select(u => u.Usernme).FirstOrDefault() ?? "",
                        LedgerState = ldg.Occustate ?? "",
                        ResvDate = resv.Arrivedte,
                        OccuDte = ldg.Occudte
                    })
                    .Where(x => x.LedgerState == CommonItems.CommonStrings.LEDGER_RESERVE && x.OccuDte.Date == x.ResvDate.Date)
                    .ToList();

            var reportList = firstReportList.Union(secondReportList);

            return reportList;
        }


        #endregion


        #region // ExtendStay Report //

        public IActionResult ExtendStayListingPrint(DateTime FromDate)
        {
            var cmpyId = GetCmpyId();

            var reportList = GetExtendStayReportList(FromDate);
            var hotelInfo = _context.MsHotelinfos.FirstOrDefault(hinfo => hinfo.Cmpyid == cmpyId);

            try
            {
                var report = new LocalReport();
                var path = $"{_webHostEnvironment.WebRootPath}\\report\\ExtendStayReport.rdlc";
                report.ReportPath = path;
                report.DataSources.Add(new ReportDataSource("DataSet1", reportList));
                report.SetParameters(new[] {
                    new ReportParameter("CompanyName",hotelInfo.Hotelnme.ToString()),
                    new ReportParameter("Date",FromDate.ToString("dd MMM yyyy")),
                    new ReportParameter("PrintedDatetime",DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"))
                 });
                var pdfBytes = report.Render("PDF");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IEnumerable<ExtendStaysReportModel> GetExtendStayReportList(DateTime fromDate)
        {
            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var reportList = _context.PmsRoomledgers
                    .Where(ldg => ldg.Cmpyid == cmpyId && ldg.Occuremark == CommonItems.CommonStrings.EXTEND_STAY && ldg.Occudte.Date == fromDate.Date)
                    .Join(_context.PmsCheckins,
                    ldg => ldg.Checkinid,
                    chkin => chkin.Checkinid,
                    (ldg, chkin) => new ExtendStaysReportModel
                    {
                        contactnme = chkin.Checkindte.AddDays(chkin.Nightqty).ToString("dd-MMM-yyyy"),
                        resvmadeby = ldg.Roomno ?? "",
                        confirmcancelby = _context.MsHotelRoomRates.Where(r => r.Rmrateid == ldg.Rmrateid).Select(r => r.Rmratecde).FirstOrDefault() ?? "",
                        canceldtetime = chkin.Revdtetime.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                        userid = _context.MsUsers.Where(u => u.Userid == ldg.Userid).Select(u => u.Usernme).FirstOrDefault() ?? "",
                        CheckInId = chkin.Checkinid,
                        ExpectedOutDate = chkin.Checkindte.AddDays(chkin.Nightqty)
                    })
                    .ToList();

            foreach (var report in reportList)
            {
                var guestNameList = _context.PmsCheckinroomguests.Where(crg => crg.Checkinid == report.CheckInId)
                    .Join(_context.MsGuestdata,
                    crg => crg.Guestid,
                    gd => gd.Guestid,
                    (crg, gd) => gd.Guestfullnme
                    )
                    .ToList();
                report.arrivedte = string.Join(", ", guestNameList);

                var extendToDate = _context.PmsRoomledgers
                    .Where(ldg => ldg.Checkinid == report.CheckInId && ldg.Occuremark != null)
                    .Select(ldg => ldg.Occudte)
                    .FirstOrDefault();
                report.confirmdtetime = extendToDate.ToString("dd-MMM-yyyy");

                var diffDate = report.ExpectedOutDate - extendToDate;
                report.specialremark = diffDate.Days.ToString();
            }

            return reportList;
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
