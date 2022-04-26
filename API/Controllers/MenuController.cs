using System;
using Microsoft;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using API.Models;
using API.Common;


namespace API.Controllers
{
    public class MenuController : Controller
    {
        Retorno retorno = new Retorno();
        public JsonResult menujson()
        {
            string obj;
            try
            {

            }
            catch (Exception)
            {
                
            }
            return Json(retorno);
        }

    }
}
