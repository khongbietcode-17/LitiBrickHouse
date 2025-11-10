using LitiBrickHouse.Data;
using LitiBrickHouse.Extensions;
using LitiBrickHouse.Models;
using LitiBrickHouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic; // <-- Thêm dòng này để dùng List

namespace LitiBrickHouse.Controllers
{
    public class OrderController : Controller
    {
        // 1. Kết nối đến Database
        private readonly ApplicationDbContext _context;
        // 2. Tên của Session key
        private const string OrderSessionKey = "CurrentOrder";

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Phương thức này được gọi từ nút "Đặt Hàng" ở Trang chủ
        public IActionResult StartNewOrder()
        {
            // Tạo một ViewModel rỗng và lưu vào Session
            var viewModel = new OrderBuilderViewModel();
            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);

            // Chuyển đến Giai đoạn 1
            return RedirectToAction("Stage1_SelectType");
        }

        // ===================================================================
        // GIAI ĐOẠN 1: CHỌN LOẠI (1 Lego, 2 Lego, Sport)
        // ===================================================================

        // GET: /Order/Stage1_SelectType
        [HttpGet]
        public async Task<IActionResult> Stage1_SelectType()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null)
            {
                // Nếu session mất, bắt đầu lại
                return RedirectToAction("StartNewOrder");
            }

            // Lấy các loại sản phẩm (1 Lego, 2 Lego, Sport) từ DB
            var productTypes = await _context.ProductTypes.ToListAsync();

            return View(productTypes);
        }

        // POST: /Order/Stage1_SelectType
        [HttpPost]
        public IActionResult Stage1_SelectType(int productTypeId) // Nhận ID từ form
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            // Lưu ProductTypeId vào Session
            viewModel.ProductTypeId = productTypeId;
            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);

            // Chuyển đến Giai đoạn 2
            return RedirectToAction("Stage2_SelectBackground");
        }

        // ===================================================================
        // GIAI ĐOẠN 2: CHỌN BACKGROUND & GHI CHÚ
        // ===================================================================

        // GET: /Order/Stage2_SelectBackground
        [HttpGet]
        public async Task<IActionResult> Stage2_SelectBackground()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            // Kiểm tra xem Stage 1 đã xong chưa
            if (viewModel == null || viewModel.ProductTypeId == null)
            {
                return RedirectToAction("Stage1_SelectType");
            }

            // --- LOGIC LỌC 3 LOẠI BACKGROUND MỚI ---
            string categoryName;
            int productTypeId = viewModel.ProductTypeId.Value;

            if (productTypeId == 1)
            {
                // Tên danh mục này phải khớp với CSDL (dữ liệu mồi)
                categoryName = "background (1 lego)";
            }
            else if (productTypeId == 2)
            {
                // Tên danh mục này phải khớp với CSDL (dữ liệu mồi)
                categoryName = "background (2 lego)";
            }
            else // (Giả sử là loại 3 - Sport)
            {
                // Tên danh mục này phải khớp với CSDL (dữ liệu mồi)
                categoryName = "background (sport)";
            }
            // --- KẾT THÚC LOGIC LỌC ---

            var backgroundOptions = await _context.CustomOptions
                .Include(opt => opt.OptionCategory)
                .Where(opt => opt.OptionCategory.Name.ToLower() == categoryName)
                .Where(opt => opt.Quantity > 0) // Chỉ hiển thị cái còn hàng
                .ToListAsync();

            ViewBag.AvailableBackgrounds = backgroundOptions;

            // Truyền viewModel (chứa BackgroundId, CustomerNote đã chọn) vào View
            return View(viewModel);
        }

        // POST: /Order/Stage2_SelectBackground
        [HttpPost]
        public IActionResult Stage2_SelectBackground(OrderBuilderViewModel formViewModel)
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            // Lưu BackgroundId và Ghi chú người dùng nhập từ form vào Session
            viewModel.BackgroundId = formViewModel.BackgroundId;
            viewModel.CustomerNote = formViewModel.CustomerNote;

            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);

            // Chuyển đến Giai đoạn 3
            return RedirectToAction("Stage3_Customize");
        }

        // ===================================================================
        // GIAI ĐOẠN 3: TÙY CHỈNH SẢN PHẨM (1 Lego, 2 Lego, Sport)
        // ===================================================================

        // GET: /Order/Stage3_Customize
        [HttpGet]
        public async Task<IActionResult> Stage3_Customize()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            // Kiểm tra xem Stage 2 đã xong chưa
            if (viewModel == null || viewModel.BackgroundId == null)
            {
                return RedirectToAction("Stage2_SelectBackground");
            }

            int productTypeId = viewModel.ProductTypeId.Value;

            if (productTypeId == 1 || productTypeId == 2)
            {
                // --- TRƯỜNG HỢP 1 & 2: TÙY CHỈNH LEGO ---

                // --- ĐÃ CẬP NHẬT: Tải tất cả các bộ phận ---
                ViewBag.AvailableHairs = await _context.CustomOptions.Include(o => o.OptionCategory)
                    .Where(o => o.OptionCategory.Name.ToLower() == "tóc" && (o.Quantity == null || o.Quantity > 0)).ToListAsync();

                ViewBag.AvailableFaces = await _context.CustomOptions.Include(o => o.OptionCategory)
                    .Where(o => o.OptionCategory.Name.ToLower() == "mặt" && (o.Quantity == null || o.Quantity > 0)).ToListAsync();

                ViewBag.AvailableClothes = await _context.CustomOptions.Include(o => o.OptionCategory)
                    .Where(o => o.OptionCategory.Name.ToLower() == "áo" && (o.Quantity == null || o.Quantity > 0)).ToListAsync();

                ViewBag.AvailablePants = await _context.CustomOptions.Include(o => o.OptionCategory)
                    .Where(o => o.OptionCategory.Name.ToLower() == "quần" && (o.Quantity == null || o.Quantity > 0)).ToListAsync();

                ViewBag.AvailableAccessories = await _context.CustomOptions.Include(o => o.OptionCategory)
                    .Where(o => o.OptionCategory.Name.ToLower() == "phụ kiện" && (o.Quantity == null || o.Quantity > 0)).ToListAsync();
                // --- KẾT THÚC CẬP NHẬT ---

                // Dùng chung 1 View "Stage3_CustomizeLego.cshtml"
                return View("Stage3_CustomizeLego", viewModel);
            }
            else if (productTypeId == 3)
            {
                // --- TRƯỜNG HỢP 3: CHỌN LEGO SPORT (CẦU THỦ) ---
                // Tải danh sách các cầu thủ (còn hàng)
                ViewBag.AvailableSportPlayers = await _context.CustomOptions.Include(o => o.OptionCategory)
                    .Where(o => o.OptionCategory.Name.ToLower() == "cầu thủ" && (o.Quantity == null || o.Quantity > 0)).ToListAsync();

                return View("Stage3_SelectSportPlayer", viewModel);
            }

            // Nếu không có productTypeId hợp lệ, quay về Giai đoạn 1
            return RedirectToAction("Stage1_SelectType");
        }

        // POST: /Order/Stage3_CustomizeLego (Dùng cho Loại 1 & 2)
        [HttpPost]
        public IActionResult Stage3_CustomizeLego(OrderBuilderViewModel formViewModel)
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            // Cập nhật thông tin Lego 1 từ form
            viewModel.Lego1 = formViewModel.Lego1;

            // Nếu là loại "2 Lego", cập nhật luôn Lego 2 từ form
            if (viewModel.ProductTypeId == 2)
            {
                viewModel.Lego2 = formViewModel.Lego2;
            }

            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);

            // Chuyển đến Giai đoạn 4
            return RedirectToAction("Stage4_ShippingInfo");
        }

        // POST: /Order/Stage3_SelectSportPlayer (Dùng cho Loại 3)
        [HttpPost]
        public IActionResult Stage3_SelectSportPlayer(OrderBuilderViewModel formViewModel)
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            // Cập nhật cầu thủ đã chọn từ form
            viewModel.SportPlayerId = formViewModel.SportPlayerId;

            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);

            // Chuyển đến Giai đoạn 4
            return RedirectToAction("Stage4_ShippingInfo");
        }

        // ===================================================================
        // GIAI ĐOẠN 4: THÔNG TIN GIAO HÀNG
        // ===================================================================

        // GET: /Order/Stage4_ShippingInfo
        [HttpGet]
        public IActionResult Stage4_ShippingInfo()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            // Kiểm tra xem Giai đoạn 3 đã xong chưa
            if (viewModel == null || (viewModel.Lego1.FaceId == null && viewModel.SportPlayerId == null))
            {
                // (Đây là logic kiểm tra tạm, nếu khách chưa chọn FaceId hoặc SportPlayerId thì quay lại)
                return RedirectToAction("Stage3_Customize");
            }

            // Truyền viewModel vào View để hiển thị lại thông tin nếu đã có
            return View(viewModel);
        }

        [HttpPost]
        [ActionName("Stage4_ShippingInfo")] // Giữ tên này cho View
        public IActionResult Stage4_ShippingInfo_POST() // Giữ tên _POST
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            // === PHẦN SỬA LỖI: LẤY VÀ KIỂM TRA THỦ CÔNG ===

            // 1. Lấy 3 giá trị "thủ công" từ Form
            string socialMediaName = Request.Form["SocialMediaName"];
            string customerName = Request.Form["CustomerName"];
            string phoneNumber = Request.Form["PhoneNumber"];
            string address = Request.Form["Address"];
            
            // 2. Kiểm tra "thủ công"
            bool isValid = true;
            if (string.IsNullOrEmpty(customerName))
            {
                // Thêm lỗi vào ModelState để View hiển thị
                ModelState.AddModelError("CustomerName", "Vui lòng nhập tên");
                isValid = false;
            }
            if (string.IsNullOrEmpty(phoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Vui lòng nhập SĐT");
                isValid = false;
            }
            if (string.IsNullOrEmpty(address))
            {
                ModelState.AddModelError("Address", "Vui lòng nhập địa chỉ");
                isValid = false;
            }

            // 3. Nếu không hợp lệ
            if (!isValid)
            {
                // Gửi lại 3 giá trị đã nhập để người dùng không phải gõ lại
                viewModel.CustomerName = customerName;
                viewModel.PhoneNumber = phoneNumber;
                viewModel.Address = address;

                // Trả về View, lỗi đỏ sẽ hiển thị
                return View("Stage4_ShippingInfo", viewModel);
            }

            // 4. Nếu hợp lệ
            viewModel.SocialMediaName = socialMediaName;
            viewModel.CustomerName = customerName;
            viewModel.PhoneNumber = phoneNumber;
            viewModel.Address = address;

            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);

            // 5. Chuyển đến Giai đoạn 5
            return RedirectToAction("Stage5_Confirm");
        }

        // ===================================================================
        // GIAI ĐOẠN 5: XÁC NHẬN VÀ TỔNG KẾT
        // ===================================================================

        // GET: /Order/Stage5_Confirm
        [HttpGet]
        public async Task<IActionResult> Stage5_Confirm()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            // Kiểm tra xem Stage 4 đã xong chưa
            if (viewModel == null || string.IsNullOrEmpty(viewModel.CustomerName))
            {
                return RedirectToAction("Stage4_ShippingInfo");
            }

            // --- Tải chi tiết thông tin (Tên, Giá) để hiển thị ---
            viewModel.SelectedProductType = await _context.ProductTypes.FindAsync(viewModel.ProductTypeId);
            viewModel.SelectedBackground = await _context.CustomOptions.FindAsync(viewModel.BackgroundId);

            decimal totalPrice = 0;

            // 1. Cộng giá gốc của Loại sản phẩm
            totalPrice += viewModel.SelectedProductType?.BasePrice ?? 0;

            // 2. Cộng giá của Background (nếu có)
            totalPrice += viewModel.SelectedBackground?.AdditionalPrice ?? 0;

            // 3. Cộng giá của Tùy chỉnh
            if (viewModel.ProductTypeId == 1 || viewModel.ProductTypeId == 2) // Lego Tùy chỉnh
            {
                // Tính giá cho Lego 1
                var hair1 = await _context.CustomOptions.FindAsync(viewModel.Lego1.HairId);
                var face1 = await _context.CustomOptions.FindAsync(viewModel.Lego1.FaceId);
                // (Thêm code tải giá cho Áo, Quần, Phụ kiện...)
                totalPrice += hair1?.AdditionalPrice ?? 0;
                totalPrice += face1?.AdditionalPrice ?? 0;

                if (viewModel.ProductTypeId == 2)
                {
                    // Tính giá cho Lego 2
                    var hair2 = await _context.CustomOptions.FindAsync(viewModel.Lego2.HairId);
                    var face2 = await _context.CustomOptions.FindAsync(viewModel.Lego2.FaceId);
                    // (Thêm code tải giá cho Áo, Quần, Phụ kiện...)
                    totalPrice += hair2?.AdditionalPrice ?? 0;
                    totalPrice += face2?.AdditionalPrice ?? 0;
                }
            }
            else if (viewModel.ProductTypeId == 3) // Lego Sport
            {
                viewModel.SelectedSportPlayer = await _context.CustomOptions.FindAsync(viewModel.SportPlayerId);
                totalPrice += viewModel.SelectedSportPlayer?.AdditionalPrice ?? 0;
            }

            // Lưu tổng giá cuối cùng vào ViewModel (để hiển thị và lưu vào DB)
            viewModel.TotalPrice = totalPrice;
            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);

            return View(viewModel);
        }

        // POST: /Order/Stage5_Confirm
        [HttpPost]
        public async Task<IActionResult> Stage5_CompleteOrder()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            // 1. Tạo đối tượng Order chính
            var order = new Order
            {
                OrderDate = DateTime.Now,
                SocialMediaName = viewModel.SocialMediaName,
                CustomerName = viewModel.CustomerName,
                PhoneNumber = viewModel.PhoneNumber,
                Address = viewModel.Address,
                Note = viewModel.CustomerNote,
                TotalPrice = viewModel.TotalPrice,
                CustomLegos = new List<OrderCustomLego>(),
                OrderItems = new List<OrderItem>()
            };

            // 2. Lưu các món đồ (Background, Cầu thủ...)
            if (viewModel.BackgroundId != null)
            {
                order.OrderItems.Add(new OrderItem { CustomOptionId = viewModel.BackgroundId.Value });

                // --- LOGIC SỬA LỖI ---
                // Chỉ trừ tồn kho nếu món đồ này CÓ quản lý số lượng
                var bgItem = await _context.CustomOptions.FindAsync(viewModel.BackgroundId.Value);
                if (bgItem != null && bgItem.Quantity != null) // Kiểm tra != null
                {
                    bgItem.Quantity -= 1;
                }
                // --- KẾT THÚC SỬA LỖI ---
            }

            if (viewModel.SportPlayerId != null) // Nếu là Lego Sport
            {
                order.OrderItems.Add(new OrderItem { CustomOptionId = viewModel.SportPlayerId.Value });

                // --- LOGIC SỬA LỖI ---
                var playerItem = await _context.CustomOptions.FindAsync(viewModel.SportPlayerId.Value);
                if (playerItem != null && playerItem.Quantity != null) // Kiểm tra != null
                {
                    playerItem.Quantity -= 1;
                }
                // --- KẾT THÚC SỬA LỖI ---
            }

            // 3. Lưu các bộ Lego tùy chỉnh (Loại 1 & 2)

            // Hàm trợ giúp để trừ tồn kho (sẽ dùng cho Lego 1 và 2)
            async Task SubtractStock(int? customOptionId)
            {
                if (customOptionId != null)
                {
                    var item = await _context.CustomOptions.FindAsync(customOptionId.Value);
                    if (item != null && item.Quantity != null) // Kiểm tra != null
                    {
                        item.Quantity -= 1;
                    }
                }
            }

            if (viewModel.ProductTypeId == 1 || viewModel.ProductTypeId == 2)
            {
                // Lưu Lego 1
                var lego1 = new OrderCustomLego
                {
                    HairId = viewModel.Lego1.HairId,
                    FaceId = viewModel.Lego1.FaceId,
                    ClothesId = viewModel.Lego1.ClothesId,
                    PantsId = viewModel.Lego1.PantsId,
                    Accessory1Id = viewModel.Lego1.Accessory1Id,
                    Accessory2Id = viewModel.Lego1.Accessory2Id
                };
                order.CustomLegos.Add(lego1);

                // Trừ tồn kho 6 món của Lego 1
                await SubtractStock(lego1.HairId);
                await SubtractStock(lego1.FaceId);
                await SubtractStock(lego1.ClothesId);
                await SubtractStock(lego1.PantsId);
                await SubtractStock(lego1.Accessory1Id);
                await SubtractStock(lego1.Accessory2Id);
            }
            if (viewModel.ProductTypeId == 2)
            {
                // Lưu Lego 2
                var lego2 = new OrderCustomLego
                {
                    HairId = viewModel.Lego2.HairId,
                    FaceId = viewModel.Lego2.FaceId,
                    ClothesId = viewModel.Lego2.ClothesId,
                    PantsId = viewModel.Lego2.PantsId,
                    Accessory1Id = viewModel.Lego2.Accessory1Id,
                    Accessory2Id = viewModel.Lego2.Accessory2Id
                };
                order.CustomLegos.Add(lego2);

                // Trừ tồn kho 6 món của Lego 2
                await SubtractStock(lego2.HairId);
                await SubtractStock(lego2.FaceId);
                await SubtractStock(lego2.ClothesId);
                await SubtractStock(lego2.PantsId);
                await SubtractStock(lego2.Accessory1Id);
                await SubtractStock(lego2.Accessory2Id);
            }

            // 4. Thêm Order vào Context
            _context.Orders.Add(order);

            // 5. Lưu tất cả thay đổi vào CSDL
            await _context.SaveChangesAsync();

            // 6. Xóa Session và chuyển đến trang Cảm ơn
            HttpContext.Session.Remove(OrderSessionKey);

            return RedirectToAction("ThankYou", new { id = order.Id });
        }
    }
}