using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meetaroo.Controllers{
    
    [TypeFilterAttribute(typeof(RetrieveSchemaActionFilter))]
    [Authorize]
    public class AnalyticsController : Controller{
        
        [HttpPost]
        public JsonResult TrackFor(TrackRequestDto dto)
        {
            Console.WriteLine($"track for presenation id : {dto.PresenationId}");
            return Json(new TrackResponseDto{AnalyticsId = 22});
        }
    }


    public class TrackRequestDto{
        public long? PresenationId{get;set;}
        public long? DocumentId {get;set;}
    }

    public class TrackResponseDto{
        public long AnalyticsId {get;set;}
    }
}