using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDMIndonesiaReports.Models.CustomModels
{
    public class HcrPerformaceByProductVM
    {
        public int Profile { get; set; }
        public string Hcr_Name { get; set; }
        public string Team_Name { get; set; }
        public string Product { get; set; }
        public int? Attendance { get; set; }
        public double? Bud { get; set; }
        public double? Ach { get; set; }
        public string Ach_Percent { get; set; }
    }
}