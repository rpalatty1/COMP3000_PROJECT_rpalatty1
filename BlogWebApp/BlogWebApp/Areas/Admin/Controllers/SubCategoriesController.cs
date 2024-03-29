﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWebApp.Data;
using BlogWebApp.Models;
using BlogWebApp.Models.ViewModels;

namespace BlogWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SubCategories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SubCategory.Include(s => s.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/SubCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SubCategory == null)
            {
                return NotFound();
            }

            var subCategory = await _context.SubCategory
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        // GET: Admin/SubCategories/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName");
            return View();
        }

        // POST: Admin/SubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubCategoryName,IsActive,CategoryId,Id,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] SubCategory subCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", subCategory.CategoryId);
            return View(subCategory);
        }

        // GET: Admin/SubCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SubCategory == null)
            {
                return NotFound();
            }

            var subCategory = await _context.SubCategory.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", subCategory.CategoryId);
            return View(subCategory);
        }

        // POST: Admin/SubCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubCategoryName,IsActive,CategoryId,Id,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy")] SubCategory subCategory)
        {
            if (id != subCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubCategoryExists(subCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", subCategory.CategoryId);
            return View(subCategory);
        }

        // GET: Admin/SubCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SubCategory == null)
            {
                return NotFound();
            }

            var subCategory = await _context.SubCategory
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        // POST: Admin/SubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SubCategory == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SubCategory'  is null.");
            }
            var subCategory = await _context.SubCategory.FindAsync(id);
            if (subCategory != null)
            {
                _context.SubCategory.Remove(subCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubCategoryExists(int id)
        {
          return (_context.SubCategory?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
