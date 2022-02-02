using System;
using System.Text.Json;

namespace ActionFramework.Helpers
{
    public class JsonHelper
    {
        private string jsonString;
        private JsonDocument document;
        private JsonElement rootElement;

        public JsonHelper(string jsonString)
        {
            if (IsValidJson())
            {
                this.jsonString = jsonString;
                rootElement = document.RootElement;
            }
            else
                throw new Exception($"Invalid Json {jsonString}");
        }

        public JsonElement GetProperty(string propertyName)
        {
            return rootElement.GetProperty(propertyName);
        }

        public bool IsValidJson()
        {
            try
            {
                var document = System.Text.Json.JsonDocument.Parse(jsonString);
                return true;
            }
            catch (System.Exception ex) when
                (ex is System.Text.Json.JsonException || ex is System.ArgumentException)
            {
                //LogFactory.File.Error(ex, $"Invalid Json Received {jsonString}");
                return false;
            }

        }
    }
}
