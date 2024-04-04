using Hotel_Core_MVC_V1.Common;
using Hotel_Core_MVC_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Text;
using Rectangle = System.Drawing.Rectangle;


namespace Hotel_Core_MVC_V1.Controllers.GuestBilling
{
    [Authorize]
    public class GuestBillingController : Controller
    {
        private readonly HotelCoreMvcContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static List<Stream> m_streams = new List<Stream>();
        private static int m_currentPageIndex = 0;

        public GuestBillingController(HotelCoreMvcContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        #region // Main methods //

        public IActionResult Index()
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDte = GetHotelDate();

            var billingList = _context.PmsCheckins
                .Join(
                    _context.PmsRoomledgers,
                    chkin => chkin.Checkinid,
                    ledg => ledg.Checkinid,
                    (chkin, ledg) => new { Checkin = chkin, Ledger = ledg }
                )
                .Where(joinResult => joinResult.Checkin.Checkoutflg != true)
                .GroupBy(joinResult => new { joinResult.Ledger.Roomno })
                .Select(group => new BillingModel
                {
                    CheckInId = group.Max(joinResult => joinResult.Checkin.Checkinid),
                    RoomLgId = group.Max(joinResult => joinResult.Ledger.Roomlgid),
                    Roomno = group.Key.Roomno
                })
                .OrderBy(result => result.Roomno)
                .ToList();

            foreach (var billing in billingList)
            {
                var chkIn = _context.PmsCheckins.Where(chkin => chkin.Checkinid == billing.CheckInId && chkin.Cmpyid == cmpyId).FirstOrDefault();

                if (chkIn != null)
                {
                    billing.Occudte = chkIn.Checkindte;
                    billing.Departdte = chkIn.Checkindte.AddDays(chkIn.Nightqty);
                }

                billing.GuestName = GetGuestName(billing.CheckInId);
                billing.Balance = GetBalance(billing.CheckInId);
                billing.No = billingList.IndexOf(billing) + 1;
            }

            return View(billingList);
        }

        public IActionResult Edit(int id)
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();

            var checkInId = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == id && ledg.Cmpyid == cmpyId)
                .Select(ledg => ledg.Checkinid)
                .FirstOrDefault();

            if (checkInId == null)
            {
                return RedirectToAction("Index");
            }

            var checkInModel = new CheckInModel();
            var list = new List<AddBillModel>();

            var todayLedger = _context.PmsRoomledgers.FirstOrDefault(ledg => ledg.Checkinid == checkInId && ledg.Occudte.Date == hotelDate.Date && ledg.Cmpyid == cmpyId);

            var checkIn = _context.PmsCheckins.FirstOrDefault(chkIn => chkIn.Checkinid == checkInId && chkIn.Cmpyid == cmpyId);

            if (todayLedger != null && checkIn != null)
            {
                checkInModel = new CheckInModel()
                {
                    RoomLgId = todayLedger.Roomlgid,
                    CheckInId = todayLedger.Checkinid,
                    Roomno = todayLedger.Roomno,
                    GuestName = GetGuestName(checkInId)
                };

                // Room Charges
                var serviceCode = _context.MsHotelservicegrpds.FirstOrDefault(srvc => srvc.Srvccde == CommonItems.CommonStrings.ROOM_CHARGES_CODE);
                if (serviceCode != null)
                {
                    var addBill = new AddBillModel()
                    {
                        SrvcCde = serviceCode.Srvccde,
                        SrvcCdeDesc = serviceCode.Srvcdesc,
                        DeptCde = serviceCode.Deptcde,
                        FolioCde = GetFolioCode(checkInId, serviceCode.Srvccde),
                        Qty = 1, // Default one day
                        Price = todayLedger.Price,
                        Amount = todayLedger.Price - todayLedger.Discountamt
                    };

                    list.Add(addBill);
                }

                // Extra Bed
                if (todayLedger.Extrabedqty > 0)
                {
                    serviceCode = _context.MsHotelservicegrpds.FirstOrDefault(srvc => srvc.Srvccde == CommonItems.CommonStrings.EXTRA_BED_CODE);
                    if (serviceCode != null)
                    {
                        var addBill = new AddBillModel()
                        {
                            SrvcCde = serviceCode.Srvccde,
                            SrvcCdeDesc = serviceCode.Srvcdesc,
                            DeptCde = serviceCode.Deptcde,
                            FolioCde = GetFolioCode(checkInId, serviceCode.Srvccde),
                            Qty = todayLedger.Extrabedqty,
                            Price = todayLedger.Extrabedprice,
                            Amount = todayLedger.Extrabedqty * todayLedger.Extrabedprice
                        };

                        list.Add(addBill);
                    }
                }
            }

