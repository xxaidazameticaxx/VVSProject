using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
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
using System.Xml.Serialization;

namespace AyanaTests
{
    [TestClass]
    public class ProductSorterTests
    {
        private Mock<ApplicationDbContext> dbContextMock;
        private ProductSorter pattern;
        private List<Product> testData;
  

        [TestInitialize]
        public void Setup()
        {
            pattern = new ProductSorter();
        }

        //written by Vedran Mujić and Almedin Pašalić
        [TestMethod]
        public void SetSortStrategy_ShouldSetSortStrategy()
        {
            var mockSortStrategy = new Mock<ISort>();

            pattern.SetSortStrategy(mockSortStrategy.Object);
            
            Assert.AreEqual(mockSortStrategy.Object, pattern.GetSortStrategy()); 
        }
        

        //written by Vedran Mujič
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

        //written by Vedran Mujič
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public async Task Sort_DataDrivenTest(int productId, string productName, string imageUrl, double price, string flowerType, int stock, string category, string description, string productType)
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

             
            var resultAsc = pattern.Sort(productList, "AscendingName");
            var resultDsc = pattern.Sort(productList, "DescendingName");
            var resultInvalid = pattern.Sort(productList, "InvalidSortOption");


            CollectionAssert.AreEqual(productList.OrderBy(p => p.Name).ToList(), resultAsc);
            CollectionAssert.AreEqual(productList.OrderByDescending(p => p.Name).ToList(), resultDsc);
            CollectionAssert.AreEqual(productList.OrderBy(p => p.Name).ToList(), resultInvalid);
        }
    }
}
