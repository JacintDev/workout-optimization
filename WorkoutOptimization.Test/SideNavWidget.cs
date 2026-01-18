using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Test
{
    public class SideNavWidget : BaseWidget
    {
        public SideNavWidget(IWebDriver driver, WebDriverWait wait) : base(driver, wait) {}

        private IWebElement DashboardMenuButton => WaitAndFind(By.XPath("//li[contains(.,'Irányítópult')]"));
        private IWebElement TrainingMenuButton => WaitAndFind(By.XPath("//li[contains(.,'Edzések')]"));
        private IWebElement StatisticsMenuButton => WaitAndFind(By.XPath("//li[contains(.,'Statisztika')]"));



        public HomePage OpenDashboard()
        {
            DashboardMenuButton.Click();
            return new HomePage(Driver);
        }

        public TrainingPage OpenTraining()
        {
            TrainingMenuButton.Click();
            return new TrainingPage(Driver);
        }

        public StatisticsPage OpenStatistics()
        {
            StatisticsMenuButton.Click();
            return new StatisticsPage(Driver);
        }

    }
}
