using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AyanaTests
{
    [TestClass]
    public class DiscountCodeVerifierTests
    {
        private Mock<ApplicationDbContext> _dbContextMock;
        private DiscountCodeVerifier _discountCodeVerifier;

        [TestInitialize]
        public void TestInitialize()
        {
            _dbContextMock = new Mock<ApplicationDbContext>();
            _discountCodeVerifier = new DiscountCodeVerifier(_dbContextMock.Object);
        }

        private Mock<DbSet<T>> GetDbSetMock<T>(List<T> data) where T : class
        {
            var dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return dbSetMock;
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void VerifyDiscountCode_WithExistingCodeAndValidDate_ReturnsTrue()
        {
            var discount = new Discount
            {
                DiscountCode = "postojeci_kod",
                DiscountBegins = DateTime.Now.AddDays(-2),
                DiscountEnds = DateTime.Now.AddDays(2)
            };

            _dbContextMock.Setup(d => d.Discounts).Returns(GetDbSetMock(new List<Discount> { discount }).Object);

            var result = _discountCodeVerifier.VerifyDiscountCode("postojeci_kod");

            Assert.IsTrue(result);
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void GetDiscount_WithNonExistingCode_ReturnsNull()
        {
            var discount = new Discount
            {
                DiscountCode = "postojeci_kod",
                DiscountBegins = DateTime.Now.AddDays(1),
                DiscountEnds = DateTime.Now.AddDays(2)
            };

            _dbContextMock.Setup(d => d.Discounts).Returns(GetDbSetMock(new List<Discount> { discount }).Object);

            var result = _discountCodeVerifier.GetDiscount("nepostojeci_kod");

            Assert.IsNull(result);
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void VerifyDiscountCode_WithNonExistingCode_ReturnsFalse()
        {
            _dbContextMock.Setup(d => d.Discounts).Returns(GetDbSetMock(new List<Discount>()).Object);

            var result = _discountCodeVerifier.VerifyDiscountCode("nepostojeci_kod");

            Assert.IsFalse(result);
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void VerifyExperationDate_WithValidDate_ReturnsTrue()
        {
            var discount = new Discount
            {
                DiscountCode = "postojeci_kod",
                DiscountBegins = DateTime.Now.AddDays(-2),
                DiscountEnds = DateTime.Now.AddDays(2)
            };

            _dbContextMock.Setup(d => d.Discounts).Returns(GetDbSetMock(new List<Discount> { discount }).Object);

            var result = _discountCodeVerifier.VerifyExperationDate("postojeci_kod");

            Assert.IsTrue(result);
        }

        // written by : Lejla Heleg
        [TestMethod]
        public void VerifyExperationDate_WithInvalidDate_ReturnsFalse()
        {
            var discount = new Discount
            {
                DiscountCode = "postojeci_kod",
                DiscountBegins = DateTime.Now.AddDays(1),
                DiscountEnds = DateTime.Now.AddDays(5)
            };

            _dbContextMock.Setup(d => d.Discounts).Returns(GetDbSetMock(new List<Discount> { discount }).Object);

            var result = _discountCodeVerifier.VerifyExperationDate("postojeci_kod");

            Assert.IsFalse(result);
        }
    }
}
