using System.Linq.Expressions;
using System.Security.Claims;
using Ayana.Controllers;
using Ayana.Data;
using Ayana.Models;
using Ayana.Patterns;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

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

            // Konfiguracija DbContext mock-a
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

            iProductMock.Setup(m => m.EditAll(It.IsAny<Product>()))
                .Throws(new DbUpdateConcurrencyException());

            controller = new ProductsController(dbContextMock.Object, iProductMock.Object);
        }


        [TestMethod]
        public async Task Index_ReturnsViewWithProducts()
        {
            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Create_ReturnsView()
        {
            // Act
            var result = controller.Create();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }


        [TestMethod]
        public void SearchResult_ReturnsViewWithFilteredProducts()
        {
            // Arrange
            var searchString = "Product 1"; // Postavite željenu pretragu
            var expectedResultCount = 1; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.SearchResult(searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void SearchResult_ReturnsViewWithAllProductsWhenSearchIsNull()
        {
            // Act
            var result = controller.SearchResult(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            CollectionAssert.AreEqual(testData, model);
        }

        [TestMethod]
        public async Task Details_ReturnsViewWithProduct()
        {
            int productId = 1; // Postavite ID proizvoda koji želite koristiti za testiranje

            // Act
            var result = await controller.Details(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            // Dobijanje modela iz rezultata
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as Product;

            // Assert za model
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.ProductID);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenProductIsNull()
        {
            // Act
            var result = await controller.Details(4);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PopularSearches_ReturnsViewWithFilteredProducts()
        {
            // Arrange
            var searchString = "BAM.25"; // Postavite željenu pretragu
            var expectedResultCount = 1; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.PopularSearches(searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void PopularSearches_ReturnsViewWithCategoryProducts()
        {
            // Arrange
            var searchString = "Flowers"; // Postavite željenu pretragu
            var expectedResultCount = 2; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.PopularSearches(searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void PopularSearches_ReturnsViewWithFlowerTypeProducts()
        {
            // Arrange
            var searchString = "Rose"; // Postavite željenu pretragu
            var expectedResultCount = 1; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.PopularSearches(searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void PopularSearches_ReturnsViewWithEmptyCategoryList()
        {
            // Arrange
            var searchString = "NonExistingCategory"; // Postavite željenu pretragu
            var expectedResultCount = 0; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.PopularSearches(searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts()
        {
            // Arrange
            var sortOption = "ascendingName"; // Postavite željeni sortOption
            var searchString = "Flowers"; // Postavite željenu pretragu
            var expectedResultCount = 2; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.Sort(sortOption, searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
            CollectionAssert.AreEqual(testData, model);
        }

        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenCategoryIsNull()
        {
            // Arrange
            var sortOption = "ascendingName"; // Postavite željeni sortOption
            var searchString = "Lily"; // Postavite željenu pretragu
            var expectedResultCount = 1; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.Sort(sortOption, searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenFlowerTypeIsNull()
        {
            // Arrange
            var sortOption = "ascendingName"; // Postavite željeni sortOption
            var searchString = "Product 1"; // Postavite željenu pretragu
            var expectedResultCount = 1; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.Sort(sortOption, searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenSearchStringIsNull()
        {
            // Arrange
            var sortOption = "descendingName"; // Postavite željeni sortOption
            string searchString = null; // Postavite željenu pretragu
            var expectedResultCount = 2; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.Sort(sortOption, searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenSortOptionIsAscendingPrice()
        {
            // Arrange
            var sortOption = "ascendingPrice"; // Postavite željeni sortOption
            string searchString = null; // Postavite željenu pretragu
            var expectedResultCount = 2; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.Sort(sortOption, searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public void Sort_ReturnsPartialViewWithSortedProducts_WhenSortOptionIsDescendingPrice()
        {
            // Arrange
            var sortOption = "descendingPrice"; // Postavite željeni sortOption
            string searchString = null; // Postavite željenu pretragu
            var expectedResultCount = 2; // Broj očekivanih elemenata u rezultatu

            // Act
            var result = controller.Sort(sortOption, searchString);
            var result1 = controller.Sort("Test", searchString);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = (PartialViewResult)result;
            var model = viewResult.Model as List<Product>;

            Assert.IsNotNull(model);
            Assert.AreEqual(expectedResultCount, model.Count);
        }

        [TestMethod]
        public async Task Create_ValidModel_RedirectsToIndex()
        {

            controller.ModelState.Clear(); // Očisti ModelState

            // Act
            var result = await controller.Create(testProduct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task Create_ValidModel_RedirectsToIndex_WhenModelIsNotValid()
        {

            controller.ModelState.AddModelError("Price", "Price must be greater than zero.");

            // Act
            var result = await controller.Create(testProduct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ReturnsViewWithProduct()
        {
            // Arrange
            int? existingProductId = 1;  // Postavite ID proizvoda koji postoji
            int? nonExistingProductId = 100;  // Postavite ID koji ne postoji

            // Simulirajte postojanje proizvoda sa datim ID-om u bazi podataka
            var existingProduct = new Product { ProductID = existingProductId.Value, Name = "ExistingProduct" };
            dbContextMock.Setup(d => d.Products.FindAsync(existingProductId)).ReturnsAsync(existingProduct);

            int? id = null;
            // Act
            var existingProductResult = await controller.Edit(existingProductId);
            var nonExistingProductResult = await controller.Edit(nonExistingProductId);
            var idIsNull = await controller.Edit(id);

            // Assert
            // Proverite da li je rezultat za postojeci proizvod tipa ViewResult i da li sadrži proizvod
            Assert.IsInstanceOfType(existingProductResult, typeof(ViewResult));
            var existingProductViewResult = (ViewResult)existingProductResult;
            Assert.IsNotNull(existingProductViewResult.Model);
            Assert.IsInstanceOfType(existingProductViewResult.Model, typeof(Product));

            // Proverite da li rezultat za nepostojeci proizvod vraća NotFoundResult
            Assert.IsInstanceOfType(nonExistingProductResult, typeof(NotFoundResult));
            Assert.IsInstanceOfType(idIsNull, typeof(NotFoundResult));
        }


        [TestMethod]
        public async Task Edit_ShouldReturnNotFound_WhenIdDoesNotMatch()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await controller.Edit(id, testProduct);

            controller.ModelState.AddModelError("Price", "Price must be greater than zero.");
            var result1 = await controller.Edit(id, testData[0]);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            Assert.IsInstanceOfType(result1, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Edit_ShouldReturnViewWithAllProducts_WhenEditIsSuccessful()
        {
            // Arrange
            int id = 1; // ID koji postoji u testnim podacima

            // Act
            var result = await controller.Edit(id, testData[0]);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            Assert.AreEqual("~/Views/Home/Index.cshtml", viewResult.ViewName);

            var model = viewResult.Model as List<Product>;
            Assert.IsNotNull(model);
            // Provjerite možda ovisno o implementaciji, model treba sadržavati izmijenjene proizvode.
        }

        [TestMethod]
        public async Task Delete_ShouldReturnViewWithProduct_WhenProductExists()
        {
            // Arrange
            int id = 1; // ID koji postoji u testnim podacima
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

        [TestMethod]
        public async Task DeleteConfirmed_ShouldRedirectToIndex_WhenProductExists()
        {
            // Arrange
            int id = 2; // ID koji postoji u testnim podacima

            // Act
            var result = await controller.DeleteConfirmed(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public void ProductExists_ShouldReturnTrue_WhenProductExists()
        {
            // Arrange
            int id = 1; // ID koji postoji u testnim podacima

            // Act
            var result = controller.ProductExists(id);

            // Assert
            Assert.IsTrue(result);
        }




    }


}
