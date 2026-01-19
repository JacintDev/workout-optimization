using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Models.Models;

namespace WorkoutOptimization.Test.Pages
{
    public class RegisterPage : BasePage
    {
        private const string URL = "http://141.147.9.110:4200/register";

        public RegisterPage(IWebDriver driver) : base(driver)
        {
        }
        private IWebElement SnackBarMessage => WaitAndFind(By.CssSelector(".mdc-snackbar__label, .mat-mdc-snack-bar-label"));
        private IWebElement WelcomeText => WaitAndFind(By.XPath("//h2[contains(text(),'Regisztráció')]"));
        private IWebElement EmailInput => WaitAndFind(By.CssSelector("[data-testid='register-email']"));
        private IWebElement PasswordInput => WaitAndFind(By.CssSelector("[data-testid='register-password']"));
        private IWebElement FirstNameInput => WaitAndFind(By.CssSelector("[data-testid='register-first-name']"));
        private IWebElement LastNameInput => WaitAndFind(By.CssSelector("[data-testid='register-last-name']"));
        private IWebElement SexSelector => WaitAndFind(By.CssSelector("[data-testid='register-sex']"));
        private IWebElement SubmitButton => WaitAndFind(By.CssSelector("[data-testid='register-submit']"));


        public override bool IsLoaded()
        {
            return WelcomeText.Displayed;
        }

        public bool GetErrorMessage(string errorMessage)
        {
            return SnackBarMessage.Text.Contains(errorMessage);
        }

        public LoginPage Registration(RegisterModel reg)
        {
            EmailInput.SendKeys(reg.Email);
            PasswordInput.SendKeys(reg.Password);
            FirstNameInput.SendKeys(reg.FirstName);
            LastNameInput.SendKeys(reg.LastName);
            SelectSex(reg.Sex);
            SubmitButton.Click();
            return new LoginPage(Driver);
        }

        private void SelectSex(Sex sexValue)
        {
            SexSelector.Click();
            var optionToClick = WaitAndFind(By.CssSelector($"mat-option[value='{(int)sexValue}']"));
            optionToClick.Click();

        }

        public RegisterPage Open()
        {
            Driver.Navigate().GoToUrl(URL);
            return this;
        }
    }
}
