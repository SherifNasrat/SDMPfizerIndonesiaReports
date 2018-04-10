using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDMIndonesiaReports.Models.CustomModels
{
    public class BudAndAchByHCRVM
    {
        public string Country { get; set; }
        public string SM { get; set; }
        public string AM { get; set; }
        public string HCR { get; set; }
        public string Product_Group { get; set; }
        public string Product { get; set; }
        public double? Target_QTY { get; set; }
        public decimal? Sales_QTY { get; set; }
        public double? Target_Amount { get; set; }
        public double? Sales_Amount { get; set; }
        public string Percent { get; set; }
    }

}