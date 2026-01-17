using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Test
{
    public abstract class BasePage
    {
        protected IWebDriver Driver;
        protected WebDriverWait Wait;

        protected BasePage(IWebDriver driver)
        {
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }

        protected IWebElement WaitAndFind(By locator)
        {
            return Wait.Until(d => d.FindElement(locator));
        }

        protected IReadOnlyCollection<IWebElement> WaitAndFindAll(By locator)
        {
            return Wait.Until(d =>
            {
                var elements = d.FindElements(locator);
                return elements.Any() ? elements : null;
            });
        }

        public abstract bool IsLoaded();

    }
}
