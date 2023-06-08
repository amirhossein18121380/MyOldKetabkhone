using Newtonsoft.Json;

namespace Common.Extension
{
    public static class JsonExtension
    {
        public static string ToJson(this object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static T? ToModel<T>(this string? jsonString) where T : new()
        {
            try
            {
                if (jsonString == null)
                {
                    return new T();
                }

                var obj = JsonConvert.DeserializeObject<T>(jsonString);
                return obj;
            }
            catch
            {
                return new T();
            }
        }
    }
}
