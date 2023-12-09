using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ayana.Data;
using Ayana.MailgunService;
using Ayana.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using OfficeOpenXml.Style;

namespace AyanaTests
{
    [TestClass]
    public class CustomerServiceClassTests
    {
        // written by : Lejla Heleg
        [TestMethod]
        public void GetInactiveCustomers_HasInactiveCustomers_ReturnsOneCustomer()
        {
            var orders = new List<Order>
            {
                new Order { CustomerID = "1", purchaseDate = DateTime.Now.AddDays(29) },
                new Order { CustomerID = "2", purchaseDate = DateTime.Now.AddDays(31) }
            };

            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1" },
                new ApplicationUser { Id = "2" }
            };
            var orderDbSetMock = new Mock<DbSet<Order>>();
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.AsQueryable().Provider);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            orderDbSetMock.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var userDbSetMock = new Mock<DbSet<ApplicationUser>>();
            userDbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.AsQueryable().Provider);
            userDbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            userDbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            userDbSetMock.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var dbContextMock = new Mock<ApplicationDbContext>();
            dbContextMock.Setup(d => d.Orders).Returns(orderDbSetMock.Object);
            dbContextMock.Setup(d => d.Users).Returns(userDbSetMock.Object);

            var service = new CustomerService(dbContextMock.Object);

            var result = service.GetInactiveCustomers();

            Assert.AreEqual(1, result.Count);
        }
    }
}
