
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Core_MVC_V1.Common
{
    [Authorize]
    public class DefaultValues : Controller
    {
        public static class GlobalDefault
        {
            public const int SaluteId = 1; // U
            public const int CountryId = 13; //Myanmar
            public const int StateId = 1; // Yangon
            public const int NationId = 11; //Burma
        }

    }
}
