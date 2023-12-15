using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AyanaTests
{
    [TestClass]
    public class ProductEditorTests
    {
        private Mock<ApplicationDbContext> dbContextMock;
        private ProductEditor pattern;
        private List<Product> testData;


        [TestInitialize]
        public void TestInitialize()
        {
            dbContextMock = new Mock<ApplicationDbContext>();
            testData = new List<Product>
        {
            new Product
            {
                ProductID = 1,
                Name = "Product 1",
                ImageUrl = "url1",
                Price = 19.99,
                FlowerType = "Rose",
                Stock = 10,
                Category = "Flowers",
                Description = "Beautiful red rose",
                productType = "Type A"
            },
            new Product
            {
                ProductID = 2,
                Name = "Product 2",
                ImageUrl = "url2",
                Price = 29.99,
                FlowerType = "Lily",
                Stock = 15,
                Category = "Flowers",
                Description = "Elegant white lily",
                productType = "Type B"
            }
        };

            var productsDbSetMock = new Mock<DbSet<Product>>();
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(testData.AsQueryable().Provider);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(testData.AsQueryable().Expression);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(testData.AsQueryable().ElementType);
            productsDbSetMock.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());
            productsDbSetMock.Setup(m => m.Find(It.IsAny<object[]>()))
    .Returns<object[]>(keyValues =>
    {
        var id = (int)keyValues[0];
        return testData.FirstOrDefault(o => o.ProductID == id);
    });

            dbContextMock.Setup(d => d.Products).Returns(productsDbSetMock.Object);
            dbContextMock.Setup(d => d.Update(It.IsAny<Product>()))
    .Callback<Product>(updatedProduct =>
    {
        var productId = updatedProduct.ProductID;
        var updatedName = updatedProduct.Name;
        var updatedImageUrl = updatedProduct.ImageUrl;
        var updatedPrice = updatedProduct.Price;
        var updatedFlowerType = updatedProduct.FlowerType;
        var updatedStock = updatedProduct.Stock;
        var updatedCategory = updatedProduct.Category;
        var updatedDescription = updatedProduct.Description;
        var updatedProductType = updatedProduct.productType;

    });

            dbContextMock.Setup(d => d.SaveChanges())
    .Returns(0);

            pattern = new ProductEditor(dbContextMock.Object);


        }
     
        //written by Vedran Mujić

        [TestMethod]
        public async Task EditNameAndPrice_ShouldUpdateNameAndPriceAndSaveChanges()
        {
            var productId = 1;

            var novi = new Product
            {
                ProductID = 1, 
                Name = "LUDOLINO",
                ImageUrl = "url1",
                Price = 12,
                FlowerType = "Rose",
                Stock = 10,
                Category = "Flowers",
                Description = "Beautiful red rose",
                productType = "Type A"
            };

            await pattern.EditNameAndPrice(productId, novi);

            dbContextMock.Verify(d => d.Update(It.IsAny<Product>()), Times.Once);
            dbContextMock.Verify(d => d.SaveChanges(), Times.Once);

            Assert.AreEqual(novi.Name, testData[0].Name);
            Assert.AreEqual(novi.Price, testData[0].Price);
        }

        //written by Vedran Mujić

        [TestMethod]
        public void GetAllProducts_ShouldReturnAllProducts()
        {
            var result = pattern.GetAllProducts();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count); 
        }

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

        //written by Almedin Pašalić
        [TestMethod]
        [DynamicData(nameof(GetTestDataCsv), DynamicDataSourceType.Method)]
        public void EditAll_ShouldUpdateProductAndSaveChanges(int productId, string productName, string imageUrl, double price, string flowerType, int stock, string category, string description, string productType)
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

            pattern = new ProductEditor(dbContextMock.Object);

            var result = pattern.EditAll(productList[0]);

            
            dbContextMock.Verify(d => d.Update(It.IsAny<Product>()), Times.Once);
            dbContextMock.Verify(d => d.SaveChanges(), Times.Once);

        }

    }
}
