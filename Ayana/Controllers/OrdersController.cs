using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ayana.Data;
using Ayana.Models;
using System.Security.Claims;
using System;

namespace Ayana.Controllers
{
    // Controller responsible for managing and processing user order reviews
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
            // Get the associated products for each order
            foreach (var order in orders)
            {
                var products = _context.ProductOrders
                    .Where(po => po.OrderID == order.OrderID)
                    .Select(po => po.Product)
                    .ToList();

                orderProducts.Add(products);
            }
            // Returns list of product lists for each order
            return orderProducts;
        }

        public IActionResult UserOrders()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Retrieve user-specific orders based on the CustomerId
            List<Order> userOrders = _context.Orders
                .Include(o => o.Payment)
                .Where(o => o.CustomerID == userId)
                .ToList();
            userOrders = userOrders.OrderBy(order => order.Rating).ToList();
            // Get the associated products for each order
            List<List<Product>> orderProducts = GetOrderProducts(userOrders);

            // Pass the userOrders and orderProducts to the view
            ViewBag.UserOrders = userOrders;
            ViewBag.OrderProducts = orderProducts;

            // Render the view
            return View("UserOrders");
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("OrderID,Rating")] Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(order.OrderID);
            if (existingOrder == null)
            {
                return NotFound();
            }
            existingOrder.Rating = order.Rating;
            await _context.SaveChangesAsync();
            return Redirect("UserOrders");
        }

    }
}
