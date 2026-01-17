using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Test
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver driver) : base(driver) {}

        private IWebElement WelcomeText => WaitAndFind(By.XPath("//p[contains(text(),'👋Üdvözöllek')]"));

        public override bool IsLoaded()
        {
            return WelcomeText.Displayed;
        }
    }
}
