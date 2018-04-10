using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Resources;
using SDMIndonesiaReportsDB.Model;
using SDMIndonesiaReports.Models.CustomModels;
using System.Web.UI.WebControls;
using SDMIndonesiaReports.App_GlobalResources;

namespace SDMIndonesiaReports.Shared
{
    public class SessionContainer
    {
        public static SystemUser User
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.User] as SystemUser; }
            set { HttpContext.Current.Session[SessionVariables_Resource.User] = value; }
        }
        public static int? NumberOfLoginTrials
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.NumberOfLoginTrials] as int?; }
            set { HttpContext.Current.Session[SessionVariables_Resource.NumberOfLoginTrials] = value; }
        }
        public static bool? PreventLogIn
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.PreventLogIn] as bool?; }
            set { HttpContext.Current.Session[SessionVariables_Resource.PreventLogIn] = value; }
        }
        public static List<string> BreadCrumbLinks
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.BreadCrumbLinks] as List<string>; }
            set { HttpContext.Current.Session[SessionVariables_Resource.BreadCrumbLinks] = value; }
        }
        public static List<string> BreadCrumbText
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.BreadCrumbText] as List<string>; }
            set { HttpContext.Current.Session[SessionVariables_Resource.BreadCrumbText] = value; }
        }
        public static string LastPage
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.LastPage] as string; }
            set { HttpContext.Current.Session[SessionVariables_Resource.LastPage] = value; }
        }
        public static string PageStatus
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.PageStatus] as string; }
            set { HttpContext.Current.Session[SessionVariables_Resource.PageStatus] = value; }
        }
        public static List<SDMIndonesiaReports.Models.CustomModels.Menu> Menus
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.Menus] as List<SDMIndonesiaReports.Models.CustomModels.Menu>; }
            set { HttpContext.Current.Session[SessionVariables_Resource.Menus] = value; }
        }
        public static string ActiveTab
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.ActiveTab] as string; }
            set { HttpContext.Current.Session[SessionVariables_Resource.ActiveTab] = value; }
        }
        public static string SelectedMenu
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.SelectedMenu] as string; }
            set { HttpContext.Current.Session[SessionVariables_Resource.SelectedMenu] = value; }
        }
        public static int?[] Parents
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.Parents] as int?[]; }
            set { HttpContext.Current.Session[SessionVariables_Resource.Parents] = value; }
        }
        public static string ParentMenu
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.ParentMenu] as string; }
            set { HttpContext.Current.Session[SessionVariables_Resource.ParentMenu] = value; }
        }
        public static List<string> Success
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.Success] as List<string>; }
            set { HttpContext.Current.Session[SessionVariables_Resource.Success] = value; }
        }
        public static List<string> Errors
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.Errors] as List<string>; }
            set { HttpContext.Current.Session[SessionVariables_Resource.Errors] = value; }
        }
        public static bool? MultiLingual
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.MultiLingual] as bool?; }
            set { HttpContext.Current.Session[SessionVariables_Resource.MultiLingual] = value; }
        }
        public static List<string> Warning
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.Warning] as List<string>; }
            set { HttpContext.Current.Session[SessionVariables_Resource.Warning] = value; }
        }
        public static List<string> CompletedWithErrors
        {
            get { return HttpContext.Current.Session[SessionVariables_Resource.CompletedWithErrors] as List<string>; }
            set { HttpContext.Current.Session[SessionVariables_Resource.CompletedWithErrors] = value; }
        }
    }
}