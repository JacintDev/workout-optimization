using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Test.Pages
{
    public class LoginPage: BasePage
    {

        private const string URL = "http://141.147.9.110:4200/login";
        public LoginPage(IWebDriver driver) : base(driver) { }

        private IWebElement WelcomeText => WaitAndFind(By.XPath("//h2[contains(text(),'Bejelentkezés')]"));

        private IWebElement EmailInput => WaitAndFind(By.Id("mat-input-0")); 
        private IWebElement PasswordInput => WaitAndFind(By.Id("mat-input-1"));
        private IWebElement LoginButton => WaitAndFind(By.XPath("//button[contains(., 'Bejelentkezés')]"));

        public LoginPage Open()
        {
            Driver.Navigate().GoToUrl(URL);
            return this;
        }

        public override bool IsLoaded()
        {
            return WelcomeText.Displayed;
        }

        private void EnterEmail(string email)
        {
            EmailInput.Clear();
            EmailInput.SendKeys(email);
        }
        private void EnterPassword(string password)
        {
            PasswordInput.Clear();
            PasswordInput.SendKeys(password);
        }

        public void ClickLogin()
        {
            LoginButton.Click();
        }

        public HomePage Login(string email, string password)
        {
            EnterEmail(email);
            EnterPassword(password);
            ClickLogin();

            return new HomePage(Driver);


        }
    }
}
