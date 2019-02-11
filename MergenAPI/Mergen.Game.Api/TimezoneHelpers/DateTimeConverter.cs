using System;
using System.Globalization;
using Mergen.Game.Api.Security;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NodaTime;

namespace Mergen.Game.Api.TimezoneHelpers
{
    public class DateTimeConverter : JsonConverter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DateTimeConverter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            bool nullable = objectType == typeof(DateTime?);
            if (reader.TokenType == JsonToken.Null)
            {
                if (!nullable)
                    throw new JsonSerializationException($"Cannot convert null value to {objectType}.");

                return null;
            }

            var value = reader.Value as string;
            if (value == null)
                return null;

            if (!DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var dateTimeOffset))
                throw new JsonSerializationException($"Cannot parse value {value} to {objectType}.");

            return dateTimeOffset.UtcDateTime;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Null are already filtered  

            if (_httpContextAccessor.HttpContext?.User is AccountPrincipal accountPrincipal)
            {
                var timezone = DateTimeZoneProviders.Tzdb[accountPrincipal.Timezone];
                var dateTime = Convert.ToDateTime(value);
                var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
                var result = instant.InZone(timezone).ToDateTimeOffset().ToString("O");
                writer.WriteValue(result);
                writer.Flush();
            }
            else
            {
                writer.WriteValue(Convert.ToDateTime(value).ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}