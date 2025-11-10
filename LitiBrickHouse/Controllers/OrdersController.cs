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
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        // (Trong file Controllers/OrdersController.cs)
        // THAY THẾ action Index CŨ BẰNG CÁI NÀY:

        public async Task<IActionResult> Index(Enums.OrderStatus status = Enums.OrderStatus.Moi)
        {
            // Mặc định, chỉ hiển thị đơn "Mới"
            var query = _context.Orders
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate); // Sắp xếp đơn mới nhất lên đầu

            // Gửi trạng thái hiện tại ra View
            ViewBag.CurrentStatus = status;

            return View(await query.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // --- BẮT ĐẦU NÂNG CẤP ---
            // Tải đơn hàng VÀ tất cả các chi tiết liên quan
            var order = await _context.Orders

                // 1. Tải các món "đơn lẻ" (Background, Cầu thủ)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.CustomOption) // Lấy thông tin (Tên, Giá) của món đó

                // 2. Tải các bộ "Lego tùy chỉnh"
                .Include(o => o.CustomLegos)
                    // Lấy thông tin (Tên) của 6 món trong bộ Lego
                    .ThenInclude(cl => cl.Hair)
                .Include(o => o.CustomLegos)
                    .ThenInclude(cl => cl.Face)
                .Include(o => o.CustomLegos)
                    .ThenInclude(cl => cl.Clothes)
                .Include(o => o.CustomLegos)
                    .ThenInclude(cl => cl.Pants)
                .Include(o => o.CustomLegos)
                    .ThenInclude(cl => cl.Accessory1)
                .Include(o => o.CustomLegos)
                    .ThenInclude(cl => cl.Accessory2)

                .FirstOrDefaultAsync(m => m.Id == id);
            // --- KẾT THÚC NÂNG CẤP ---

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SocialMediaName,CustomerName,PhoneNumber,Address,Note,OrderDate,TotalPrice")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SocialMediaName,CustomerName,PhoneNumber,Address,Note,OrderDate,TotalPrice")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
