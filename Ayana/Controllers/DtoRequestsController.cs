using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ayana.Data;
using Ayana.Models;
using System.Security.Claims;
using Ayana.Paterni;

namespace Ayana.Controllers
{
    public class DtoRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IDiscountCodeVerifier _discountCodeVerifier;

        public DtoRequestsController(ApplicationDbContext context, IDiscountCodeVerifier discountCodeVerifier)
        {
            _context = context;
            _discountCodeVerifier = discountCodeVerifier;
        }

        public async Task<IActionResult> ApplyDiscount(string userInputtedCode)
        {

            // Initial values for discountAmount and discountType
            double appliedDiscountAmount = 0;
            string appliedDiscountType = "";

            // Check if the user-entered code exists in the database and is not expired
            var isDiscountCodeValid =  _discountCodeVerifier.VerifyDiscountCode(userInputtedCode); 
            if (!isDiscountCodeValid)
                userInputtedCode = "Wrong code, try again...";
            else if (!_discountCodeVerifier.VerifyExperationDate(userInputtedCode))
                userInputtedCode = "Code is expired...";
            else
            {
                // If the code is valid and not expired, retrieve discount information from the database
                Discount appliedDiscount = _discountCodeVerifier.GetDiscount(userInputtedCode);
                appliedDiscountAmount = appliedDiscount.DiscountAmount;
                appliedDiscountType = appliedDiscount.DiscountType.ToString();
            }

            // Determine the discount type (0 for PercentageOff, 1 for AmountOff)
            var discountTypeCode = appliedDiscountType == "AmountOff" ? 1 : 0;

            // Update the current cart with new discount information or leave it without a discount if the code is not valid
            return RedirectToAction("Cart", new 
            {
                discountAmount = appliedDiscountAmount.ToString(),
                discountType = discountTypeCode.ToString(),
                discountCode = userInputtedCode
            });

        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            // Get the ID of the currently logged-in user

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            // Find the item in the cart based on the user ID and product ID
            var cart = _context.Cart.FirstOrDefault(o => o.CustomerID == userId && o.ProductID == id);
           

            // Check the quantity of the product in the cart
            if (cart.ProductQuantity != 1)
            {
                // Decrease the quantity of the product by one and update the database
                cart.ProductQuantity--;
                await _context.SaveChangesAsync();
            }

            else
            {
                // If the quantity of the product is 1, remove the item from the cart
                _context.Remove(cart);
                await _context.SaveChangesAsync();
            }

            // Redirect to the "Cart" action and remove the discount because the total order amount is changing
            return RedirectToAction("Cart", new
            {
                discountAmount = 0,
                discountType = 1,
                discountCode = ""
            });

        }


