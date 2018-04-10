using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDMIndonesiaReports.Models.CustomModels
{
    public class SalesTeamVM
    {

        public List<string> SalesTeamNames { get; set; }
        public int? countryID { get; set; }
        public int? fromPeriodID { get; set; }
        public int? toPeriodID { get; set; }



    }
}