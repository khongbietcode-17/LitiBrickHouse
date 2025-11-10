namespace LitiBrickHouse.Enums
{
    // Dùng để phân loại Tóc/Mặt ở Giai đoạn 3
    public enum GenderType
    {
        Unisex = 0, // Dùng cho cả nam/nữ, hoặc cho các món không cần phân loại (Áo, Quần...)
        Male = 1,   // Đồ cho Nam
        Female = 2  // Đồ cho Nữ
    }
    public enum OrderStatus
    {
        Moi = 0,         // Đơn mới đặt
        DangLam = 1,     // Bạn đang làm
        HoanThanh = 2, // Đã giao xong
        DaHuy = 3       // Đã hủy
    }
}