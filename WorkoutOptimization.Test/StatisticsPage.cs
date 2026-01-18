using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Test
{
    public class StatisticsPage : BasePage
    {
        public StatisticsPage(IWebDriver driver) : base(driver)
        {
        }

        private IWebElement WelcomeText => WaitAndFind(By.ClassName("table-title"));

        public override bool IsLoaded()
        {
            return WelcomeText.Displayed;
        }
    }
}
