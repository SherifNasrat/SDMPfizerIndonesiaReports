using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDMIndonesiaReports.Models.CustomModels
{
    public class Menu
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string MenuDescription { get; set; }
        public string ArabicMenuName { get; set; }
        public string IconClass { get; set; }
        public int? FK_ParentID { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}