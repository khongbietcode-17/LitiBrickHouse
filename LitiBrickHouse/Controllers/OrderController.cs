using LitiBrickHouse.Data;
using LitiBrickHouse.Extensions;
using LitiBrickHouse.Models;
using LitiBrickHouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace LitiBrickHouse.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string OrderSessionKey = "CurrentOrder";

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult StartNewOrder()
        {
            var viewModel = new OrderBuilderViewModel();
            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);
            return RedirectToAction("Stage1_SelectType");
        }

        // ===================================================================
        // GIAI ĐOẠN 1: CHỌN LOẠI
        // ===================================================================
        [HttpGet]
        public async Task<IActionResult> Stage1_SelectType()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            var productTypes = await _context.ProductTypes.ToListAsync();
            return View(productTypes);
        }

        [HttpPost]
        public IActionResult Stage1_SelectType(int productTypeId)
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            viewModel.ProductTypeId = productTypeId;
            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);
            return RedirectToAction("Stage2_SelectBackground");
        }

        // ===================================================================
        // GIAI ĐOẠN 2: CHỌN BACKGROUND
        // ===================================================================
        [HttpGet]
        public async Task<IActionResult> Stage2_SelectBackground()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null || viewModel.ProductTypeId == null) return RedirectToAction("Stage1_SelectType");

            string categoryName;
            int productTypeId = viewModel.ProductTypeId.Value;

            if (productTypeId == 1) categoryName = "background (1 lego)";
            else if (productTypeId == 2) categoryName = "background (2 lego)";
            else categoryName = "background (sport)";

            var backgroundOptions = await _context.CustomOptions
                .Include(opt => opt.OptionCategory)
                .Where(opt => opt.OptionCategory.Name.ToLower() == categoryName)
                .Where(opt => opt.Quantity == null || opt.Quantity > 0)
                .ToListAsync();

            ViewBag.AvailableBackgrounds = backgroundOptions;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Stage2_SelectBackground(OrderBuilderViewModel formViewModel)
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            viewModel.BackgroundId = formViewModel.BackgroundId;
            viewModel.CustomerNote = formViewModel.CustomerNote;
            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);
            return RedirectToAction("Stage3_Customize");
        }

        // ===================================================================
        // GIAI ĐOẠN 3: TÙY CHỈNH LEGO
        // ===================================================================
        [HttpGet]
        public async Task<IActionResult> Stage3_Customize()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null || viewModel.BackgroundId == null) return RedirectToAction("Stage2_SelectBackground");

            int productTypeId = viewModel.ProductTypeId.Value;

            if (productTypeId == 1 || productTypeId == 2)
            {
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

                return View("Stage3_CustomizeLego", viewModel);
            }
            else // Lego Sport
            {
                ViewBag.AvailableSportPlayers = await _context.CustomOptions.Include(o => o.OptionCategory)
                    .Where(o => o.OptionCategory.Name.ToLower() == "cầu thủ" && (o.Quantity == null || o.Quantity > 0)).ToListAsync();
                return View("Stage3_SelectSportPlayer", viewModel);
            }
        }

        [HttpPost]
        public IActionResult Stage3_CustomizeLego(OrderBuilderViewModel formViewModel)
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            viewModel.Lego1 = formViewModel.Lego1;
            if (viewModel.ProductTypeId == 2) viewModel.Lego2 = formViewModel.Lego2;

            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);
            return RedirectToAction("Stage4_ShippingInfo");
        }

        [HttpPost]
        public IActionResult Stage3_SelectSportPlayer(OrderBuilderViewModel formViewModel)
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            viewModel.SportPlayerId = formViewModel.SportPlayerId;
            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);
            return RedirectToAction("Stage4_ShippingInfo");
        }

        // ===================================================================
        // GIAI ĐOẠN 4: THÔNG TIN GIAO HÀNG
        // ===================================================================
        [HttpGet]
        public IActionResult Stage4_ShippingInfo()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            // Kiểm tra xem GĐ3 đã xong chưa (ít nhất phải chọn Mặt cho Lego 1 HOẶC chọn Cầu thủ)
            bool isLegoReady = (viewModel?.Lego1?.FaceId != null);
            bool isSportReady = (viewModel?.SportPlayerId != null);

            if (viewModel == null || (!isLegoReady && !isSportReady))
            {
                return RedirectToAction("Stage3_Customize");
            }
            return View(viewModel);
        }

        [HttpPost]
        [ActionName("Stage4_ShippingInfo")]
        public IActionResult Stage4_ShippingInfo_POST()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

            string socialMediaName = Request.Form["SocialMediaName"];
            string customerName = Request.Form["CustomerName"];
            string phoneNumber = Request.Form["PhoneNumber"];
            string address = Request.Form["Address"];

            bool isValid = true;
            if (string.IsNullOrEmpty(customerName)) { ModelState.AddModelError("CustomerName", "Vui lòng nhập tên"); isValid = false; }
            if (string.IsNullOrEmpty(phoneNumber)) { ModelState.AddModelError("PhoneNumber", "Vui lòng nhập SĐT"); isValid = false; }
            if (string.IsNullOrEmpty(address)) { ModelState.AddModelError("Address", "Vui lòng nhập địa chỉ"); isValid = false; }

            if (!isValid)
            {
                viewModel.CustomerName = customerName;
                viewModel.PhoneNumber = phoneNumber;
                viewModel.Address = address;
                return View("Stage4_ShippingInfo", viewModel);
            }

            viewModel.SocialMediaName = socialMediaName;
            viewModel.CustomerName = customerName;
            viewModel.PhoneNumber = phoneNumber;
            viewModel.Address = address;

            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);
            return RedirectToAction("Stage5_Confirm");
        }

        // ===================================================================
        // GIAI ĐOẠN 5: XÁC NHẬN & HOÀN TẤT
        // ===================================================================
        [HttpGet]
        public async Task<IActionResult> Stage5_Confirm()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null || string.IsNullOrEmpty(viewModel.CustomerName)) return RedirectToAction("Stage4_ShippingInfo");

            // Tải thông tin chi tiết
            viewModel.SelectedProductType = await _context.ProductTypes.FindAsync(viewModel.ProductTypeId);
            viewModel.SelectedBackground = await _context.CustomOptions.FindAsync(viewModel.BackgroundId);

            decimal totalPrice = 0;
            totalPrice += viewModel.SelectedProductType?.BasePrice ?? 0;
            totalPrice += viewModel.SelectedBackground?.AdditionalPrice ?? 0;

            if (viewModel.ProductTypeId == 1 || viewModel.ProductTypeId == 2)
            {
                // Lego 1
                var hair1 = await _context.CustomOptions.FindAsync(viewModel.Lego1.HairId);
                var face1 = await _context.CustomOptions.FindAsync(viewModel.Lego1.FaceId);
                var clothes1 = await _context.CustomOptions.FindAsync(viewModel.Lego1.ClothesId);
                var pants1 = await _context.CustomOptions.FindAsync(viewModel.Lego1.PantsId);
                var acc1_1 = await _context.CustomOptions.FindAsync(viewModel.Lego1.Accessory1Id);
                var acc1_2 = await _context.CustomOptions.FindAsync(viewModel.Lego1.Accessory2Id);

                decimal lego1Price = (hair1?.AdditionalPrice ?? 0) + (face1?.AdditionalPrice ?? 0) + (clothes1?.AdditionalPrice ?? 0) + (pants1?.AdditionalPrice ?? 0) + (acc1_1?.AdditionalPrice ?? 0) + (acc1_2?.AdditionalPrice ?? 0);
                ViewBag.Lego1_Price = lego1Price;
                ViewBag.Lego1_Names = new List<string?> { hair1?.Name, face1?.Name, clothes1?.Name, pants1?.Name, acc1_1?.Name, acc1_2?.Name };

                if (viewModel.ProductTypeId == 2)
                {
                    // Lego 2
                    var hair2 = await _context.CustomOptions.FindAsync(viewModel.Lego2.HairId);
                    var face2 = await _context.CustomOptions.FindAsync(viewModel.Lego2.FaceId);
                    var clothes2 = await _context.CustomOptions.FindAsync(viewModel.Lego2.ClothesId);
                    var pants2 = await _context.CustomOptions.FindAsync(viewModel.Lego2.PantsId);
                    var acc2_1 = await _context.CustomOptions.FindAsync(viewModel.Lego2.Accessory1Id);
                    var acc2_2 = await _context.CustomOptions.FindAsync(viewModel.Lego2.Accessory2Id);

                    decimal lego2Price = (hair2?.AdditionalPrice ?? 0) + (face2?.AdditionalPrice ?? 0) + (clothes2?.AdditionalPrice ?? 0) + (pants2?.AdditionalPrice ?? 0) + (acc2_1?.AdditionalPrice ?? 0) + (acc2_2?.AdditionalPrice ?? 0);
                    ViewBag.Lego2_Price = lego2Price;
                    ViewBag.Lego2_Names = new List<string?> { hair2?.Name, face2?.Name, clothes2?.Name, pants2?.Name, acc2_1?.Name, acc2_2?.Name };
                    totalPrice += lego1Price + lego2Price;
                }
                else
                {
                    totalPrice += lego1Price;
                }
            }
            else if (viewModel.ProductTypeId == 3)
            {
                viewModel.SelectedSportPlayer = await _context.CustomOptions.FindAsync(viewModel.SportPlayerId);
                totalPrice += viewModel.SelectedSportPlayer?.AdditionalPrice ?? 0;
            }

            viewModel.TotalPrice = totalPrice;
            HttpContext.Session.SetObjectAsJson(OrderSessionKey, viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Stage5_CompleteOrder()
        {
            var viewModel = HttpContext.Session.GetObjectFromJson<OrderBuilderViewModel>(OrderSessionKey);
            if (viewModel == null) return RedirectToAction("StartNewOrder");

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

            // Helper function để trừ tồn kho
            async Task SubtractStock(int? customOptionId)
            {
                if (customOptionId != null)
                {
                    var item = await _context.CustomOptions.FindAsync(customOptionId.Value);
                    if (item != null && item.Quantity != null) item.Quantity -= 1;
                }
            }

            if (viewModel.BackgroundId != null)
            {
                order.OrderItems.Add(new OrderItem { CustomOptionId = viewModel.BackgroundId.Value });
                await SubtractStock(viewModel.BackgroundId);
            }
            if (viewModel.SportPlayerId != null)
            {
                order.OrderItems.Add(new OrderItem { CustomOptionId = viewModel.SportPlayerId.Value });
                await SubtractStock(viewModel.SportPlayerId);
            }

            if (viewModel.ProductTypeId == 1 || viewModel.ProductTypeId == 2)
            {
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

                await SubtractStock(lego1.HairId); await SubtractStock(lego1.FaceId); await SubtractStock(lego1.ClothesId);
                await SubtractStock(lego1.PantsId); await SubtractStock(lego1.Accessory1Id); await SubtractStock(lego1.Accessory2Id);
            }
            if (viewModel.ProductTypeId == 2)
            {
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

                await SubtractStock(lego2.HairId); await SubtractStock(lego2.FaceId); await SubtractStock(lego2.ClothesId);
                await SubtractStock(lego2.PantsId); await SubtractStock(lego2.Accessory1Id); await SubtractStock(lego2.Accessory2Id);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove(OrderSessionKey);

            return RedirectToAction("ThankYou", new { id = order.Id });
        }

        [HttpGet]
        public IActionResult ThankYou(int? id)
        {
            ViewBag.OrderId = id ?? 0;
            return View();
        }
    }
}