            var addBillModels = new AddBillModels()
            {
                CheckInModel = checkInModel,
                BillList = list
            };

            return View("~/Views/GuestBilling/Add.cshtml", addBillModels);
        }

        public IActionResult Add(int id)
        {
            SetLayOutData();

            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();

            var checkInId = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == id && ledg.Cmpyid == cmpyId)
                .Select(ledg => ledg.Checkinid)
                .FirstOrDefault();

            if (checkInId == null)
            {
                return RedirectToAction("Index");
            }

            var checkInModel = new CheckInModel();
            var list = new List<AddBillModel>();

            var todayLedger = _context.PmsRoomledgers
                .FirstOrDefault(ledg => ledg.Checkinid == checkInId && ledg.Occudte.Date == hotelDate.Date && ledg.Cmpyid == cmpyId);

            // When checkout read the bill from the previous day roomledger
            todayLedger ??= _context.PmsRoomledgers
                .FirstOrDefault(ledg => ledg.Checkinid == checkInId && ledg.Occudte.Date == hotelDate.Date.AddDays(-1) && ledg.Cmpyid == cmpyId);

            var checkIn = _context.PmsCheckins.FirstOrDefault(chkIn => chkIn.Checkinid == checkInId && chkIn.Cmpyid == cmpyId);

            if (todayLedger != null && checkIn != null)
            {
                checkInModel = new CheckInModel()
                {
                    RoomLgId = todayLedger.Roomlgid,
                    CheckInId = todayLedger.Checkinid,
                    Roomno = todayLedger.Roomno,
                    GuestName = GetGuestName(checkInId)
                };
            }

            var addBillModels = new AddBillModels()
            {
                CheckInModel = checkInModel,
                BillList = list
            };

            return View(addBillModels);
        }

        [HttpPost]
        public void SaveBills(string checkInId, string[][] billTable)
        {
            var cmpyId = GetCmpyId();
            var hotelDate = GetHotelDate();
            var userId = GetUserId();

            var todayLedger = _context.PmsRoomledgers
                .FirstOrDefault(ledg => ledg.Checkinid == checkInId && ledg.Occudte.Date == hotelDate.Date && ledg.Cmpyid == cmpyId);

            // When checkout insert the bill to the previous day roomledger
            todayLedger ??= _context.PmsRoomledgers
                .FirstOrDefault(ledg => ledg.Checkinid == checkInId && ledg.Occudte.Date == hotelDate.Date.AddDays(-1) && ledg.Cmpyid == cmpyId);


            var custName = _context.PmsCheckinroomguests
                .Where(crg => crg.Checkinid == checkInId && crg.Principleflg == true)
                .Join(_context.MsGuestdata,
                    crg => crg.Guestid,
                    gd => gd.Guestid,
                    (crg, gd) => gd.Guestfullnme
                )
                .FirstOrDefault();

            if (todayLedger != null)
            {
                var count = 0;

                foreach (var bill in billTable)
                {
                    var guestBilling = new PmsGuestbilling()
                    {
                        Checkinid = checkInId,
                        Billno = GenerateRefNo("BILL", count),
                        Bizdte = hotelDate,
                        Roomno = todayLedger.Roomno ?? "",
                        Custfullnme = custName ?? "",
                        Billdiscamt = 0,
                        Currcde = "MMK",
                        Currrate = 1,
                        Shiftno = GetShiftNo(),
                        Userid = userId,
                        Cmpyid = cmpyId,
                        Revdtetime = DateTime.Now,
                        Billdesp = GetSrvcDesc(bill[1]),
                        Srvccde = bill[1],
                        Deptcde = bill[2],
                        Foliocde = bill[3],
                        Qty = ParseDecimal(bill[4]),
                        Pricebill = ParseDecimal(bill[5]),
                        Remark = bill[7]
                    };

                    if (GetTranType(guestBilling.Srvccde) == "-") // If Trantyp is minus, pricebill should be minus
                    {
                        guestBilling.Pricebill = -guestBilling.Pricebill;
                    }

                    _context.PmsGuestbillings.Add(guestBilling);

                    count++;
                }

                var billNo = _context.MsAutonumbers.FirstOrDefault(no => no.Posid == "BILL" && no.Cmpyid == cmpyId);
                if (billNo != null)
                {
                    billNo.Lastusedno += count;
                    _context.MsAutonumbers.Update(billNo);
                }

                _context.SaveChanges();
            }
        }

        public IEnumerable<PmsGuestbilling> LoadBills(string checkInId)
        {
            var billList = _context.PmsGuestbillings
                .Where(gb => gb.Checkinid == checkInId && gb.Cmpyid == GetCmpyId())
                .ToList();

            return billList;
        }

        public IActionResult View(int id)
        {
            SetLayOutData();

            var checkInId = _context.PmsRoomledgers
                .Where(ledg => ledg.Roomlgid == id && ledg.Cmpyid == GetCmpyId())
                .Select(ledg => ledg.Checkinid)
                .FirstOrDefault();

            if (checkInId == null)
            {
                return RedirectToAction("Index");
            }

            // Main Folio !Important
            var mainFolioBills = _context.PmsGuestbillings
                .Where(gb => gb.Checkinid == checkInId
                && gb.Foliocde == CommonItems.CommonStrings.DEFAULT_FOLIO_CODE
                && gb.Cmpyid == GetCmpyId())
                .Select(gb => new BillingView
                {
                    BillId = gb.Billd,
                    BillNo = gb.Billno,
                    BillDesp = gb.Billdesp,
                    BizDte = gb.Bizdte,
                    RefNo2 = gb.Refno2,
                    POSId = gb.Posid,
                    OutletId = gb.Deptcde,
                    BillDateTime = gb.Revdtetime,
                    VoidFlg = gb.Voidflg,
                    Amount = gb.Qty * gb.Pricebill
                })
                .ToList();

            decimal mainFolioTotal = 0;

            foreach (var bill in mainFolioBills)
            {
                bill.No = mainFolioBills.IndexOf(bill) + 1;
                if (bill.VoidFlg != true)
                {
                    mainFolioTotal += bill.Amount ?? 0;
                }
            }

            var mainPaymentView = new PaymentBillingView()
            {
                CheckInId = checkInId,
                FolioCde = CommonItems.CommonStrings.DEFAULT_FOLIO_CODE,
                FolioName = "Main Folio",
                FolioTotal = mainFolioTotal.ToString("n0")
            };

            // Other Folios

            var finalList = new List<OtherBillingViewModels>();

            var folioList = _context.PmsRoomfoliohs
                .Where(rfh => rfh.Checkinid == checkInId && rfh.Cmpyid == GetCmpyId())
                .ToList();

            foreach (var folio in folioList)
            {
                var otherFolioBills = _context.PmsGuestbillings
                .Where(gb => gb.Checkinid == checkInId
                && gb.Foliocde == folio.Foliocde
                && gb.Cmpyid == GetCmpyId())
                .Select(gb => new BillingView
                {
                    BillId = gb.Billd,
                    BillNo = gb.Billno,
                    BillDesp = gb.Billdesp,
                    BizDte = gb.Bizdte,
                    RefNo2 = gb.Refno2,
                    POSId = gb.Posid,
                    OutletId = gb.Deptcde,
                    BillDateTime = gb.Revdtetime,
                    VoidFlg = gb.Voidflg,
                    Amount = gb.Qty * gb.Pricebill
                })
                .ToList();

                decimal otherFolioTotal = 0;

                foreach (var bill in otherFolioBills)
                {
                    bill.No = otherFolioBills.IndexOf(bill) + 1;
                    if (bill.VoidFlg != true)
                    {
                        otherFolioTotal += bill.Amount ?? 0;
                    }
                }

                var paymentView = new PaymentBillingView()
                {
                    CheckInId = checkInId,
                    FolioCde = folio.Foliocde,
                    FolioName = folio.Foliodesc,
                    FolioTotal = otherFolioTotal.ToString("n0"),
                    CloseFolioFlg = folio.Foliocloseflg
                };

                var otherBillingViewModels = new OtherBillingViewModels()
                {
                    OtherFolioBills = otherFolioBills,
                    PaymentBillingView = paymentView
                };

                finalList.Add(otherBillingViewModels);
            }

            var billingViewModel = new BillingViewModels()
            {
                MainFolioBills = mainFolioBills,
                PaymentBillingView = mainPaymentView,
                OtherFolioBillList = finalList
            };

            return View(billingViewModel);

        }

        [HttpPost]
        public void VoidBill(int billId)
        {
            var bill = _context.PmsGuestbillings
                .Where(bill => bill.Billd == billId && bill.Cmpyid == GetCmpyId())
                .FirstOrDefault();

            if (bill != null)
            {
                bill.Voidflg = true;
                bill.Voiddatetime = DateTime.Now;
                bill.Voiduserid = GetUserId();

                _context.PmsGuestbillings.Update(bill);
            }
            _context.SaveChanges();
        }

        [HttpPost]
        public void SavePayments(string folioCde, string checkInId, string srvcCde, string[][] paymentTable)
        {
            var hotelDate = GetHotelDate();
            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var todayLedger = _context.PmsRoomledgers.FirstOrDefault(ledg => ledg.Checkinid == checkInId && ledg.Occudte.Date == hotelDate.Date && ledg.Cmpyid == cmpyId);

            var custName = _context.PmsCheckinroomguests
                .Where(crg => crg.Checkinid == checkInId && crg.Principleflg == true)
                .Join(_context.MsGuestdata,
                    crg => crg.Guestid,
                    gd => gd.Guestid,
                    (crg, gd) => gd.Guestfullnme
                )
                .FirstOrDefault();

            if (todayLedger != null)
            {
                var count = 0;

                foreach (var payment in paymentTable)
                {
                    // Insert PmsGuestBillings
                    var guestBilling = new PmsGuestbilling()
                    {
                        Checkinid = checkInId,
                        Billno = GenerateRefNo("BILL", count),
                        Bizdte = hotelDate,
                        Roomno = todayLedger.Roomno ?? "",
                        Custfullnme = custName ?? "",
                        Billdiscamt = 0,
                        Currcde = payment[0],
                        Currrate = ParseInt16(payment[1]),
                        Shiftno = GetShiftNo(),
                        Userid = userId,
                        Cmpyid = cmpyId,
                        Revdtetime = DateTime.Now,
                        Billdesp = GetSrvcDesc(srvcCde),
                        Srvccde = srvcCde,
                        Deptcde = _context.MsHotelservicegrpds.Where(gd => gd.Srvccde == srvcCde && gd.Cmpyid == cmpyId).Select(gd => gd.Deptcde).FirstOrDefault(),
                        Foliocde = folioCde,
                        Qty = 1, // Default
                        Pricebill = ParseDecimal(payment[2])
                    };

                    if (GetTranType(guestBilling.Srvccde) == "-") // If Trantyp is minus, pricebill should be minus
                    {
                        guestBilling.Pricebill = -guestBilling.Pricebill;
                    }

                    _context.PmsGuestbillings.Add(guestBilling);

                    count++;
                }

                var billNo = _context.MsAutonumbers.FirstOrDefault(no => no.Posid == "BILL" && no.Cmpyid == cmpyId);
                if (billNo != null)
                {
                    billNo.Lastusedno += 1;
                    _context.MsAutonumbers.Update(billNo);
                }

                _context.SaveChanges();
            }
        }

        [HttpPost]
        public void CloseFolio(string folioCde, string checkInId)
        {
            var folio = _context.PmsRoomfoliohs
                .Where(rfh => rfh.Checkinid == checkInId && rfh.Foliocde == folioCde && rfh.Cmpyid == GetCmpyId())
                .FirstOrDefault();

            if (folio != null)
            {
                // Update PmsRoomfoliohs
                folio.Foliocloseflg = true;
                folio.Revdtetime = DateTime.Now;
                folio.Userid = GetUserId();

                _context.PmsRoomfoliohs.Update(folio);
            }

            _context.SaveChanges();

        }

        #endregion


        #region // Folio Report method //

        public IActionResult PrintReview(string checkInId, string folioCde)
        {
            var cmpyId = GetCmpyId();
            var userId = GetUserId();

            var folioBills = _context.PmsGuestbillings
            .Where(gb => gb.Checkinid == checkInId
                && gb.Foliocde == folioCde
                && gb.Voidflg != true
                && gb.Cmpyid == cmpyId)
                .Select(gb => new
                {
                    custfullnme = gb.Bizdte.ToString("dd-MMM-yyyy"),
                    srvccde = gb.Srvccde,
                    qty = gb.Qty,
                    pricebill = gb.Pricebill > 0 ? gb.Pricebill : -gb.Pricebill,
                    billdesp = gb.Billdesp,
                    billdiscamt = gb.Billdiscamt,
                    remark = gb.Remark,
                    itemdesc = _context.MsUsers.Where(u => u.Userid == gb.Userid && u.Cmpyid == cmpyId).Select(gb => gb.Usernme).FirstOrDefault(),
                })
                .ToList();

            var hotelInfo = _context.MsHotelinfos.FirstOrDefault(hinfo => hinfo.Cmpyid == cmpyId);

            var bill = _context.PmsGuestbillings
                .Where(gb => gb.Checkinid == checkInId
                && gb.Foliocde == folioCde
                && gb.Cmpyid == cmpyId)
                .FirstOrDefault();

            var checkIn = _context.PmsCheckins.Where(chkIn => chkIn.Checkinid == checkInId && chkIn.Cmpyid == cmpyId).FirstOrDefault();

            var billTotal = _context.PmsGuestbillings
                .Where(gb => gb.Checkinid == checkInId
                    && gb.Foliocde == folioCde
                    && gb.Voidflg != true
                    && gb.Cmpyid == cmpyId)
                .Sum(bill => bill.Qty * bill.Pricebill);

            try
            {
                var report = new LocalReport();
                var path = $"{this._webHostEnvironment.WebRootPath}\\report\\FolioReport.rdlc";
                report.ReportPath = path;
                report.DataSources.Add(new ReportDataSource("FolioReportDataSet", folioBills));
                report.SetParameters(new[] {
                    new ReportParameter("HotelName",hotelInfo.Hotelnme),
                    new ReportParameter("Phone",hotelInfo.Phone1),
                    new ReportParameter("Address",hotelInfo.Address),
                    new ReportParameter("FolioCode",folioCde),
                    new ReportParameter("ShiftNo", hotelInfo.Curshift.ToString()),
                    new ReportParameter("GuestName", bill.Custfullnme),
                    new ReportParameter("ArriveDate",checkIn.Checkindte.ToString("dd-MMM-yyyy")),
                    new ReportParameter("DepartDate",checkIn.Checkindte.AddDays(checkIn.Nightqty).ToString("dd-MMM-yyyy")),
                    new ReportParameter("billTotal",billTotal.ToString())
                 });
                var pdfBytes = report.Render("PDF");
                return File(pdfBytes, "application/pdf");
                //PrintToPrinter(report);
                //return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        #endregion


        #region // JS Functions //

        public IEnumerable<MsHotelservicegrpd> GetSrvcCodes()
        {
            var list = _context.MsHotelservicegrpds
                .Where(hsgd => hsgd.Cmpyid == GetCmpyId())
                .ToList();

            return list;
        }

        public SrvcFolio? GetOthersBySrvcCde(string checkInId, string srvcCde)
        {

            var srvcFolio = _context.MsHotelservicegrpds
                .Where(hsgd => hsgd.Srvccde == srvcCde)
                .Select(hsgd => new SrvcFolio
                {
                    SrvcCde = hsgd.Srvccde,
                    DeptCde = hsgd.Deptcde ?? "",
                })
                .FirstOrDefault();

            if (srvcFolio != null)
            {
                srvcFolio.FolioCde = GetFolio(checkInId, srvcCde);
            }

            return srvcFolio;
        }

        public string GetFolio(string checkInId, string srvcCde)
        {
            var folioHId = _context.PmsRoomfoliohs
                        .Where(folio => folio.Checkinid == checkInId && folio.Cmpyid == GetCmpyId())
                        .Join(_context.PmsRoomfoliods,
                        fh => fh.Foliohid,
                        fd => fd.Foliohid,
                        (fh, fd) => new
                        {
                            fh.Foliocde,
                            fd.Srvccde
                        })
                        .Where(gp => gp.Srvccde == srvcCde)
                        .Select(gp => gp.Foliocde)
                        .FirstOrDefault();

            return folioHId ?? CommonItems.CommonStrings.DEFAULT_FOLIO_CODE;
        }

        public IEnumerable<MsDepartment> GetDepartments()
        {
            var deptList = _context.MsDepartments
                .Where(dep => dep.Cmpyid == GetCmpyId())
                .ToList();

            return deptList;
        }

        public IEnumerable<SrvcFolio> GetFoliosById(string checkInId)
        {
            var list = _context.PmsRoomfoliohs
                .Where(fh => fh.Checkinid == checkInId && fh.Foliocloseflg == false && fh.Cmpyid == GetCmpyId())
                .Select(fh => new SrvcFolio
                {
                    FolioCde = fh.Foliocde
                })
                .ToList();

            return list;
        }

        public IEnumerable<MsCurrency> GetCurrencies()
        {
            var currencies = _context.MsCurrencies
                .ToList();

            return currencies;
        }


        #endregion


        #region // Utility methods for parsing //

        static Boolean ParseBool(string value)
        {
            if (Boolean.TryParse(value, out Boolean result))
            {
                return result;
            }
            return default;
        }

        static DateTime ParseDateTime(string value)
        {
            if (DateTime.TryParse(value, out DateTime result))
            {
                return result;
            }
            return default;
        }

        static byte ParseByte(string value)
        {
            if (byte.TryParse(value, out byte result))
            {
                return result;
            }
            return default;
        }

        static short ParseInt16(string value)
        {
            if (short.TryParse(value, out short result))
            {
                return result;
            }
            return default;
        }

        static int ParseInt32(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return default;
        }

        static decimal ParseDecimal(string value)
        {
            if (decimal.TryParse(value, out decimal result))
            {
                return result;
            }
            return default;
        }



        #endregion


        #region // Other Spin-off methods //


        protected PmsCheckin? GetCheckIn(string? checkInId)
        {
            if (checkInId != null)
            {
                var pmsCheckIn = _context.PmsCheckins.FirstOrDefault(chkIn => chkIn.Checkinid == checkInId && chkIn.Cmpyid == GetCmpyId());
                return pmsCheckIn;
            }
            else { return null; }

        }

        public string GetGuestName(string? checkInId)
        {
            var guestName = _context.PmsCheckinroomguests
                .Where(rg => rg.Checkinid == checkInId && rg.Principleflg == true)
                .Join(_context.MsGuestdata,
                rg => rg.Guestid,
                g => g.Guestid,
                (rg, g) => g.Guestfullnme)
                .FirstOrDefault();

            return guestName ?? "";
        }

        public decimal GetBalance(string? checkInId)
        {
            var billList = _context.PmsGuestbillings
                .Where(gb => gb.Checkinid == checkInId && gb.Voidflg != true && gb.Cmpyid == GetCmpyId())
                .ToList();

            decimal? balance = 0;

            foreach (var bill in billList)
            {
                balance += bill.Qty * bill.Pricebill;
            }

            return balance ?? 0;
        }

        public string GetFolioCode(string? checkInId, string srvcCde)
        {
            var folioCde = _context.PmsRoomfoliohs
                .Where(rfh => rfh.Checkinid == checkInId && rfh.Cmpyid == GetCmpyId())
                .Join(_context.PmsRoomfoliods,
                rfh => rfh.Foliohid,
                rfd => rfd.Foliohid,
                (rfh, rfd) => new
                {
                    folioCde = rfh.Foliocde,
                    srvcCde = rfd.Srvccde
                })
                .Where(gp => gp.srvcCde == srvcCde)
                .Select(gp => gp.folioCde)
                .FirstOrDefault();

            return folioCde ?? CommonItems.CommonStrings.DEFAULT_FOLIO_CODE;
        }

        public string GetSrvcDesc(string srvcCde)
        {
            var srvcDesc = _context.MsHotelservicegrpds
                .Where(sgd => sgd.Srvccde == srvcCde && sgd.Cmpyid == GetCmpyId())
                .Select(sgd => sgd.Srvcdesc)
                .FirstOrDefault();

            return srvcDesc ?? "";
        }

        protected string GenerateRefNo(string posId, int index)
        {
            var autoNumber = _context.MsAutonumbers.FirstOrDefault(no => no.Posid == posId && no.Cmpyid == GetCmpyId());
            if (autoNumber == null)
                return "";

            var generateNo = (autoNumber.Lastusedno + 1 + index).ToString();
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

        protected string GetTranType(string srvcCde)
        {
            var tranType = _context.MsHotelservicegrpds
                .Where(srvc => srvc.Srvccde == srvcCde)
                .Select(srvc => srvc.Trantyp)
                .FirstOrDefault();

            return tranType;
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


        #region // Direct Print Methods //

        public static void PrintToPrinter(LocalReport report)
        {
            Export(report);

        }

        public static void Export(LocalReport report, bool print = true)
        {
            string deviceInfo =
             @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>12in</PageWidth>
                <PageHeight>8in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0.1in</MarginLeft>
                <MarginRight>0.1in</MarginRight>
                <MarginBottom>0in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;

            if (print)
            {
                Print();
            }
        }

        public static void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }

        public static Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        public static void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        public static void DisposePrint()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }


        #endregion 


    }
}
