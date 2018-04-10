using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDMIndonesiaReports.Models.CustomModels
{
    public class TransactionDetailsVM
    {
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }
        public string Invoice { get; set; }
        public string Product_Name { get; set; }
        public string Cust_Code { get; set; }
        public string Cust_Name { get; set; }
        public string Sector { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Brick { get; set; }
        public string SM { get; set; }
        public string AM { get; set; }
        public string HCR { get; set; }
        public string Profile { get; set; }
        public decimal Qty { get; set; }
        public double? Value { get; set; }
    }
}