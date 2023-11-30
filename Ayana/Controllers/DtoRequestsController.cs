using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ayana.Data;
using Ayana.Models;
using System.Security.Claims;
using Humanizer;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using static Humanizer.In;
using static Humanizer.On;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.VisualBasic;
using Ayana.Paterni;
using System.Collections;

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

            // Inicijalne vrijednosti za discountAmount i discountType 
            double appliedDiscountAmount = 0;
            string appliedDiscountType = "";

            // Provjera da li uneseni korisnički kod postoji u bazi i da li je istekao
            var isDiscountCodeValid =  _discountCodeVerifier.VerifyDiscountCode(userInputtedCode); 
            if (!isDiscountCodeValid)
                userInputtedCode = "Wrong code, try again...";
            else if (isDiscountCodeValid && !_discountCodeVerifier.VerifyExperationDate(userInputtedCode))
                userInputtedCode = "Code is expired...";
            else
            {
                // Ako je kod validan i nije istekao, dohvati informacije o popustu iz baze
                Discount appliedDiscount = _discountCodeVerifier.GetDiscount(userInputtedCode);
                appliedDiscountAmount = appliedDiscount.DiscountAmount;
                appliedDiscountType = appliedDiscount.DiscountType.ToString();
            }

            // Odredi tip popusta (0 za PercentageOff, 1 za AmountOff)
            var discountTypeCode = appliedDiscountType == "AmountOff" ? 1 : 0;

            // Ažuriraj trenutni cart s novim informacijama o popustu ili ostavi bez popusta ako kod nije validan
            return RedirectToAction("Cart", new 
            {
                discountAmount = appliedDiscountAmount.ToString(),
                discountType = discountTypeCode.ToString(),
                discountCode = userInputtedCode
            });

        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            // Dobavljanje ID korisnika koji je trenutno prijavljen
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Provjera da li je korisnik prijavljen
            if (userId == null)
                return View("Error");

            //Pronalaženje stavke u korpi na osnovu ID korisnika i ID proizvoda
            var cart = _context.Cart.FirstOrDefault(o => o.CustomerID == userId && o.ProductID == id);

            // Provjera količine proizvoda u korpi
            if (cart.ProductQuantity != 1)
            {
                // Smanjenje količine proizvoda za jedan i ažuriranje baze podataka
                cart.ProductQuantity--;
                await _context.SaveChangesAsync();
            }

            else
            {
                // Ako je količina proizvoda jednaka 1, uklanjanje stavke iz korpe
                _context.Remove(cart);
                await _context.SaveChangesAsync();
            }

            // Redirekcija na akciju "Cart" i uklananje popusta zato što se mijenja ukupna cijena narudžbe 
            return RedirectToAction("Cart", new
            {
                discountAmount = 0,
                discountType = 1,
                discountCode = ""
            });

        }


        public async Task<IActionResult> AddToCart(int productId)
        {
            // Dobavljanje ID korisnika koji je trenutno prijavljen
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Provjera da li je korisnik prijavljen
            if (userId == null)
                return View("Error","Only registered users can buy our products. Sign up and enjoy our products");

            // Pretraga korpe da li već postoji proizvod sa datim ID-jem za trenutnog korisnika
            var existingCartItem = _context.Cart.FirstOrDefault(o => o.CustomerID == userId && o.ProductID == productId);

            // Ako proizvod nije pronađen u korpi, dodaj ga
            if (existingCartItem == null)
            {
                // Korisnik ima cart za svaki proizvod zasebno, gdje je upisan ID i količina proizvoda
                Cart newCartItem = new Cart()
                {
                    CustomerID = userId,
                    ProductID = productId,
                    ProductQuantity = 1

                };

                // Dodavanje novog proizvoda u korpu
                _context.Add(newCartItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Ako proizvod već postoji, povećaj količinu
                existingCartItem.ProductQuantity++;
                await _context.SaveChangesAsync();
            }

            // Vraćanje JSON odgovora koji označava uspeh operacije
            return Json(new { message = "Radi!" });

        }


        public List<List<Product>> GetCartProducts(List<Cart> carts)
        {
            // Lista koja će sadržavati listu proizvoda za svaku korpu
            List<List<Product>> cartProducts = new List<List<Product>>();

            // Iteracija kroz svaku korpu u listi korpi
            foreach (var cart in carts)
            {
                // Dobavljanje proizvoda za trenutnu korpu iz baze podataka
                var products = _context.Cart
                    .Where(po => po.CartID == cart.CartID)
                    .Select(po => po.Product)
                    .ToList();

                // Dodavanje liste proizvoda za trenutnu korpu u glavnu listu
                cartProducts.Add(products);
            }

            // Vraćanje liste listi proizvoda za svaku korpu
            return cartProducts;
        }

        // GET: DtoRequests/Create
        public IActionResult Cart(string discountAmount, string discountType, string discountCode = "")
        {
            // Parsiranje vrijednosti za popust
            double doubleDiscountAmount = Double.Parse(discountAmount);

            // Parsiranje vrijednosti za tip popusta
            int intDiscountType = int.Parse(discountType);

            // Dobavljanje ID-ja trenutno prijavljenog korisnika
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Provjera da li je korisnik prijavljen
            if (userId == null)
                return View("Error");

            // Dobavljanje korpi specifičnih za korisnika na osnovu CustomerId
            List<Cart> userCarts = _context.Cart
                .Where(o => o.CustomerID == userId)
                .ToList();

            // Dobavljanje povezanih proizvoda za svaku korpu
            List<List<Product>> cartProducts = GetCartProducts(userCarts);

            // Prosljeđivanje korisničkih korpi i proizvoda korpi u view
            ViewBag.UserCarts = userCarts;
            ViewBag.CartProducts = cartProducts;
            ViewBag.DiscountCode = discountCode;

            // Računanje ukupnog iznosa bez popusta
            double amountToPayWithoutDiscount = 0;

            for (var i = 0; i < userCarts.Count; i++)
            {
                var cart = userCarts[i];
                var products = cartProducts[i];

                foreach (var product in products)
                {
                    // Dodavanje cijene proizvoda pomnožene sa količinom u ukupni iznos
                    amountToPayWithoutDiscount += (double)(product.Price * cart.ProductQuantity);
                }
            }

            // Računanje ukupnog iznosa sa popustom
            double amountToPayWithDiscount = 0;

            // AmountOff tip popusta 1
            if (intDiscountType == 1)
                // Računanje iznosa sa fiksnim popustom
                amountToPayWithDiscount = amountToPayWithoutDiscount - doubleDiscountAmount;
            else
            {
                // PercentageOff tip popusta 0
                // Računanje iznosa sa procentualnim popustom
                amountToPayWithDiscount = (amountToPayWithoutDiscount * (100 - doubleDiscountAmount)) / 100;
            }

            // Prosljeđivanje iznosa koji treba platiti u view
            ViewBag.TotalAmountToPay = amountToPayWithDiscount;

            return View();
        }

        private async Task<(double totalWithDiscount, int? discountId, double? discountAmount)> ApplyDiscount(Payment payment, Discount discount)
        {
           
            int? discountId = null;
            double? discountAmount = null;

            double totalWithDiscount = payment.PayedAmount;

            // Provjeri postoji li uneseni kod za popust
            if (discount.DiscountCode != null)
            {
                int? discountType = null;

                // Provjeri i primijeni popust ukoliko je kod ispravan i nije istekao
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
                // Primijeni popust na ukupnu cijenu
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
            Payment paymentForOrder = new Payment
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
            Order newOrder = new Order
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
                    ProductOrder productOrder = new ProductOrder
                    {
                        OrderID = newOrder.OrderID,
                        ProductID = product.ProductID,
                        ProductQuantity = cart.ProductQuantity
                    };

                    for (var k = 0; k < cart.ProductQuantity; k++)
                    {
                        ProductSales productSales = new ProductSales
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

            // Dobavljanje ID-ja trenutno prijavljenog korisnika
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Provjeri da li je korisnik prijavljen
            var existingCustomer = _context.Users.FirstOrDefault(m => m.Id == userId);

            if (userId == null)
            {
                return View("~/Views/Shared/Error.cshtml");
            }

            // Ukupna cijena korpe bez discounta se dodaje na novi order 
            var totalWithoutDiscount = payment.PayedAmount;

            // Parsiranje tuple rezultata iz metode ApplyDiscount
            var discountResult = await ApplyDiscount(payment, discount);
            double totalWithDiscount = discountResult.totalWithDiscount;
            int? discountId = discountResult.discountId;
            double? discountAmount = discountResult.discountAmount;

            // Kreiranje paymenta za order
            Payment paymentForOrder = await SavePaymentData(payment, totalWithDiscount, discountId);

            // Kreiranje ordera
            Order newOrder = await SaveOrderData(order, userId, paymentForOrder, totalWithDiscount);

            // Brisanje carta i ažuriranje tabela za izvještaj o broju prodanih produkata
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



