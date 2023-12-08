using Ayana.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyanaTests
{
    [TestClass()]
    public class ProductSalesTests
    {
        // written by : Aida Zametica
        [TestMethod]
        public void ProductSalesID_PropertyTest()
        {
            var productSales = new ProductSales();

            productSales.ProductSalesID = 1;

            Assert.AreEqual(1, productSales.ProductSalesID);
        }

        // written by : Aida Zametica
        [TestMethod]
        public void Product_PropertyTest()
        {
            var productSales = new ProductSales();
            var product = new Product { ProductID = 3 };

            productSales.Product = product;

            Assert.AreEqual(product, productSales.Product);
        }
    }
}