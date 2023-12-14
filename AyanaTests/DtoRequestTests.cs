using Ayana.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AyanaTests
{

    [TestClass]
    public class DtoRequestTests
    {
        // written by : Aida Zametica
        [TestMethod]
        public void DtoRequest_Initialization_NotNullAfter()
        {

            var dtoRequest = new DtoRequest
            {
                DtoRequestID = 1,
                payment = new Payment(),
                order = new Order(),
                discount = new Discount(),
                product = new Product(),
                productOrder = new ProductOrder(),
                productSales = new ProductSales(),
                cart= new Cart(),
                customer=new Ayana.Data.ApplicationUser { AccessFailedCount = 1 },
            };

            Assert.IsNotNull(dtoRequest);
        }
    }

}
