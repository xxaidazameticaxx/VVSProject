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

       
        public IActionResult ActiveOrders()
            {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            DateTime today = DateTime.Today;

            // Retrieve user-specific orders based on the CustomerId
            List<Order> userOrders = _context.Orders
                .Include(o => o.Payment)
                .Where(o => o.CustomerID == userId && o.DeliveryDate >= today)
                .OrderBy(o => o.DeliveryDate)
                .ToList();

            // Get the associated products for each order
            List<List<Product>> orderProducts = GetOrderProducts(userOrders);

            // Pass the userOrders and orderProducts to the view
            ViewBag.UserOrders = userOrders;
            ViewBag.OrderProducts = orderProducts;

            // Render the view
            return View("ActiveOrders");
        }

        //TDD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder([Bind("OrderID")] Order order)
        {
            // Get the ID of the currently logged-in user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            // Get the order that needs to be canceled
            var orderToDelete = await _context.Orders.FindAsync(order.OrderID);
            if (orderToDelete == null)
            {
                return NotFound();
            }

            // Check if the delivery date is less than 3 days before today
            if ((orderToDelete.DeliveryDate - DateTime.Today).TotalDays < 3)
            {

                TempData["ErrorMessage"] = "You cannot cancel an order scheduled for delivery within the next 3 days.";
            }
            else
            {

                await ProcessOrderCancellationAsync(orderToDelete);
                TempData["SuccessMessage"] = "Order successfully canceled.";

            }

            // Reload the user's orders after cancellation
            ViewBag.UserOrders = GetUserOrders(userId);

            return Redirect("ActiveOrders");
        }

        //TDD
        private List<Order> GetUserOrders(string userId)
        {
            DateTime today = DateTime.Today;

            // Retrieve user-specific orders based on the CustomerId
            List<Order> userOrders = _context.Orders
                .Include(o => o.Payment)
                .Where(o => o.CustomerID == userId && o.DeliveryDate >= today)
                .OrderBy(o => o.DeliveryDate)
                .ToList();

            return userOrders;
        }

        //TDD
        private async Task ProcessOrderCancellationAsync(Order orderToDelete)
        {
            // Retrieve productOrders based on the OrderId
            List<ProductOrder> productOrderToDelete = _context.ProductOrders
                .Where(o => o.OrderID == orderToDelete.OrderID)
                .ToList();

            // Retrieve payment based on the PaymentId
            Payment paymentToDelete = _context.Payments
                .SingleOrDefault(o => o.PaymentID == orderToDelete.PaymentID);

            foreach (var productOrder in productOrderToDelete)
            {
                List<ProductSales> productSalesToDelete = _context.ProductSales
               .Where(o => o.ProductID == productOrder.ProductID && o.SalesDate == orderToDelete.purchaseDate)
               .Take((int)productOrder.ProductQuantity)
               .ToList();

                foreach (var productSales in productSalesToDelete)
                {
                    _context.Remove(productSales);
                }

            }

            foreach (var productOrder in productOrderToDelete)
            {
                _context.Remove(productOrder);
            }

            _context.Remove(paymentToDelete);

            _context.Remove(orderToDelete);

            await _context.SaveChangesAsync();
        }
    }
}
