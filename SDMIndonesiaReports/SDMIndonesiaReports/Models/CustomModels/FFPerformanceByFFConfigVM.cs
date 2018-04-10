using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDMIndonesiaReports.Models.CustomModels
{
    public class FFPerformanceByFFConfigVM
    {
        public string SM { get; set; }
        public string AM { get; set; }
        public string HCR { get; set; }
        public string Team_Name { get; set; }
        public double? Att { get; set; }
        public double? Bud { get; set; }
        public double? Ach { get; set; }
        public string Percent { get; set; }
        public string Perf_null { get; set; }
        public double? MCR { get; set; }
        public double? DCR { get; set; }
        public double? PC { get; set; }
        public double? EC { get; set; }
        public double? ACC { get; set; }
        public double? Pre { get; set; }
        public double? JV { get; set; }
        public double? Perf { get; set; }
    }
}