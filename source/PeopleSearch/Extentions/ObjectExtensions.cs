using Newtonsoft.Json;

namespace PeopleSearch.Extentions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Ensure consistent serializer settings.
        /// </summary>
        public static string ToJson(this object @this, bool ignoreNullValues = false)
        {
            @this.GuardNull(nameof(@this));

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = ignoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include
            };

            return JsonConvert.SerializeObject(@this, settings);
        }
    }
}
