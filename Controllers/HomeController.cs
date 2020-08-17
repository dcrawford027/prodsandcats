using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductsAndCategories.Models;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private PacContext db;

        public HomeController(PacContext context)
        {
            db = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToAction("Categories");
        }

        [HttpGet("{categoryId}")]
        public IActionResult Category(int categoryId)
        {
            Category category = db.Categories
                .Include(cat => cat.Products)
                .ThenInclude(association => association.Product)
                .FirstOrDefault(cat => cat.CategoryId == categoryId);

            List<Product> products = db.Products
                .Include(prod => prod.Categories)
                .Where(prod => prod.Categories.Any(cat => cat.CategoryId == categoryId) == false)
                .ToList();

            List<Association> associations = db.Associations.ToList();

            // foreach (var association in associations)
            // {
            //     Product product = association.Product;
            //     Association relation = category.Products.FirstOrDefault(ass => ass.ProductId == product.ProductId);
            //     Product relatedProduct = relation == null ? null : relation.Product;
            //     if (relatedProduct != null)
            //     {
            //         products.Remove(relatedProduct);
            //     }

            // }
            

            ViewBag.Products = products;

            return View("Index", category);
        }

        [HttpPost("{categoryId}/add_product")]
        public IActionResult AddProductToCategory(int productId, int categoryId, Association newAssociation)
        {
            newAssociation.ProductId = productId;
            newAssociation.CategoryId = categoryId;
            db.Associations.Add(newAssociation);
            db.SaveChanges();
            return RedirectToAction("Categories");
        }

        [HttpGet("categories")]
        public IActionResult Categories()
        {
            List<Category> categories = db.Categories.ToList();
            ViewBag.Categories = categories;
            return View("Categories");
        }

        [HttpPost("categories/create")]
        public IActionResult CreateCategory(Category newCategory)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(newCategory);
                db.SaveChanges();
                return RedirectToAction("Categories");
            }
            return View("Categories");
        }

        [HttpGet("products")]
        public IActionResult Products()
        {
            List<Product> products = db.Products.ToList();
            ViewBag.Products = products;
            return View("Products");
        }

        [HttpPost("products/create")]
        public IActionResult CreateProduct(Product newProduct)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(newProduct);
                db.SaveChanges();
                return RedirectToAction("Products");
            }
            return View("Products");
        }

        [HttpGet("products/{productId}")]
        public IActionResult Product(int productId)
        {
            var product = db.Products
                .Include(prod => prod.Categories)
                .ThenInclude(association => association.Category)
                .FirstOrDefault(prod => prod.ProductId == productId);

            List<Category> categories = db.Categories
                .Where(cat => cat.Products.Any(prod => prod.ProductId == productId) == false)
                .ToList();
            ViewBag.Categories = categories;

            return View("Product", product);
        }

        [HttpPost("products/{productId}/add_category")]
        public IActionResult AddCategoryToProduct(int productId, int categoryId, Association newAssociation)
        {
            newAssociation.ProductId = productId;
            newAssociation.CategoryId = categoryId;
            db.Associations.Add(newAssociation);
            db.SaveChanges();
            return RedirectToAction("Products");
        }

    }
}
