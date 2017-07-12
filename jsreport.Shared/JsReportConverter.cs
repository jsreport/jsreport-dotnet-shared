using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace jsreport.Shared
{
    public class JsReportConverter<T> : JsonConverter
    {
        private string _fallback;
        private StringEnumConverter _enumConverter;

        public JsReportConverter(string fallBack)
        {
            _fallback = fallBack;
            _enumConverter = new StringEnumConverter();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty(_fallback))
            {
                _enumConverter.WriteJson(writer, value, serializer);
            }
            else
            {
                writer.WriteValue(_fallback);
            }
        }
    }
}
