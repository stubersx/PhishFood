using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using PhishFood.Models;

namespace PhishFood.Helpers
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value)
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T? Get<T>(this ITempDataDictionary tempData, string key)
        {
            object? o;
            tempData.TryGetValue(key, out o);
            return o == null ? default : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}