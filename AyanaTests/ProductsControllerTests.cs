using System.Linq.Expressions;
using System.Security.Claims;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Patterns;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Xml.Linq;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace AyanaTests
{
    [TestClass]
    public class ProductsControllerTests
    {
        private Mock<IProduct> iProductMock;
        private Mock<ApplicationDbContext> dbContextMock;
        private ProductsController controller;
        private List<Product> testData;

        private Product testProduct = new Product
        {
            ProductID = 3,
            Name = "Product 1",
            ImageUrl = "url1",
            Price = 19.99,
            FlowerType = "Rose",
            Stock = 10,
            Category = "Flowers",
            Description = "Beautiful red rose",
            productType = "Type A"
        };

        //written by: Almedin Pašalić
        [TestInitialize]
        public void TestInitialize()
        {
            iProductMock = new Mock<IProduct>();
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

            productsDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(async keyValues =>
                {
                    var id = (int)keyValues[0];
                    return await Task.FromResult(testData.FirstOrDefault(o => o.ProductID == id));
                });

            dbContextMock.Setup(d => d.Products).Returns(productsDbSetMock.Object);

            dbContextMock.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0));

            dbContextMock.Setup(d => d.Add(It.IsAny<Product>()))
                .Callback<Product>(product => testData.Add(product));

            dbContextMock.Setup(d => d.Remove(It.IsAny<Product>()))
                .Callback<Product>(product => testData.Remove(product));

            iProductMock.Setup(editor => editor.EditAll(It.IsAny<Product>()))
                .Callback<Product>(editedProduct =>
                {
                    var existingProduct = testData.FirstOrDefault(p => p.ProductID == editedProduct.ProductID);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = editedProduct.Name;
                        existingProduct.Price = editedProduct.Price;
                    }
                    else
                    {
                        throw new DbUpdateConcurrencyException();
                    }
                });

            iProductMock.Setup(editor => editor.EditNameAndPrice(It.IsAny<int>(), It.IsAny<Product>()))
                .Callback<int, Product>((productId, editedProduct) =>
                {
                    var existingProduct = testData.FirstOrDefault(p => p.ProductID == productId);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = editedProduct.Name;
                        existingProduct.Price = editedProduct.Price;
                    }
                    else
                    {
                        throw new DbUpdateConcurrencyException();
                    }
                });

            controller = new ProductsController(dbContextMock.Object, iProductMock.Object);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public async Task Index_ReturnsViewWithProducts()
        {
            var result = await controller.Index();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void Create_ReturnsView()
        {
            var result = controller.Create();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void SearchResult_ReturnsViewWithFilteredProducts()
        {
            var searchString = "Product 1"; 
            var expectedResultCount = 1; 

            var result = controller.SearchResult(searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void SearchResult_ReturnsViewWithAllProductsWhenSearchIsNull()
        {
            var result = controller.SearchResult(null);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            CollectionAssert.AreEqual(testData, model);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await controller.Details(null);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenProductIsNull()
        {
            var result = await controller.Details(4);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void PopularSearches_ReturnsViewWithFilteredProducts()
        {
            var searchString = "BAM.25"; 
            var expectedResultCount = 1; 

            var result = controller.PopularSearches(searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void PopularSearches_ReturnsViewWithCategoryProducts()
        {
            var searchString = "Flowers"; 
            var expectedResultCount = 2;

            var result = controller.PopularSearches(searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void PopularSearches_ReturnsViewWithFlowerTypeProducts()
        {
            var searchString = "Rose"; 
            var expectedResultCount = 1; 

            var result = controller.PopularSearches(searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void PopularSearches_ReturnsViewWithEmptyCategoryList()
        {
            var searchString = "NonExistingCategory"; 
            var expectedResultCount = 0; 
            var result = controller.PopularSearches(searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts()
        {
            var sortOption = "ascendingName"; 
            var searchString = "Flowers"; 
            var expectedResultCount = 2; 

            var result = controller.Sort(sortOption, searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
            CollectionAssert.AreEqual(testData, model);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenCategoryIsNull()
        {
            var sortOption = "ascendingName"; 
            var searchString = "Lily";
            var expectedResultCount = 1; 

            var result = controller.Sort(sortOption, searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenFlowerTypeIsNull()
        {
            var sortOption = "ascendingName";
            var searchString = "Product 1"; 
            var expectedResultCount = 1; 

            var result = controller.Sort(sortOption, searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenSearchStringIsNull()
        {
            var sortOption = "descendingName"; 
            string searchString = null; 
            var expectedResultCount = 2; 

            var result = controller.Sort(sortOption, searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenSortOptionIsAscendingPrice()
        {
            var sortOption = "ascendingPrice";
            string searchString = null; 
            var expectedResultCount = 2; 

            var result = controller.Sort(sortOption, searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenSortOptionIsDescendingPrice()
        {
            var sortOption = "descendingPrice"; 
            string searchString = null; 
            var expectedResultCount = 2; 

            var result = controller.Sort(sortOption, searchString);
            var result1 = controller.Sort("Test", searchString);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        

      

        //written by: Almedin Pašalić
        [TestMethod]
        public async Task Edit_ReturnsViewWithProduct()
        {
            int? existingProductId = 1;  
            int? nonExistingProductId = 100;  
           
            var existingProduct = new Product { ProductID = existingProductId.Value, Name = "ExistingProduct" };
            dbContextMock.Setup(d => d.Products.FindAsync(existingProductId)).ReturnsAsync(existingProduct);

            int? id = null;
            
            var existingProductResult = await controller.Edit(existingProductId);
            var nonExistingProductResult = await controller.Edit(nonExistingProductId);
            var idIsNull = await controller.Edit(id);

            Assert.IsInstanceOfType(existingProductResult, typeof(ViewResult));
            var existingProductViewResult = (ViewResult)existingProductResult;
            Assert.IsNotNull(existingProductViewResult.Model);
            Assert.IsInstanceOfType(existingProductViewResult.Model, typeof(Product));

            Assert.IsInstanceOfType(nonExistingProductResult, typeof(NotFoundResult));
            Assert.IsInstanceOfType(idIsNull, typeof(NotFoundResult));
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public async Task Edit_ShouldReturnNotFound_WhenIdDoesNotMatch()
        {
            var id = 1;

            var result = await controller.Edit(id, testProduct);

            controller.ModelState.AddModelError("Price", "Price must be greater than zero.");
            var result1 = await controller.Edit(id, testData[0]);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            Assert.IsInstanceOfType(result1, typeof(ViewResult));
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public async Task Edit_ShouldReturnViewWithAllProducts_WhenEditIsSuccessful()
        {
            int id = 1;

            var result = await controller.Edit(id, testData[0]);

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            Assert.AreEqual("~/Views/Home/Index.cshtml", viewResult.ViewName);

            var model = viewResult.Model as List<Product>;
            Assert.IsNotNull(model);
        }

        //written by: Almedin Pašalić and Vedran Mujić
        [TestMethod]
        public async Task Edit_ShouldReturnViewWithAllProducts_WhenProductIsNull()
        {
            int id = -1; 

            var product = new Product
            {
                ProductID = -1,
                Name = "Product 2",
                ImageUrl = "url2",
                Price = 29.99,
                FlowerType = "Lily",
                Stock = 15,
                Category = "Flowers",
                Description = "Elegant white lily",
                productType = "Type B"
            };

            var result = await controller.Edit(id, product);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        //written by: Almedin Pašalić and Vedran Mujić
        [TestMethod]
        public async Task EditNameAndPrice_ReturnsViewWithProduct_WhenProductDoesntExist_ModelStateIsntValid()
        {
            int nonExistingProductId = 100;
            
            var result = await controller.EditNameAndPrice(nonExistingProductId, testData[0]);
            Assert.IsInstanceOfType(result,typeof(NotFoundResult));

            controller.ModelState.AddModelError("Price", "Price must be greater than zero.");
            var result1 = await controller.EditNameAndPrice(testData[0].ProductID, testData[0]);

            Assert.IsInstanceOfType(result1, typeof(ViewResult));

            var viewResult = (ViewResult)result1;
            Assert.AreEqual("~/Views/Home/Index.cshtml", viewResult.ViewName);
        }

        //written by: Almedin Pašalić and Vedran Mujić
        [TestMethod]
        public async Task EditNameAndPrice_ShouldReturnViewWithAllProducts_WhenEditIsSuccessful()
        {
            int id = 1;
          
            var result = await controller.EditNameAndPrice(id, testData[0]);

            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            Assert.AreEqual("~/Views/Home/Index.cshtml", viewResult.ViewName);

            var model = viewResult.Model as List<Product>;
            Assert.IsNotNull(model);
        }

        //written by: Almedin Pašalić and Vedran Mujić
        [TestMethod]
        public async Task EditNameAndPrice_ShouldReturnViewWithAllProducts_WhenProductIsNull()
        {
            int id = -1; 

            var product = new Product
            {
                ProductID = -1,
                Name = "Product 2",
                ImageUrl = "url2",
                Price = 29.99,
                FlowerType = "Lily",
                Stock = 15,
                Category = "Flowers",
                Description = "Elegant white lily",
                productType = "Type B"
            };

            var result = await controller.EditNameAndPrice(id, product);
   
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public async Task Delete_ShouldReturnViewWithProduct_WhenProductExists()
        {
            int id = 1; 
            int notExsistingId = 8;

            // Act
            var result = await controller.Delete(id);
            var resultIdNull = await controller.Delete(null);
            var resultProductNull = await controller.Delete(notExsistingId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(resultIdNull, typeof(NotFoundResult));
            Assert.IsInstanceOfType(resultProductNull, typeof(NotFoundResult));

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.Model, typeof(Product));
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public async Task DeleteConfirmed_ShouldRedirectToIndex_WhenProductExists()
        {
            int id = 2; 

            var result = await controller.DeleteConfirmed(id);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        
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
        public async Task Details_ShouldReturnViewWithCorrectProduct(int productId, string productName, string imageUrl, double price, string flowerType, int stock, string category, string description, string productType)
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


        //written by: Vedran Mujić
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public async Task Create_ShouldRedirectToAction(int productId, string productName, string imageUrl, double price, string flowerType, int stock, string category, string description, string productType)
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

            productsController.ModelState.Clear();

            var result = await productsController.Create(testProduct);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        //written by: Vedran Mujić
        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public async Task Create_ProductWithPriceError(int productId, string productName, string imageUrl, double price, string flowerType, int stock, string category, string description, string productType)
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

            productsController.ModelState.Clear();

            productsController.ModelState.AddModelError("Price", "Price must be greater than zero.");

            var result = await productsController.Create(testProduct);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

    }

}
