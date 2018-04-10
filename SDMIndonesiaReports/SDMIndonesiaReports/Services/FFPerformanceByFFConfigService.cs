using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDMIndonesiaReports.Models.CustomModels;
namespace SDMIndonesiaReports.Services
{
    public class FFPerformanceByFFConfigService : ReportBase
    {
        public List<SelectListItem> getProfiles(int? countryid, int? fromPeriodID, int? toPeriodID)
        {
            
            if (fromPeriodID == null)
            {
                //fromPeriodID = context.Periods.Where(p => p.Year == DateTime.Now.Year).OrderByDescending(p => p.PeriodID).Select(p => p.PeriodID).FirstOrDefault();
                fromPeriodID = GetCurrentPeriod(countryid);
            }
            if (toPeriodID == null)
            {
                toPeriodID = GetCurrentPeriod(countryid);
            }
            var result = context.SP_GetProfileByPeriods(countryid, fromPeriodID, toPeriodID).Select(p => new SelectListItem { Text = p.profilename, Value = p.ProfileId.ToString() }).Distinct().OrderBy(p => p.Text).ToList();

            return result;

        }
        public List<FFPerformanceByFFConfigVM> GetReportData(int? countryID, int? fromPeriodID, int? toPeriodID, string[] profiles, int? positionID)
        {

            string profilesCSV = "";
            if (profiles != null)
            {
                if (profiles[0] == "" || profiles[0] == "null" || profiles[0] == "~")
                    profiles = null;
                else
                    profilesCSV = string.Join(",", profiles);
            }
            if (fromPeriodID == null)
            {
                fromPeriodID = GetCurrentPeriod(countryID);
            }

            if (toPeriodID == null)
            {
                toPeriodID = GetCurrentPeriod(countryID);
            }

            if (positionID == 1 || positionID == null)
            {
                var data = context.SP_FFPerformanceByFFConfig(countryID, fromPeriodID, toPeriodID, profilesCSV).Select(m => new FFPerformanceByFFConfigVM
                {
                    SM = m.SM,
                    AM = m.AM,
                    HCR = m.HCR,
                    Team_Name = m.Team,
                    Att = Math.Round((double)m.ATT,4),
                    Bud = Math.Round((double)m.Bud,4),
                    Ach = Math.Round((double)m.Ach,4),
                    Percent = (m.Bud == 0 ? "" : Math.Round((double)((m.Ach / m.Bud)*100),4).ToString()),
                    Perf_null = (m.Perf2 == null ? "" : Math.Round((double)((m.Perf2) * 100), 4).ToString()),
                    MCR = Math.Round((double)m.MCR,4),
                    DCR = Math.Round((double)m.DCR,4),
                    PC = Math.Round((double)m.PC,4),
                    EC = Math.Round((double)m.EC,4),
                    ACC = Math.Round((double)m.Acc,4),
                    Pre = Math.Round((double)m.Pre,4),
                    JV = Math.Round((double)m.JV,4),
                    Perf = Math.Round((double)m.Perf,4)
                }).OrderByDescending(m => m.HCR).AsQueryable();
                return data.ToList();
            }
            else if (positionID == 2)
            {
                var data = context.SP_FFPerformanceByFFConfig_AM(countryID, fromPeriodID, toPeriodID, profilesCSV).Select(m => new FFPerformanceByFFConfigVM
                {
                    SM = m.SM,
                    AM = m.AM,
                    HCR = m.SM,
                    Team_Name = m.SM,
                    Att = Math.Round((double)m.ATT, 4),
                    Bud = Math.Round((double)m.Bud, 4),
                    Ach = Math.Round((double)m.Ach, 4),
                    Percent = (m.Bud == 0 ? "" : Math.Round((double)((m.Ach / m.Bud) * 100), 4).ToString()),
                    Perf_null = (m.Perf2 == null ? "" : Math.Round((double)((m.Perf2) * 100), 4).ToString()),
                    MCR = Math.Round((double)m.MCR, 4),
                    DCR = Math.Round((double)m.DCR, 4),
                    PC = Math.Round((double)m.PC, 4),
                    EC = Math.Round((double)m.EC, 4),
                    ACC = Math.Round((double)m.Acc, 4),
                    Pre = Math.Round((double)m.Pre, 4),
                    JV = Math.Round((double)m.JV, 4),
                    Perf = Math.Round((double)m.Perf, 4)
                });
                return data.OrderByDescending(m => m.AM).AsQueryable().ToList();
            }
            else
            {
                var data = context.SP_FFPerformanceByFFConfig_SM(countryID, fromPeriodID, toPeriodID, profilesCSV).Select(m => new FFPerformanceByFFConfigVM
                {
                    SM = m.SM,
                    AM = m.SM,
                    HCR = m.SM,
                    Team_Name = m.SM,
                    Att = Math.Round((double)m.ATT, 4),
                    Bud = Math.Round((double)m.Bud, 4),
                    Ach = Math.Round((double)m.Ach, 4),
                    Percent = (m.Bud == 0 ? "" : Math.Round((double)((m.Ach / m.Bud) * 100), 4).ToString()),
                    Perf_null = (m.Perf2 == null ? "" : Math.Round((double)((m.Perf2) * 100), 4).ToString()),
                    MCR = Math.Round((double)m.MCR, 4),
                    DCR = Math.Round((double)m.DCR, 4),
                    PC = Math.Round((double)m.PC, 4),
                    EC = Math.Round((double)m.EC, 4),
                    ACC = Math.Round((double)m.Acc, 4),
                    Pre = Math.Round((double)m.Pre, 4),
                    JV = Math.Round((double)m.JV, 4),
                    Perf = Math.Round((double)m.Perf, 4)
                });
                return data.OrderByDescending(m => m.SM).AsQueryable().ToList();
            }



        }
    }
}