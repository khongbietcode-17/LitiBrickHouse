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
    public class CustomOptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomOptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CustomOptions
        // (Trong file Controllers/CustomOptionsController.cs)
        // XÓA HẲN action Index CŨ, THAY BẰNG CÁI NÀY:

        // GET: CustomOptions
        public async Task<IActionResult> Index(int? categoryId) // Thêm tham số lọc
        {
            // 1. Tạo một truy vấn (query) cơ sở
            var query = _context.CustomOptions
                .Include(c => c.OptionCategory) // Luôn kèm theo thông tin Danh mục
                .OrderBy(c => c.Name); // Sắp xếp theo tên

            // 2. Nếu có lọc (categoryId), thì lọc theo ID đó
            if (categoryId != null && categoryId > 0)
            {
                query = (IOrderedQueryable<CustomOption>)query
                    .Where(c => c.OptionCategoryId == categoryId);
            }

            // 3. (ĐÂY LÀ PHẦN BỊ THIẾU)
            // Tải danh sách Danh mục để làm dropdown bộ lọc (menu bên trái)
            ViewBag.CategoryFilter = new SelectList(
                await _context.OptionCategories.OrderBy(c => c.Name).ToListAsync(),
                "Id",
                "Name",
                categoryId // Giữ lại giá trị đã lọc
            );

            // 4. Thực thi truy vấn và gửi ra View
            return View(await query.ToListAsync());
        }

        // GET: CustomOptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customOption = await _context.CustomOptions
                .Include(c => c.OptionCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customOption == null)
            {
                return NotFound();
            }

            return View(customOption);
        }

        // GET: CustomOptions/Create
        public IActionResult Create()
        {
            // Tải danh sách Danh mục từ CSDL và gửi qua ViewBag
            ViewBag.OptionCategoryId = new SelectList(
                _context.OptionCategories.OrderBy(c => c.Name), // Lấy tất cả danh mục, sắp xếp theo Tên
                "Id",   // Giá trị (value) của option (sẽ là ID)
                "Name"  // Văn bản (text) hiển thị (sẽ là Tên)
            );
            return View();
        }

        // POST: CustomOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageUrl,ThumbnailUrl,AdditionalPrice,Quantity,Gender,OptionCategoryId")] CustomOption customOption)
        {
            ModelState.Remove("OptionCategory");
            if (ModelState.IsValid)
            {
                _context.Add(customOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // SỬA LỖI: Tải lại ViewBag trước khi trả về View
            ViewBag.OptionCategoryId = new SelectList(
                _context.OptionCategories.OrderBy(c => c.Name),
                "Id",
                "Name",
                customOption.OptionCategoryId // Giữ lại giá trị đã chọn
            );
            return View(customOption);
        }

        // GET: CustomOptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customOption = await _context.CustomOptions.FindAsync(id);
            if (customOption == null)
            {
                return NotFound();
            }
            ViewData["OptionCategoryId"] = new SelectList(_context.OptionCategories, "Id", "Name", customOption.OptionCategoryId);
            return View(customOption);
        }

        // POST: CustomOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl,ThumbnailUrl,AdditionalPrice,Quantity,Gender,OptionCategoryId")] CustomOption customOption)
        {
            if (id != customOption.Id)
            {
                return NotFound();
            }
            ModelState.Remove("OptionCategory");
            if (ModelState.IsValid)
            {

                try
                {
                    _context.Update(customOption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomOptionExists(customOption.Id))
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
            ViewData["OptionCategoryId"] = new SelectList(_context.OptionCategories, "Id", "Name", customOption.OptionCategoryId);
            return View(customOption);
        }

        // GET: CustomOptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customOption = await _context.CustomOptions
                .Include(c => c.OptionCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customOption == null)
            {
                return NotFound();
            }

            return View(customOption);
        }

        // POST: CustomOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customOption = await _context.CustomOptions.FindAsync(id);
            if (customOption != null)
            {
                _context.CustomOptions.Remove(customOption);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomOptionExists(int id)
        {
            return _context.CustomOptions.Any(e => e.Id == id);
        }
    }
}
