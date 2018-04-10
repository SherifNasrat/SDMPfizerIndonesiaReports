using SDMIndonesiaReports.Models;
using SDMIndonesiaReports.Shared;
using SDMIndonesiaReportsDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDMIndonesiaReports.Controllers
{
    public class HomeController : Controller
    {
        SDW_TargetingEntities db = new SDW_TargetingEntities();

        public ActionResult Index()
        {

            return View();
            
        }


    }
}