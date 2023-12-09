using Ayana.Paterni;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyanaTests
{
    [TestClass]
    public class GuestStateTests
    {
        //written by Vedran Mujić and Almedin Pašalić
        [TestMethod]
        public void GetStatus_ShouldReturnGuest()
        {
            // Arrange
            GuestState yourInstance = new GuestState(); 

            // Act
            string result = yourInstance.GetStatus();

            // Assert
            Assert.AreEqual("Guest", result);
        }
    }
}
