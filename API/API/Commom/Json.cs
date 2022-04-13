using System;
using System.Collections.Generic;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using API.Models;

namespace API
{
    class Json : Controller
    {
        public static JsonResult Serialize(Retorno ret)
        {
            //ret.versao = "v01.000";
            var JsonInstance = new API.Json();
            var JsonResult = JsonInstance.getJsonResult(ret);
            return JsonResult;
        }

        private JsonResult getJsonResult(Retorno ret)
        {
            var JsonRet = Json(ret);
            //JsonRet.MaxJsonLength = 2147483647;
            return JsonRet;
        }
    }
}
