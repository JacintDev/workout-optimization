using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutOptimization.Models.Models;
using WorkoutOptimization.Test.Factories;
using WorkoutOptimization.Test.Pages;

namespace WorkoutOptimization.Test.Tests
{
    public class Tests: TestBase
    {

        [Test]
        public void Login_Entering_PageIsLoaded()
        {
            var loginPage = new LoginPage(Driver).Open();

            Assert.That(loginPage.IsLoaded, Is.True);
        }

        [Test]
        public void Login_ShouldWork_WithValidCredentials()
        {
            var loginPage= new LoginPage(Driver).Open();
            var homePage=loginPage.Login("admin@gmail.com", "asd123");

            Assert.That(homePage.IsLoaded(), Is.True);
        }

        [Test]
        public void Login_ShouldFail_WithInvalidCredentials()
        {
            var loginPage = new LoginPage(Driver).Open();
            loginPage.Login("admin@gmail.com", "asd1234");

            Assert.That(loginPage.GetErrorMessage("Sikertelen bejelentkezés!"), Is.True);
        }


        [Test]
        public void Login_AlreadyLoggedin_RedirectsToHome()
        {
            var loginPage = new LoginPage(Driver).Open();
            var homePage = loginPage.Login("admin@gmail.com", "asd123");
            homePage.WaitForUrlContains("/home");
            loginPage.Open();
            homePage.WaitForUrlContains("/home");

            Assert.That(homePage.IsAt(), Is.True);
            
        }

        [Test]
        public void Register_ShouldWork_WithValidCredentials()
        {
            var registerPage = new RegisterPage(Driver).Open();
            
            var regUser= UserFactory.CreateValidUser();

            var loginPage= registerPage.Registration(regUser);

            Assert.That(loginPage.IsLoaded(), Is.True);

        }

        [Test]
        public void Register_ShouldFail_WithAlreadyExistsCredentials()
        {
            var registerPage = new RegisterPage(Driver).Open();

            var regUser = UserFactory.CreateValidUser();
            regUser.Email = "admin@gmail.com";

            registerPage.Registration(regUser);

            Assert.That(registerPage.GetErrorMessage("Failed to register User with this email already exists"), Is.True);

        }




        [TestCase("Dashboard")]
        [TestCase("Traning")]
        [TestCase("Statistics")]
        public void Home_SideBarNavigation_PageIsLoaded(string target)
        {
            var loginPage = new LoginPage(Driver).Open();
            var homePage = loginPage.Login("admin@gmail.com", "asd123");

            
            BasePage resultPage= target switch
            {
                "Dashboard"=> homePage.SideNavWidget.OpenDashboard(),
                "Traning" => homePage.SideNavWidget.OpenTraining(),
                "Statistics" => homePage.SideNavWidget.OpenStatistics(),
                _ => throw new ArgumentOutOfRangeException()
            };

            Assert.That(resultPage.IsLoaded(), Is.True);
            
        }

    }
}
