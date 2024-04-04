using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Hotel_Core_MVC_V1.Common
{
    [Authorize]
    public class CommonItems : Controller
    {
        public static class CommonStrings
        {
            public const string connectionString = "HotelCoreMVCConnection";
            public const string duplicateDataMessage = "Duplicate data exists.";

            // common
            public const string DEFAULT_DEPARTMENT = "FO";
            public const string DEFAULT_LEVEL = "ALL";

            // error messages
            public const string COMMON_ERROR_MESSAGE = "Error occured!";

            // pms_reservation
            public const string RESERVATION_CONFIRM = "C";
            public const string RESERVATION_CANCEL = "N";
            public const string RESERVATION_RESERVE = "R";

            // pms_roomledger
            public const string LEDGER_RESERVE = "reserve";
            public const string LEDGER_OCCUPIED = "occupied";
            public const string LEDGER_MAINTENANCE = "maintenance";
            public const string EXTEND_STAY = "Extend Stay";

            // pms_checkin
            public const string CHECK_IN_CHECKIN = "CheckIn";
            public const string CHECK_IN_NOSHOW = "NoShow";
            public const string CHECK_IN_CANCEL = "Cancel";

            // pms_roomfolioh
            public const string DEFAULT_FOLIO_CODE = "MF";

            // guestbilling
            public const string ROOM_CHARGES_CODE = "FO-RC001";
            public const string EXTRA_BED_CODE = "FO-EB001";
            public const string CASH_RECEIVE = "CASH_RECEIVE";

            // guest
            public const string DEFAULT_GUEST_CODE = "1401/H01";

            // night audit
            public const string DEFAULT_NIGHT_AUDIT_CODE = "lastnightauditdtetime";



        }


        #region Common Functions
        public class AllCommonFunctions
        {
            public DateTime CurrentDatetime()
            {
                var d = DateTime.UtcNow.AddHours(6).AddMinutes(30);
                return d;
            }
            public int currentUserID()
            {
                return 1;
            }

            public short currentUserIDShort()
            {
                return 1;
            }

            public short currentCompanyID()
            {
                return 1;
            }

            public int getImageSizeLimit()
            {
                return 1024 * 1024; //image size 1MB
            }
        }

        #endregion
    }
}
