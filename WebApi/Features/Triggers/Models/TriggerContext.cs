using Newtonsoft.Json.Linq;

namespace WebApi.Features.Triggers.Models
{
    public class TriggerContext
    {
        private JObject _json;
        public TriggerContext(string message)
        {
            _json = JObject.Parse(message);
        }

        private JToken GetPropertyValue(string propertyName)
        {
            return _json[propertyName];
        }
        
        public string GetPropertyStringValue(string propertyName)
        {
            return GetPropertyValue(propertyName).Value<string>();
        }

        public decimal GetPropertyNumberValue(string propertyName)
        {
            return GetPropertyValue(propertyName).Value<decimal>();
        }
    }
}