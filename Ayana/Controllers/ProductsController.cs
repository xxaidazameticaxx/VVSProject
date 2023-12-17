﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ayana.Data;
using Ayana.Models;
using System.Text.RegularExpressions;
using Ayana.Patterns;
using Microsoft.AspNetCore.Authorization;
using Ayana.Paterni;

namespace Ayana.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProduct _productEditor;
        public ProductsController(ApplicationDbContext context, IProduct p)
        {
            _context = context;
            _productEditor = p;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(_context.Products.Where(x=>true).ToList());
        }



        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product =  _context.Products
                .FirstOrDefault(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Employee")]
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult SearchResult(string search)
        {
            List<Product> products = _context.Products.ToList();
            if (search == null)
                return View(products);
            else
            {
                string pattern = $"{Regex.Escape(search)}";
                List<Product> searchResults = products.Where(p => Regex.IsMatch(p.Name, pattern, RegexOptions.IgnoreCase)).ToList();

                ViewBag.String = search;
                return View(searchResults);
            }
        }

        public IActionResult PopularSearches(string popularSearch)
        {
            List<Product> products = _context.Products.ToList();
            bool isItBam = Regex.IsMatch(popularSearch, "BAM", RegexOptions.IgnoreCase);
            if (isItBam)
            {
                int o = int.Parse(popularSearch.Substring(4).Split(".").First());
                List<Product> inPriceRange = products.FindAll(product => product.Price <= o);
                ViewBag.p = inPriceRange;
            }
            else
            {   
                List<Product> categoryList = _context.Products.Where(x => x.Category.ToLower() == popularSearch.ToLower()).ToList();

                if (categoryList.Count == 0)
                {
                    categoryList = _context.Products.Where(x => x.FlowerType != null && x.FlowerType.ToLower() == popularSearch.ToLower()).ToList();

                }
                ViewBag.String = popularSearch;
                ViewBag.p = categoryList;
            }
            return View("~/Views/Products/SearchResult.cshtml", ViewBag.p);
        }

        //Sort products
        [HttpGet]
        public ActionResult Sort(string sortOption, string String)
        {
            ISort sortStrategy;

            if (sortOption == "ascendingName")
            {
                sortStrategy = new AscendingNameSortStrategy();
            }
            else if (sortOption == "descendingName")
            {
                sortStrategy = new DescendingNameSortStrategy();
            }
            else if (sortOption == "ascendingPrice")
            {
                sortStrategy = new AscendingPriceSortStrategy();
            }
            else if (sortOption == "descendingPrice")
                sortStrategy = new DescendingPriceSortStrategy();
            else
                sortStrategy = new AscendingNameSortStrategy();
            List<Product> searchResults;
            if (String != null)
            {
                searchResults = _context.Products.Where(x => x.Category == String).ToList();
                if (!searchResults.Any())
                {
                    searchResults = _context.Products.Where(x => x.FlowerType == String).ToList();
                }
                if (!searchResults.Any())
                {
                    string pattern = $"{Regex.Escape(String)}";
                    searchResults = _context.Products.Where(p => Regex.IsMatch(p.Name, pattern, RegexOptions.IgnoreCase)).ToList();
                }
            }

            else
                searchResults = _context.Products.Where(x=>true).ToList();

            ViewBag.String = String;
            var sortedProducts = sortStrategy.Sort(searchResults);
            ViewBag.SelectedSortOption = sortOption;

            return PartialView("~/Views/Products/SearchResult.cshtml", sortedProducts);
        }
        // POST: Products/Create
        // To protect from over posting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]

        public async Task<IActionResult> Create([Bind("ProductID,Name,Price,Stock,FlowerType,ImageUrl,productType,SalesHistory,Category,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Employee")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // POST: Products/Edit/5
        // To protect from over posting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]

        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Name,Price,Stock,Category,Description,ImageUrl,FlowerType")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productEditor.EditAll(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                        return NotFound();
                }
                var allProducts = _context.Products.Where(x=>true).ToList();
                return View("~/Views/Home/Index.cshtml", allProducts);
            }

            return View("~/Views/Home/Index.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        //Edit name and price of products
        public async Task<IActionResult> EditNameAndPrice(int id, [Bind("ProductID,Name,Price")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productEditor.EditNameAndPrice(id, product);
                }
                catch (DbUpdateConcurrencyException)
                {
                        return NotFound();
                }
                var allProducts = _context.Products.Where(x => true).ToList();
                return View("~/Views/Home/Index.cshtml", allProducts);
            }

            return View("~/Views/Home/Index.cshtml");
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Employee")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _context.Products
                .FirstOrDefault(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }


        //Metohod for "White box" testing
        //A method that applies a discount for a product whose price is greater than 50BAM, and otherwise, adds a free gift to the product.
        public void ProcessProducts(List<Product> products)
        {
            if (products != null && products.Any())
            {
                foreach (var product in products)
                {

                    if (product.Price > 50)
                    {
                        ApplyDiscount(product);
                        Console.WriteLine("Na proizvod je uračunat popust 10%!");
                    }
                    else
                    {
                        ApplyOtherDiscount(product);
                        Console.WriteLine("Uz proizvod je dodan i besplatan poklon!");
                    }
                }
            }
            else
            {
                Console.WriteLine("Nema dostupnih proizvoda za obradu.");
            }
        }

        private void ApplyDiscount(Product product)
        {
            product.Price *= 0.9; 
        }

        private void ApplyOtherDiscount(Product product)
        {
            product.Description += " + Besplatan poklon!";
        }
    }
}
