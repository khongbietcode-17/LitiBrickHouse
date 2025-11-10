using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LitiBrickHouse.Data;
using LitiBrickHouse.Models;

namespace LitiBrickHouse.Controllers
{
    public class OptionCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OptionCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OptionCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.OptionCategories.ToListAsync());
        }

        // GET: OptionCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionCategory = await _context.OptionCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (optionCategory == null)
            {
                return NotFound();
            }

            return View(optionCategory);
        }

        // GET: OptionCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OptionCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] OptionCategory optionCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(optionCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(optionCategory);
        }

        // GET: OptionCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionCategory = await _context.OptionCategories.FindAsync(id);
            if (optionCategory == null)
            {
                return NotFound();
            }
            return View(optionCategory);
        }

        // POST: OptionCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] OptionCategory optionCategory)
        {
            if (id != optionCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(optionCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OptionCategoryExists(optionCategory.Id))
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
            return View(optionCategory);
        }

        // GET: OptionCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionCategory = await _context.OptionCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (optionCategory == null)
            {
                return NotFound();
            }

            return View(optionCategory);
        }

        // POST: OptionCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var optionCategory = await _context.OptionCategories.FindAsync(id);
            if (optionCategory != null)
            {
                _context.OptionCategories.Remove(optionCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OptionCategoryExists(int id)
        {
            return _context.OptionCategories.Any(e => e.Id == id);
        }
    }
}
