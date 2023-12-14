using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AyanaTests
{
    [TestClass]
    public class ProductsControllerDataDrivenTests
    {
        private Mock<IProduct> iProductMock;
        private Mock<ApplicationDbContext> dbContextMock;
        private ProductsController controller;

        
            //written by: Vedran Mujić
            public static IEnumerable<object[]> GetTestDataCsv()
        {
            string csvFilePath = @"C:\Users\rzyen\OneDrive\Desktop\VVSProject\AyanaTests\TestData\Products.csv";

            foreach (var line in File.ReadLines(csvFilePath).Skip(1))
            {
                var values = line.Split(',');

                int productId = int.Parse(values[0]);
                string productName = values[1];
                string imageUrl = values[2];
                double price = double.Parse(values[3]);
                string flowerType = values[4];
                int stock = int.Parse(values[5]);
                string category = values[6];
                string description = values[7];
                string productType = values[8];

                yield return new object[] { productId, productName, imageUrl, price, flowerType, stock, category, description, productType };
            }
        }

        //written by: Vedran Mujić
        [TestMethod]
        [DynamicData(nameof(GetTestDataCsv), DynamicDataSourceType.Method)]
        public void ProductExists_ShouldReturnTrue_WhenProductExists(int productId, string productName, string imageUrl, double price, string flowerType, int stock, string category, string description, string productType)
        {
            var product = new Product
            {
                ProductID = productId,
                Name = productName,
                ImageUrl = imageUrl,
                Price = price,
                FlowerType = flowerType,
                Stock = stock,
                Category = category,
                Description = description,
                productType = productType
            };

            var productList = new List<Product> { product };

            var dbContextMock = new Mock<ApplicationDbContext>();
            

            var productsDbSetMock = new Mock<DbSet<Product>>();
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productList.GetEnumerator());

            dbContextMock.Setup(d => d.Products).Returns(productsDbSetMock.Object);
            iProductMock = new Mock<IProduct>();

            controller = new ProductsController(dbContextMock.Object, iProductMock.Object);

            var result = controller.ProductExists(product.ProductID);


            Assert.IsTrue(result);
        }
        
        //written by: Almedin Pašalić
        public static IEnumerable<object[]> GetTestData()
        {
            string xmlFilePath = @"C:\Users\rzyen\OneDrive\Desktop\VVSProject\AyanaTests\TestData\Products.xml";

            XDocument doc = XDocument.Load(xmlFilePath);

            foreach (var productElement in doc.Descendants("Product"))
            {
                int productId = int.Parse(productElement.Element("ProductID").Value);
                string productName = productElement.Element("Name").Value;
                string imageUrl = productElement.Element("ImageUrl").Value;
                double price = double.Parse(productElement.Element("Price").Value);
                string flowerType = productElement.Element("FlowerType").Value;
                int stock = int.Parse(productElement.Element("Stock").Value);
                string category = productElement.Element("Category").Value;
                string description = productElement.Element("Description").Value;
                string productType = productElement.Element("ProductType").Value;

                yield return new object[] { productId, productName, imageUrl, price, flowerType, stock, category, description, productType };
            }
        }

        //written by: Almedin Pašalić
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public async Task Details_DataDrivenTest(int productId, string productName, string imageUrl, double price, string flowerType, int stock, string category, string description, string productType)
        {
            var product = new Product
            {
                ProductID = productId,
                Name = productName,
                ImageUrl = imageUrl,
                Price = price,
                FlowerType = flowerType,
                Stock = stock,
                Category = category,
                Description = description,
                productType = productType
            };


            var productList = new List<Product> { product };

            var iProductMock = new Mock<IProduct>();
            var dbContextMock = new Mock<ApplicationDbContext>();

            var productsDbSetMock = new Mock<DbSet<Product>>();
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productList.AsQueryable().Provider);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productList.AsQueryable().Expression);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productList.AsQueryable().ElementType);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productList.GetEnumerator());

            dbContextMock.Setup(d => d.Products).Returns(productsDbSetMock.Object);

            var productsController = new ProductsController(dbContextMock.Object, iProductMock.Object);

            var result = await productsController.Details(product.ProductID);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as Product;

            Assert.IsNotNull(model);
            Assert.AreEqual(product.ProductID, model.ProductID);
        }
    }
}
