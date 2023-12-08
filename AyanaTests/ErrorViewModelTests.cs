using Ayana.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyanaTests
{
    [TestClass]
    public class ErrorViewModelTests
    {
        // written by : Aida Zametica
        [TestMethod]
        public void RequestId_SetAndGet()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel();
            var expectedRequestId = "123";

            // Act
            errorViewModel.RequestId = expectedRequestId;
            var actualRequestId = errorViewModel.RequestId;

            // Assert
            Assert.AreEqual(expectedRequestId, actualRequestId);
        }

        // written by : Aida Zametica
        [TestMethod]
        public void ShowRequestId_TrueWhenRequestIdSet()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel();
            errorViewModel.RequestId = "123";

            // Act
            var showRequestId = errorViewModel.ShowRequestId;

            // Assert
            Assert.IsTrue(showRequestId);
        }

        // written by : Aida Zametica
        [TestMethod]
        public void ShowRequestId_FalseWhenRequestIdNotSet()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel();

            // Act
            var showRequestId = errorViewModel.ShowRequestId;

            // Assert
            Assert.IsFalse(showRequestId);
        }

    }
}
