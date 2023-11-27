using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ayana.Data;
using Ayana.Models;
using System.Security.Claims;

namespace Ayana.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<List<Product>> GetOrderProducts(List<Order> orders)
        {
            List<List<Product>> orderProducts = new List<List<Product>>();

            foreach (var order in orders)
            {
                var products = _context.ProductOrders
                    .Where(po => po.OrderID == order.OrderID)
                    .Select(po => po.Product)
                    .ToList();

                orderProducts.Add(products);
            }

            return orderProducts;
        }

        public IActionResult UserOrders()
        {
            var p1 = _context.Users.ToList();
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Retrieve user-specific orders based on the CustomerId
            List<Order> userOrders = _context.Orders
                .Include(o => o.Payment)
                .Where(o => o.CustomerID == userId)
                .ToList();
            // Get the associated products for each order
            List<List<Product>> orderProducts = GetOrderProducts(userOrders);

            // Pass the userOrders and orderProducts to the view
            ViewBag.UserOrders = userOrders.OrderBy(order => order.Rating).ToList(); ;
            ViewBag.OrderProducts = orderProducts;

            // Render the view
            return View();
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("OrderID,Rating")] Order order)
        {
            var o = _context.Orders.ToList();
            var existingOrder = o.Find(m => m.OrderID == order.OrderID);

            existingOrder.Rating = order.Rating;
            _context.SaveChanges();
            return Redirect("/Orders/UserOrders");
        }
    }
}
