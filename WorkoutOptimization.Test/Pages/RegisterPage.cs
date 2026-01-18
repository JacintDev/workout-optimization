using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Test.Pages
{
    public class RegisterPage : BasePage
    {
        public RegisterPage(IWebDriver driver) : base(driver)
        {
        }

        private IWebElement WelcomeText => WaitAndFind(By.XPath("//h2[contains(text(),'Regisztráció')]"));
        public override bool IsLoaded()
        {
            return WelcomeText.Displayed;
        }
    }
}
