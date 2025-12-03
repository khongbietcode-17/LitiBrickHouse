using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitiBrickHouse.Data;
using LitiBrickHouse.Enums;
using System.Linq;

namespace LitiBrickHouse.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 1. Thống kê số lượng đơn hàng theo trạng thái
            var orders = _context.Orders.ToList();

            ViewBag.CountNew = orders.Count(o => o.Status == OrderStatus.Moi);
            ViewBag.CountProcessing = orders.Count(o => o.Status == OrderStatus.DangLam);
            ViewBag.CountCompleted = orders.Count(o => o.Status == OrderStatus.HoanThanh);
            ViewBag.CountCancelled = orders.Count(o => o.Status == OrderStatus.DaHuy);

            // 2. Tính doanh thu tháng này (chỉ tính đơn "Hoàn thành")
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var monthlyRevenue = orders
                .Where(o => o.Status == OrderStatus.HoanThanh && 
                            o.OrderDate.Month == currentMonth && 
                            o.OrderDate.Year == currentYear)
                .Sum(o => o.TotalPrice);

            ViewBag.MonthlyRevenue = monthlyRevenue;

            // 3. Tính tổng doanh thu toàn thời gian
            var totalRevenue = orders
                .Where(o => o.Status == OrderStatus.HoanThanh)
                .Sum(o => o.TotalPrice);

            ViewBag.TotalRevenue = totalRevenue;

            return View();
        }
    }
}