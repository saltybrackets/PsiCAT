namespace PsiCat
{
    using System.IO;
    using Newtonsoft.Json;


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

            T deserializedObject = JsonConvert.DeserializeObject<T>(json);
            return deserializedObject;
        }
        
        
        public void Save(string filePath)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}