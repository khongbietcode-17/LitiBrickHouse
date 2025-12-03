using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LitiBrickHouse.Data;
using LitiBrickHouse.Models;
using Microsoft.AspNetCore.Hosting; // Đã có
using Microsoft.AspNetCore.Http;    // Đã có
using System.IO;                    // Đã có

namespace LitiBrickHouse.Controllers
{
    public class CustomOptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // --- 1. SỬA LẠI CONSTRUCTOR (Thêm tham số webHostEnvironment) ---
        public CustomOptionsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // Bây giờ dòng này mới hoạt động đúng
        }

        public async Task<IActionResult> Index(int? categoryId, string filterType)
        {
            // (Giữ nguyên code Index của bạn - không thay đổi)
            var query = _context.CustomOptions
                .Include(c => c.OptionCategory)
                .AsQueryable();

            if (filterType == "OutOfStock")
            {
                query = query.Where(x => x.Quantity <= 0);
                ViewBag.CurrentFilter = "Hàng cần nhập (Hết hàng)";
            }
            else if (filterType == "LowStock")
            {
                query = query.Where(x => x.Quantity > 0 && x.Quantity < 5);
                ViewBag.CurrentFilter = "Hàng sắp hết (SL < 5)";
            }
            else if (categoryId.HasValue)
            {
                query = query.Where(x => x.OptionCategoryId == categoryId);
                var catName = await _context.OptionCategories
                    .Where(c => c.Id == categoryId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();
                ViewBag.CurrentFilter = catName;
                ViewBag.CurrentCategoryId = categoryId;
            }
            else
            {
                ViewBag.CurrentFilter = "Tất cả phụ kiện";
            }

            ViewBag.Categories = await _context.OptionCategories.ToListAsync();
            ViewBag.ActiveFilter = filterType;

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customOption = await _context.CustomOptions
                .Include(c => c.OptionCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customOption == null) return NotFound();

            return View(customOption);
        }

        public IActionResult Create()
        {
            ViewBag.OptionCategoryId = new SelectList(
                _context.OptionCategories.OrderBy(c => c.Name),
                "Id",
                "Name"
            );
            return View();
        }

        // --- 2. SỬA LẠI HÀM CREATE (POST) ĐỂ UPLOAD ẢNH ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomOption customOption, IFormFile fImage, IFormFile fThumb)
        {
            // Bỏ qua lỗi validation của ImageUrl/ThumbnailUrl vì ta sẽ tự tạo chúng
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ThumbnailUrl");
            ModelState.Remove("OptionCategory"); 

            if (ModelState.IsValid)
            {
                // -- Xử lý Ảnh Chính (fImage) --
                if (fImage != null)
                {
                    // Tạo tên file ngẫu nhiên để tránh trùng tên
                    string folderName = "images/options/";
                    string fileName = Guid.NewGuid().ToString() + "_" + fImage.FileName;
                    
                    // Đường dẫn vật lý trên server (C:\...\wwwroot\images\options\)
                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

                    // Nếu chưa có thư mục thì tạo mới
                    if (!Directory.Exists(serverFolder)) Directory.CreateDirectory(serverFolder);

                    // Copy file vào thư mục
                    string filePath = Path.Combine(serverFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fImage.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn web (ví dụ: /images/options/abc.jpg) vào Database
                    customOption.ImageUrl = "/" + folderName + fileName;
                }

                // -- Xử lý Ảnh Nhỏ (fThumb) --
                if (fThumb != null)
                {
                    string folderName = "images/options/";
                    string fileName = "thumb_" + Guid.NewGuid().ToString() + "_" + fThumb.FileName;
                    string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

                    string filePath = Path.Combine(serverFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fThumb.CopyToAsync(stream);
                    }
                    customOption.ThumbnailUrl = "/" + folderName + fileName;
                }
                else
                {
                    // Nếu không chọn ảnh nhỏ, dùng luôn ảnh chính làm ảnh nhỏ
                    customOption.ThumbnailUrl = customOption.ImageUrl;
                }

                _context.Add(customOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi (ví dụ chưa điền tên), load lại danh sách danh mục
            ViewBag.OptionCategoryId = new SelectList(
                _context.OptionCategories.OrderBy(c => c.Name),
                "Id",
                "Name",
                customOption.OptionCategoryId
            );
            return View(customOption);
        }

        // --- CÁC HÀM CÒN LẠI GIỮ NGUYÊN ---
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customOption = await _context.CustomOptions.FindAsync(id);
            if (customOption == null) return NotFound();
            
            ViewData["OptionCategoryId"] = new SelectList(_context.OptionCategories, "Id", "Name", customOption.OptionCategoryId);
            return View(customOption);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl,ThumbnailUrl,AdditionalPrice,Quantity,Gender,OptionCategoryId")] CustomOption customOption)
        {
            if (id != customOption.Id) return NotFound();

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
                    if (!CustomOptionExists(customOption.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OptionCategoryId"] = new SelectList(_context.OptionCategories, "Id", "Name", customOption.OptionCategoryId);
            return View(customOption);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var customOption = await _context.CustomOptions
                .Include(c => c.OptionCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customOption == null) return NotFound();

            return View(customOption);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customOption = await _context.CustomOptions.FindAsync(id);
            if (customOption != null) _context.CustomOptions.Remove(customOption);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomOptionExists(int id)
        {
            return _context.CustomOptions.Any(e => e.Id == id);
        }
    }
}