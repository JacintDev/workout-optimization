using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WorkoutOptimization.Test.Tests
{
    public abstract class TestBase
    {
        public IWebDriver Driver { get; private set; }


        [SetUp]
        public void SetUp()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--lang=hu");

            //Chrome blokkolta a tesztet popuppal, a jelszó gyengesége, és netre kijutása miatt
            chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            //chromeOptions.AddArgument("--incognito");

            Driver = new ChromeDriver(chromeOptions);
            Driver.Manage().Window.Maximize();
        }


        [TearDown]
        public void TearDown()
        {
            //ha a teszt elbukik, akkor screen
            var result = TestContext.CurrentContext.Result.Outcome.Status;
            if (result == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string fileName = $"{TestContext.CurrentContext.Test.Name}_" +
                                  $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.png";

                string fullPath = Path.Combine(baseDirectory, fileName);

                screenshot.SaveAsFile(fullPath);

                TestContext.WriteLine($"Screenshot saved: {fullPath}");
            }
            Driver?.Quit();
        }

    }
}
