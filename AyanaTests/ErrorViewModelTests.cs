using Ayana.Models;

namespace AyanaTests
{
    [TestClass]
    public class ErrorViewModelTests
    {
        // written by : Aida Zametica
        [TestMethod]
        public void RequestId_SetAndGet_ReturnsCorrectValue()
        {

            var errorViewModel = new ErrorViewModel();

            var expectedRequestId = "123";

            errorViewModel.RequestId = expectedRequestId;

            var actualRequestId = errorViewModel.RequestId;

            Assert.AreEqual(expectedRequestId, actualRequestId);
        }

        // written by : Aida Zametica
        [TestMethod]
        public void ShowRequestId_RequestIdIsSet_ReturnsTrue()
        {
 
            var errorViewModel = new ErrorViewModel();

            errorViewModel.RequestId = "123";

            var showRequestId = errorViewModel.ShowRequestId;

            Assert.IsTrue(showRequestId);
        }

        // written by : Aida Zametica
        [TestMethod]
        public void ShowRequestId_RequestIdNotSet_ReturnsFalse()
        {

            var errorViewModel = new ErrorViewModel();

            var showRequestId = errorViewModel.ShowRequestId;

            Assert.IsFalse(showRequestId);
        }

    }
}
