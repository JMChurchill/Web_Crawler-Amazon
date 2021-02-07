using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WebScrapper.Model
{
    class JSONWriter
    {
        //convert object to json
        public void addToJson(List<ProductDetails> data, string filePath)
        {
            ////deserialise
            JsonSerializer jsonSerializer = new JsonSerializer();
            if (File.Exists(filePath))
            {
                string obj = File.ReadAllText(filePath);
                var list = JsonConvert.DeserializeObject<List<ProductDetails>>(obj);
                if (list != null)
                {
                    foreach (ProductDetails prod in list)
                    {
                        data.Add(prod);
                    }
                }
                //sericalise
                StreamWriter sw = new StreamWriter(filePath);
                JsonWriter jsonWriter = new JsonTextWriter(sw);

                jsonSerializer.Serialize(jsonWriter, data);

                jsonWriter.Close();
                sw.Close();
            }
        } 
    }
}
