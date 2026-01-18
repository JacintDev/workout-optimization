using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Test
{
    public class TrainingPage : BasePage
    {
        public TrainingPage(IWebDriver driver) : base(driver) { }

        private IWebElement WelcomeText => WaitAndFind(By.ClassName("table-title"));

        public override bool IsLoaded()
        {
            return WelcomeText.Displayed;
        }
    }
}
