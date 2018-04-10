using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using System.Text;
using SDMIndonesiaReportsDB;
using SDMIndonesiaReportsDB.Model;
using Microsoft.Web;
using System.Web.WebPages.Deployment.Resources;
using SDMIndonesiaReports.Shared;
using System.DirectoryServices;
using Microsoft.Web.WebPages.OAuth;
using SDMIndonesiaReports.App_GlobalResources;
using SDMIndonesiaReports.Helpers;



namespace SDMIndonesiaReports.Controllers
{
    //[Authorize]
    //[InitializeSimpleMembership]
    public class AccountController : Controller
    {
        SDW_TargetingEntities db = new SDW_TargetingEntities();
        //
        // GET: /Account/Login

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(SystemUser user)
        {
            if (IsValid(user))
            {
                MenusInitializer myMenus = new MenusInitializer();
                SessionContainer.Menus = myMenus.ProjectMenus.OrderBy(m => m.MenuName).ToList();
                return RedirectToAction(PageActions.Index, "Home");

            }

            else
            {
                ModelState.AddModelError("", Account_Resource.LoginFailed);
                return View(user);
            }
        }
        private bool IsValid(SystemUser user)
        {
            var myUser = new SystemUser();
            try
            {

                var UseLDAP = bool.Parse(ConfigurationManager.AppSettings["UseLDAP"]);

                if (!UseLDAP)
                {
                    myUser = LogmeIn(user.UserName, user.Password);
                }
                else
                {
                    string ldapPath = "";
                    try
                    {
                        ldapPath = ConfigurationManager.AppSettings["LDAPPath"];
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    bool IsLDAPAuthenticated = IsAuthenticated(user.UserName, user.Password, ldapPath);
                    if (IsLDAPAuthenticated)

                    //var username = Login1.UserName;
                    //var password = Login1.Password;
                    //IntPtr hToken;
                    // if (WindowsLogon.LogonUser(username, Ldap.PreWindows2000DomainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT, out hToken))
                    {
                        myUser = LogmeIn(user.UserName);//, Login1.Password);

                        //Session["ImpersonationContext"] = WindowsIdentity.Impersonate(hToken);
                        //WindowsLogon.CloseHandle(hToken);
                    }
                    else
                    {
                        myUser = null;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            if (myUser != null)
            {
                SessionContainer.User = myUser;
                return true;
            }
            else
            {
                //if (SessionContainer.NumberOfLoginTrials == null)
                //    SessionContainer.NumberOfLoginTrials = 1;
                //else
                //    SessionContainer.NumberOfLoginTrials = SessionContainer.NumberOfLoginTrials + 1;

                //if (SessionContainer.NumberOfLoginTrials >= int.Parse(ConfigurationManager.AppSettings["MaximumLoginTrials"]))
                //    SessionContainer.PreventLogIn = true;
                return false;
            }
        }
        private SystemUser LogmeIn(string userName)
        {
            var user = db.SystemUsers.FirstOrDefault(s => s.UserName == userName && s.Active);
            /// TODO: Add password Filter
            if (user != null)//&& Utility.CompareHash(Password, suc.Password))
            {
                return user;
            }
            return null;

        }

        private SystemUser LogmeIn(string userName, string password)
        {
            var user = db.SystemUsers.FirstOrDefault(s => s.UserName == userName && s.Active);
            /// TODO: Add password Filter
            if (user != null && CompareHash(password, user.Password))
            {
                return user;
            }
            return null;

        }
        private static bool CompareHash(string input, string hashed)
        {
            string inputHash = GetHashedString(input);

            return (string.CompareOrdinal(inputHash, hashed)) == 0;
        }
        private static string GetHashedString(string text)
        {
            UTF8Encoding encoder = new UTF8Encoding();

            Byte[] hashedDataBytes = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(encoder.GetBytes(text));

            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < hashedDataBytes.Length; i++)
            {
                sBuilder.Append(hashedDataBytes[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        private static bool IsAuthenticated(string username, string password, string ldapPath)
        {
            using (DirectoryEntry deDirEntry = new DirectoryEntry(ldapPath, username, password, AuthenticationTypes.Secure))
            {
                try
                {
                    Object obj = deDirEntry.NativeObject;

                    DirectorySearcher search = new DirectorySearcher(deDirEntry);


                    search.Filter = "(SAMAccountName=" + username + ")";


                    search.PropertiesToLoad.Add("cn");


                    SearchResult result = search.FindOne();


                    if (null == result)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }
            }

        }
        [HttpGet]
        public ActionResult Lock()
        {
            Session.Add(SessionVariables_Resource.State, "Lock");
            SessionContainer.PageStatus = "Lock";
            SessionContainer.LastPage = Request.RawUrl;
            return View();
        }
        [HttpPost]
        public void LockLogin(SystemUser user)
        {
            if (SessionContainer.User != null)
            {
                string userName = SessionContainer.User.UserName;
                user.UserName = userName;
                if (IsValid(user))
                {
                    SessionContainer.PageStatus = null;
                    Response.Redirect("~/Home/Index");
                }
                else
                {
                    ModelState.AddModelError("", Account_Resource.LoginFailed);
                    Response.Redirect("~/Account/Lock");
                }
            }
            else
                Response.Redirect("~/Account/Login");
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            //return RedirectToAction(PageActions.Login, Pages_Resource.Account);
            return RedirectToAction(PageActions.Login, "Account");
        }
        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}