        public async Task<IActionResult> AddToCart(int productId)
        {
            // Get the ID of the currently logged-in user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the user is logged in
            if (userId == null)
                return View("Error","Only registered users can buy our products. Sign up and enjoy our products");

            // Search the cart to see if there is already a product with the given ID for the current user
            var existingCartItem = _context.Cart.FirstOrDefault(o => o.CustomerID == userId && o.ProductID == productId);

            // If the product is not found in the cart, add it
            if (existingCartItem == null)
            {
                // Each product has its own cart for the user, where the ID and quantity of the product are stored
                Cart newCartItem = new()
                {
                    CustomerID = userId,
                    ProductID = productId,
                    ProductQuantity = 1

                };

                // Add the new product to the cart
                _context.Add(newCartItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                // If the product already exists, increase the quantity
                existingCartItem.ProductQuantity++;
                await _context.SaveChangesAsync();
            }

            // Return a JSON response indicating the success of the operation
            return Json(new { message = "Radi!" });

        }


        public List<List<Product>> GetCartProducts(List<Cart> carts)
        {
            // List that will contain a list of products for each cart
            List<List<Product>> cartProducts = new();

            // Iterate through each cart in the list of carts
            foreach (var cart in carts)
            {
                // Get the products for the current cart from the database
                var products = _context.Cart
                    .Where(po => po.CartID == cart.CartID)
                    .Select(po => po.Product)
                    .ToList();

                // Add the list of products for the current cart to the main list
                cartProducts.Add(products);
            }

            // Return the list of lists of products for each cart
            return cartProducts;
        }

        // GET: DtoRequests/Create
        public IActionResult Cart(string discountAmount, string discountType, string discountCode = "")
        {
            // Parse values for the discount
            double doubleDiscountAmount = Double.Parse(discountAmount);

            // Parse values for the discount type
            int intDiscountType = int.Parse(discountType);

            // Get the ID of the currently logged-in user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the user is logged in
            if (userId == null)
                return View("Error");

            // Get user-specific carts based on CustomerId
            List<Cart> userCarts = _context.Cart
                .Where(o => o.CustomerID == userId)
                .ToList();

            // Get related products for each cart
            List<List<Product>> cartProducts = GetCartProducts(userCarts);

            // Pass user carts and cart products to the view
            ViewBag.UserCarts = userCarts;
            ViewBag.CartProducts = cartProducts;
            ViewBag.DiscountCode = discountCode;

            // Calculate the total amount without discount
            double amountToPayWithoutDiscount = 0;

            for (var i = 0; i < userCarts.Count; i++)
            {
                var cart = userCarts[i];
                var products = cartProducts[i];

                foreach (var product in products)
                {
                    // Add the product price multiplied by the quantity to the total amount
                    amountToPayWithoutDiscount += (double)(product.Price * cart.ProductQuantity);
                }
            }

            // Calculate the total amount with discount
            double amountToPayWithDiscount = 0;

            // AmountOff discount type 1
            if (intDiscountType == 1)
                // Calculate the amount with a fixed discount
                amountToPayWithDiscount = amountToPayWithoutDiscount - doubleDiscountAmount;
            else
            {
                // PercentageOff discount type 0
                // Calculate the amount with a percentage discount
                amountToPayWithDiscount = (amountToPayWithoutDiscount * (100 - doubleDiscountAmount)) / 100;
            }

            // Pass the amount to be paid to the view
            ViewBag.TotalAmountToPay = amountToPayWithDiscount;

            return View();
        }

        public async Task<(double totalWithDiscount, int? discountId, double? discountAmount)> CalculateDiscount(Payment payment, Discount discount)
        {
           
            int? discountId = null;
            double? discountAmount = null;

            double totalWithDiscount = payment.PayedAmount;

            // Check if the entered discount code exists
            if (discount.DiscountCode != null)
            {
                int? discountType = null;

                // Check and apply the discount if the code is valid and not expired
                if (_discountCodeVerifier.VerifyDiscountCode(discount.DiscountCode))
                {
                    if (_discountCodeVerifier.VerifyExperationDate(discount.DiscountCode))
                    {
                        discount = _discountCodeVerifier.GetDiscount(discount.DiscountCode);
                        discountId = discount.DiscountID;
                        discountType = (int)discount.DiscountType;
                        discountAmount = discount.DiscountAmount;
                    }
                }
                // Apply the discount to the total price
                if (discountType == 1)
                    totalWithDiscount = (double)(totalWithDiscount - discountAmount);
                else
                {
                    totalWithDiscount = (double)(totalWithDiscount * (100 - discountAmount) / 100);
                }

            }
            return (totalWithDiscount, discountId, discountAmount);
        }

        private async Task<Payment> SavePaymentData(Payment payment, double totalWithDiscount, int? discountId)
        {
            Payment paymentForOrder = new()
            {
                BankAccount = payment.BankAccount,
                DeliveryAddress = payment.DeliveryAddress,
                DiscountID = discountId,
                PaymentType = payment.PaymentType,
                PayedAmount = totalWithDiscount
            };

            _context.Add(paymentForOrder);
            await _context.SaveChangesAsync();

            return paymentForOrder;
        }

        private async Task<Order> SaveOrderData(Order order, string userId, Payment paymentForOrder, double totalWithDiscount)
        {
            Order newOrder = new()
            {
                DeliveryDate = order.DeliveryDate,
                CustomerID = userId,
                PaymentID = paymentForOrder.PaymentID,
                TotalAmountToPay = totalWithDiscount,
                purchaseDate = DateTime.Now,
                personalMessage = order.personalMessage,
                IsOrderSent = false,
                Rating = null
            };

            _context.Add(newOrder);
            await _context.SaveChangesAsync();

            return newOrder;
        }

        private async Task ProcessCartItems(string userId, Order newOrder)
        {
            List<Cart> userCarts = _context.Cart
                .Where(o => o.CustomerID == userId)
                .ToList();

            List<List<Product>> cartProducts = GetCartProducts(userCarts);

            for (var i = 0; i < userCarts.Count; i++)
            {
                var cart = userCarts[i];
                var products = cartProducts[i];

                foreach (var product in products)
                {
                    ProductOrder productOrder = new()
                    {
                        OrderID = newOrder.OrderID,
                        ProductID = product.ProductID,
                        ProductQuantity = cart.ProductQuantity
                    };

                    for (var k = 0; k < cart.ProductQuantity; k++)
                    {
                        ProductSales productSales = new()
                        {
                            SalesDate = DateTime.Now,
                            ProductID = product.ProductID
                        };

                        _context.Add(productSales);
                        await _context.SaveChangesAsync();
                    }

                    _context.Add(productOrder);
                    await _context.SaveChangesAsync();
                }
            }

            foreach (Cart cart in userCarts)
            {
                _context.Cart.Remove(cart);
            }

            _context.SaveChanges();
        }

        public async Task<IActionResult> OrderCreate([Bind("Name,Price,personalMessage,DeliveryDate")] Order order, [Bind("DeliveryAddress,BankAccount,PaymentType,PayedAmount")] Payment payment, [Bind("DiscountCode")] Discount discount)
        {
            // Get the ID of the currently logged-in user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the user is logged in
            var existingCustomer = _context.Users.FirstOrDefault(m => m.Id == userId);

            if (userId == null)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

            // Total cart price without discount is added to the new order
            var totalWithoutDiscount = payment.PayedAmount;

            // Parse the tuple results from the ApplyDiscount method
            var discountResult = await CalculateDiscount(payment, discount);
            double totalWithDiscount = discountResult.totalWithDiscount;
            int? discountId = discountResult.discountId;
            double? discountAmount = discountResult.discountAmount;

            // Create payment for the order
            Payment paymentForOrder = await SavePaymentData(payment, totalWithDiscount, discountId);

            // Create the order
            Order newOrder = await SaveOrderData(order, userId, paymentForOrder, totalWithDiscount);

             // Clear the cart and update tables for the sales report
            await ProcessCartItems(userId, newOrder);

            return Redirect("/DtoRequests/ThankYou?orderType=order");
        }


        public IActionResult ThankYou(string orderType)
        {
            ViewBag.OrderType = orderType;
            return View();
        }
    }
}



