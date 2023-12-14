using Ayana.Models;

namespace AyanaTests
{
    [TestClass()]
    public class ProductSalesTests
    {
        // written by : Aida Zametica
        [TestMethod]
        public void ProductSalesID_IsSet_GetProductSalesIDReturnsCorrectValue()
        {
            var productSales = new ProductSales();

            productSales.ProductSalesID = 1;

            Assert.AreEqual(1, productSales.ProductSalesID);
        }

        // written by : Aida Zametica
        [TestMethod]
        public void Product_IsSet_GetProductReturnsCorrectProduct()
        {
            var productSales = new ProductSales();
            var product = new Product { ProductID = 3 };

            productSales.Product = product;

            Assert.AreEqual(product, productSales.Product);
        }
    }
}