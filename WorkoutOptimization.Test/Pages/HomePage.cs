using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Test.Widgets;

namespace WorkoutOptimization.Test.Pages
{
    public class HomePage : BasePage
    {

        public SideNavWidget SideNavWidget { get; set; }

        public HomePage(IWebDriver driver) : base(driver) {
            SideNavWidget=new SideNavWidget(driver,Wait);
        }

        private IWebElement WelcomeText => WaitAndFind(By.XPath("//p[contains(text(),'👋Üdvözöllek')]"));

        public override bool IsLoaded()
        {
            return WelcomeText.Displayed;
        }
    }
}
