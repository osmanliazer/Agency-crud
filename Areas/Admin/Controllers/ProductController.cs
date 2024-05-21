using Agency.DAL;
using Agency.Models;
using Agency.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Agency.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AgencyContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AgencyContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync()
            };

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                return View(productVM);
            }

            string filename = Guid.NewGuid().ToString() + Path.GetExtension(productVM.Photo.FileName);
            string path = Path.Combine(_env.WebRootPath, "assets", "img", "portfolio", filename);

            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                await productVM.Photo.CopyToAsync(file);
            }

            Product product = new Product
            {
                CategoryId = productVM.CategoryId,
                Image = filename,
                Name = productVM.Name
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
