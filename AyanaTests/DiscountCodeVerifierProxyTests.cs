using Ayana.Paterni;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class DiscountCodeVerifierProxyTests
    {
        // written by : Lejla Heleg
        [TestMethod]
        public void GetDiscount_ValidDiscountCode_ShouldCallOnce()
        {
            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            var proxy = new DiscountCodeVerifierProxy(discountCodeVerifierMock.Object);
            var discountCode = "kod123";

            var result = proxy.GetDiscount(discountCode);

            discountCodeVerifierMock.Verify(x => x.GetDiscount(discountCode), Times.Once);
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void VerifyDiscountCode_ValidDiscountCode_ShouldCallOnce()
        {
            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            var proxy = new DiscountCodeVerifierProxy(discountCodeVerifierMock.Object);
            var discountCode = "kod123";

            var result = proxy.VerifyDiscountCode(discountCode);

            discountCodeVerifierMock.Verify(x => x.VerifyDiscountCode(discountCode), Times.Once);
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void VerifyExperationDate_ValidDiscountCode_ShouldCallOnce()
        {
            var discountCodeVerifierMock = new Mock<IDiscountCodeVerifier>();
            var proxy = new DiscountCodeVerifierProxy(discountCodeVerifierMock.Object);
            var discountCode = "kod123";

            var result = proxy.VerifyExperationDate(discountCode);

            discountCodeVerifierMock.Verify(x => x.VerifyExperationDate(discountCode), Times.Once);
        }
    }
}
