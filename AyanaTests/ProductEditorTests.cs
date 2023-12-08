using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Patterns;
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
        // Pratite promjene na atributima objekta Product
        var productId = updatedProduct.ProductID;
        var updatedName = updatedProduct.Name;
        var updatedImageUrl = updatedProduct.ImageUrl;
        var updatedPrice = updatedProduct.Price;
        var updatedFlowerType = updatedProduct.FlowerType;
        var updatedStock = updatedProduct.Stock;
        var updatedCategory = updatedProduct.Category;
        var updatedDescription = updatedProduct.Description;
        var updatedProductType = updatedProduct.productType;

        // Ovdje možete dodati logiku prema potrebi ili pratiti promjene.
    });

            dbContextMock.Setup(d => d.SaveChanges())
    .Returns(0);

            pattern = new ProductEditor(dbContextMock.Object);


        }
        //written by Vedran Mujić

        [TestMethod]
        public async Task EditAll_ShouldUpdateProductAndSaveChanges()
        {
            

            
            await pattern.EditAll(testData[0]);

            // Assert
            dbContextMock.Verify(d => d.Update(It.IsAny<Product>()), Times.Once);
            dbContextMock.Verify(d => d.SaveChanges(), Times.Once);
        }
        //written by Vedran Mujić

        [TestMethod]
        public async Task EditNameAndPrice_ShouldUpdateNameAndPriceAndSaveChanges()
        {
            // Arrange
            var productId = 1;

            var novi = new Product
            {
                ProductID = 1, // Promijenite ID kako biste odabrali postojeći proizvod
                Name = "LUDOLINO",
                ImageUrl = "url1",
                Price = 12,
                FlowerType = "Rose",
                Stock = 10,
                Category = "Flowers",
                Description = "Beautiful red rose",
                productType = "Type A"
            };

            // Act
            await pattern.EditNameAndPrice(productId, novi);

            // Assert
            dbContextMock.Verify(d => d.Update(It.IsAny<Product>()), Times.Once);
            dbContextMock.Verify(d => d.SaveChanges(), Times.Once);

            // Verify that the existing product is updated with the new name and price
            Assert.AreEqual(novi.Name, testData[0].Name);
            Assert.AreEqual(novi.Price, testData[0].Price);
            // Ostali atributi također mogu biti uspoređeni ako je potrebno
        }

        //written by Vedran Mujić

        [TestMethod]
        public void GetAllProducts_ShouldReturnAllProducts()
        {
           

            // Act
            var result = pattern.GetAllProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count); // Promijenite broj prema vašem stvarnom testnom skupu podataka
                                              // Dodatne provjere po potrebi
        }


    }
}
