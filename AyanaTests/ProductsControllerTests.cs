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
    //written by: Almedin Pašalić
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
        public async Task Details_ReturnsViewWithProduct()
        {
            int productId = 1; 

            var result = await controller.Details(1);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = viewResult.Model as Product;

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.ProductID);
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
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            controller.ModelState.Clear();

            var result = await controller.Create(testProduct);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        //written by: Almedin Pašalić
        [TestMethod]
        public async Task Create_ValidModel_RedirectsToIndex_WhenModelIsNotValid()
        {
            controller.ModelState.AddModelError("Price", "Price must be greater than zero.");

            var result = await controller.Create(testProduct);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
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

        //written by: Almedin Pašalić
        [TestMethod]
        public void ProductExists_ShouldReturnTrue_WhenProductExists()
        {
            int id = 1; 

            var result = controller.ProductExists(id);

            Assert.IsTrue(result);
        }




    }


}
