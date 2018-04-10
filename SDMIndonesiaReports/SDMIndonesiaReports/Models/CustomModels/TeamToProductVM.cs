using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDMIndonesiaReports.Models.CustomModels
{
    public class TeamToProductVM
    {
        public string Team_Code { get; set; }
        public string Team_Name { get; set; }
        public string AP { get; set; }
        public int Year { get; set; }
        public short? Tier { get; set; }
        public string Product_Group { get; set; }
        public string Product { get;set; }
    }
}