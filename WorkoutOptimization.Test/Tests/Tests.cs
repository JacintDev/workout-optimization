using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
