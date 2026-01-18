using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Test
{
    public class Tests:TestBase
    {

        [Test]
        public void Test()
        {
            var loginPage = new LoginPage(Driver).Open();

            Assert.That(loginPage.IsLoaded, Is.True);
        }

        [Test]
        public void Test2()
        {
            var loginPage= new LoginPage(Driver).Open();
            var homePage=loginPage.Login("admin@gmail.com", "asd123");

            Assert.That(homePage.IsLoaded(), Is.True);
        }

        [TestCase("Dashboard")]
        [TestCase("Traning")]
        [TestCase("Statistics")]
        public void Test3(string target)
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
