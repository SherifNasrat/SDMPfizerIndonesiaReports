using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDMIndonesiaReports.Models.CustomModels
{
    public class CustomerShareReportVM
    {
        public string Province_Name { get; set; }
        public string City_Name { get; set; }
        public string Brick_Name { get; set; }
        public string Pfizer_Customer_Code { get; set; }
        public string Pfizer_Customer_Name { get; set; }
        public string Dosage_Form_Code { get; set; }
        public string Dosage_Form_Name { get; set; }
        public string Product_Name { get; set; }
        public string Area_Code { get; set; }
        public decimal Percentage { get; set; }
    }
}