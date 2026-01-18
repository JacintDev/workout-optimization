using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Test.Widgets
{
    public abstract class BaseWidget
    {
        protected IWebDriver Driver;
        protected WebDriverWait Wait;

        protected BaseWidget(IWebDriver driver, WebDriverWait wait)
        {
            Driver = driver;
            Wait = wait;
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

    }
}
