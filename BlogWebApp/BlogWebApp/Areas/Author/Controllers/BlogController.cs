﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWebApp.Data;
using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;
using BlogWebApp.Models.ViewModels;
using BlogWebApp.Data.Migrations;

namespace BlogWebApp.Areas.Author.Controllers
{
    [Area("Author")]
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _applicationUser;

        public BlogController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> applicationUser)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _applicationUser = applicationUser;

        }

        // GET: Author/Blog
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Blog.Include(b => b.ApplicationUser).Include(b => b.Category).Include(b => b.SubCategory);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Blog == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .Include(b => b.ApplicationUser)
                .Include(b => b.Category)
                .Include(b => b.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        //GET: Admin/Blog/GetSubCategoryList
        //public IActionResult GetSubCategoryList(int Id)
        //{
        //    var subCategoryList = _context.SubCategory.GetAll().Where(u => u.CategoryId == Id).ToList();
        //    return Json(SubCategoryList);
        //}

        // GET: Admin/Blog/Create
        public IActionResult Create()
        {
            BlogVM obj = new()
            {
                blog = new(),
                CategoryList = new SelectList(_context.Category, "Id", "CategoryName"),
                SubCategoryList = new SelectList(_context.SubCategory, "Id", "SubCategoryName")
  
            };
            return View(obj);
        }

        // POST: Admin/Blog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(BlogVM obj, IFormFile? file)
        {
         
            //if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"blogImages/titleImages/");
                    var extension = Path.GetExtension(file.FileName);
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    obj.blog.TitleImageUrl = @"\blogImages\titleImages\" + fileName + extension;
                }

                obj.blog.CreatedBy = _applicationUser.GetUserId(HttpContext.User);
                obj.blog.ApplicationUserId = _applicationUser.GetUserId(HttpContext.User);
                _context.Blog.Add(obj.blog);
                _context.SaveChanges();

                TempData["success"] = "Blog created successfully";
                return RedirectToAction("Index");

            }
            return View(obj);
        }
   

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Blog == null)
            {
                return NotFound();
            }

            var blogObj = await _context.Blog.FindAsync(id);
            if (blogObj == null)
            {
                //return NotFound();
                return RedirectToAction("Index");
            }

            BlogVM obj = new()
            {
                blog = blogObj,
                CategoryList = new SelectList(_context.Category, "Id", "CategoryName"),
                SubCategoryList = new SelectList(_context.SubCategory, "Id", "SubCategoryName")
 
            };

            return View(obj);
        }

        // POST: Admin/Blog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BlogVM obj, IFormFile? file)
        {
            obj.CategoryList = new SelectList(_context.Category, "Id", "CategoryName");
            obj.SubCategoryList = new SelectList(_context.SubCategory, "Id", "SubCategoryName");

            //if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    Blog objBlog = new Blog();
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"blogImages/titleImages/");
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.blog.TitleImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.blog.TitleImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    obj.blog.TitleImageUrl = @"\blogImages\titleImages\" + fileName + extension;
                    obj.blog.ModifiedBy = _applicationUser.GetUserId(HttpContext.User);
                    obj.blog.ModifiedDate = DateTime.Now;
                    obj.blog.ApplicationUserId = _applicationUser.GetUserId(HttpContext.User);
                }

                _context.Update(obj.blog);
                await _context.SaveChangesAsync();

                TempData["success"] = "Blog created successfully";
                return RedirectToAction("Index");

            }
            return View(obj);
        }

        // GET: Admin/Blog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Blog == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .Include(b => b.ApplicationUser)
                .Include(b => b.Category)
                .Include(b => b.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Admin/Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Blog == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Blog'  is null.");
            }
            var blog = await _context.Blog.FindAsync(id);
            if (blog != null)
            {
                _context.Blog.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return (_context.Blog?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
