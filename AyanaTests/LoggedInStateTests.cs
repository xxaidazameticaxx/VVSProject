using Ayana.Paterni;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyanaTests
{
    [TestClass]
    public class LoggedInStateTests
    {

        private LoggedInState pattern;

        [TestInitialize]
        public void Setup()
        {
            pattern = new LoggedInState();
        }

        //written by Vedran Mujić

        [TestMethod]
        public void GetStatus_ShouldReturnLoggedIn()
        {
            // Act
            string status = pattern.GetStatus();

            // Assert
            Assert.AreEqual("Logged In", status);
        }

        //written by Vedran Mujić

        [TestMethod]
        public void CanPurchase_ShouldReturnTrue()
        {
            // Act
            bool canPurchase = pattern.CanPurchase();

            // Assert
            Assert.IsTrue(canPurchase);
        }

        //written by Vedran Mujić

        [TestMethod]
        public void CanView_ShouldReturnTrue()
        {
            // Act
            bool canView = pattern.CanView();

            // Assert
            Assert.IsTrue(canView);
        }
    }
}
