using Hotel_Core_MVC_V1.Models;

namespace Hotel_Core_MVC_V1.Common
{
    public class CommonItems
    {
        private readonly HotelCoreMvcContext _db;
        public CommonItems(HotelCoreMvcContext context)
        {
            _db = context;
        }
        public static class CommonStrings
        {
            public const string connectionString = "HotelCoreMVCConnection";
            public const string duplicateDataMessage = "Duplicate data exists.";
        }
        #region Common Functions
        public class AllCommonFunctions
        {
            public DateTime CurrentDatetime()
            {
                DateTime d = new DateTime();
                d = DateTime.UtcNow.AddHours(6).AddMinutes(30);
                return d;
            }
            public int currentUserID()
            {
                return 1;
            }
            public short currentCompanyID()
            {
                return 1;
            }
            //public GetCurrentUserInfoByUserName_Result GetCurrentUserInfoByUserName(string currentUserName)
            //{
            //    if (!string.IsNullOrEmpty(currentUserName))
            //    {
            //        var empUserInfo = _db.GetCurrentUserInfoByUserName(currentUserName).FirstOrDefault<GetCurrentUserInfoByUserName_Result>();
            //        return empUserInfo;
            //    }
            //    return null;
            //}
            //public List<GetCurrentUserInfoByUserName_Result> GetUserInfoList(string userName, string email)
            //{
            //    var empUserInfo = _db.GetCurrentUserInfoByUserName(userName).AsEnumerable().Where(x => string.IsNullOrEmpty(email) || x.Email == email).ToList<GetCurrentUserInfoByUserName_Result>();
            //    //var empUserInfo = _db.GetCurrentUserInfoByUserName(userName).ToList<GetCurrentUserInfoByUserName_Result>();

            //    return empUserInfo;
            //}
            //public bool isPermission(string formName, string currentUserRole)
            //{
            //    return true;
            //}
            //public List<TransactionCodeModel> GetAllTranCodeList()
            //{
            //    try
            //    {
            //        var list = _db.ms_transactioncode.Select(x => new TransactionCodeModel()
            //        {
            //            trancodeid = x.trancodeid,
            //            trancode = x.trancode,
            //            trancodedesc = x.trancodedesc,
            //            createdby = x.createdby,
            //            createddate = x.createddate
            //        }).ToList();
            //        return list;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw ex;
            //    }
            //}
        }
        #endregion
    }
}
