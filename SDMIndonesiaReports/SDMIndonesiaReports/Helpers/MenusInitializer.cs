using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDMIndonesiaReports.Models.CustomModels;

namespace SDMIndonesiaReports.Helpers
{
    public class MenusInitializer
    {
        public List<Menu> ProjectMenus { get; set; }
        public MenusInitializer()
        {
            //Initialize Menus for the Project
            ProjectMenus = new List<Menu>();
            //FFConfig Report Menu
            ProjectMenus.Add(new Menu()
            {
                MenuID = 1,
                MenuName = "FF Config Report",
                ArabicMenuName = "",
                MenuDescription = "View FF Config Report",
                ControllerName = "FFConfig",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null

            });
            //End of FFConfig

            //Customer Share Report Menu
            ProjectMenus.Add(new Menu()
            {
                MenuID = 2,
                MenuName = "Customer Share Report",
                ArabicMenuName = "",
                MenuDescription = "View Customer Share Report",
                ControllerName = "CustomerShareReport",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null

            });


            //Territoty to Brick by Area Report Menu
            ProjectMenus.Add(new Menu()
            {
                MenuID = 3,
                MenuName = "Territory to Brick by Area Report",
                ArabicMenuName = "",
                MenuDescription = "View Territory to brick by area",
                ControllerName = "TerritoryToBrickByArea",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null
            });

            //Team To Product Report Menu
            ProjectMenus.Add(new Menu()
            {
                MenuID = 4,
                MenuName = "Team to Product Report",
                ArabicMenuName = "",
                MenuDescription = "View Team To Product",
                ControllerName = "TeamToProduct",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null
            });
            //Territoty to Brick by Brick Report Menu
            ProjectMenus.Add(new Menu()
            {
                MenuID = 5,
                MenuName = "Territory to Brick by Brick Report",
                ArabicMenuName = "",
                MenuDescription = "View Territory to Brick by Brick",
                ControllerName = "TerritoryToBrickByBrick",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null

            });
            ProjectMenus.Add(new Menu()
            {
                MenuID = 6,
                MenuName = "Transaction Details Report",
                ArabicMenuName = "",
                MenuDescription = "View Transaction Details",
                ControllerName = "TransactionDetails",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null

            }); 
            ProjectMenus.Add(new Menu()
            {
                MenuID = 7,
                MenuName = "HCR Performance By Product Report",
                ArabicMenuName = "",
                MenuDescription = "View Hcr Performance By Product ",
                ControllerName = "HcrPerformanceByProduct",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null

            });
            ProjectMenus.Add(new Menu()
            {
                MenuID = 8,
                MenuName = "Bud And Ach By HCR Report",
                ArabicMenuName = "",
                MenuDescription = "View Bud And Ach By HCR ",
                ControllerName = "BudAndAchByHCR",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null

            });
            ProjectMenus.Add(new Menu()
            {
                MenuID = 9,
                MenuName = "Performance By FF Config Report",
                ArabicMenuName = "",
                MenuDescription = "View Performance By FF Config ",
                ControllerName = "FFPerformanceByFFConfig",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null

            });
            ProjectMenus.Add(new Menu()
            {
                MenuID = 10,
                MenuName = "Transaction Summary by Channel",
                ArabicMenuName = "",
                MenuDescription = "View Transaction Summary by Channel",
                ControllerName = "TransactionSummaryByChannel",
                ActionName = "Index",
                IconClass = "",
                FK_ParentID = null

            });
        }
    }
}