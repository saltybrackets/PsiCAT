namespace PsiCat
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Newtonsoft.Json;


    [Serializable]
    public class Config
    {
        public static T LoadFromJson<T>(string filePath)
            where T : Config
        {
            string json;
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                json = streamReader.ReadToEnd();
            }

            // Use default value for missing properties.
            JsonSerializerSettings configReadSettings = new JsonSerializerSettings();
            configReadSettings.DefaultValueHandling = DefaultValueHandling.Populate;
            configReadSettings.NullValueHandling = NullValueHandling.Ignore;
            
            T deserializedObject = JsonConvert.DeserializeObject<T>(json, configReadSettings);
            return deserializedObject;
        }
        
        
        public virtual void Save(string filePath = null)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }


        public static T Clone<T>(T config)
            where T: Config
        {
            string copy = JsonConvert.SerializeObject(config);
            return JsonConvert.DeserializeObject<T>(copy);
        }
    }
}