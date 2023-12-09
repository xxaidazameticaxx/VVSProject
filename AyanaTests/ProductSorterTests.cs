using Ayana.Data;
using Ayana.Models;
using Ayana.Paterni;
using Ayana.Patterns;
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

            
            Assert.AreEqual(mockSortStrategy.Object, pattern.GetSortStrategy()); // Dodajte metodu GetSortStrategy() ako je potrebno
        }
        //written by Vedran Mujić and Almedin Pašalić
        [TestMethod]
        public void Sort_ShouldSortProductsAccordingToStrategy()
        {
            
            var mockAscendingNameSortStrategy = new Mock<ISort>();
            var mockDescendingNameSortStrategy = new Mock<ISort>();
            var mockDefaultSortStrategy = new Mock<ISort>();
            var products = new List<Product> {new Product
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
            } };

           
            mockAscendingNameSortStrategy.Setup(s => s.Sort(products)).Returns(products.OrderBy(p => p.Name).ToList());
            mockDescendingNameSortStrategy.Setup(s => s.Sort(products)).Returns(products.OrderByDescending(p => p.Name).ToList());
            mockDefaultSortStrategy.Setup(s => s.Sort(products)).Returns(products.OrderBy(p => p.Name).ToList());

       
            pattern.SetSortStrategy(mockAscendingNameSortStrategy.Object);
            var sortedProductsAscendingName = pattern.Sort(products, "AscendingName");

            pattern.SetSortStrategy(mockDescendingNameSortStrategy.Object);
            var sortedProductsDescendingName = pattern.Sort(products, "DescendingName");

        
            CollectionAssert.AreEqual(products.OrderBy(p => p.Name).ToList(), sortedProductsAscendingName);
            CollectionAssert.AreEqual(products.OrderByDescending(p => p.Name).ToList(), sortedProductsDescendingName);

           
            pattern.SetSortStrategy(mockDefaultSortStrategy.Object);
            var sortedProductsDefault = pattern.Sort(products, "InvalidSortOption");

            CollectionAssert.AreEqual(products.OrderBy(p => p.Name).ToList(), sortedProductsDefault);
        }
    }
}
