
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace LitiBrickHouse.Extensions
{
    // Lớp này giúp lưu và lấy đối tượng (object) từ Session
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }
}
