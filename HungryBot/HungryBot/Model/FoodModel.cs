using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;

namespace HungryBot.Model
{
    [Serializable]
    public class FoodModel
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "url1")]
        public string URL1 { get; set; }

        [JsonProperty(PropertyName = "url1thumb")]
        public string URL1Thumb { get; set; }

        [JsonProperty(PropertyName = "url2")]
        public string URL2 { get; set; }

        [JsonProperty(PropertyName = "url2thumb")]
        public string URL2Thumb { get; set; }

        [JsonProperty(PropertyName = "url3")]
        public string URL3 { get; set; }

        [JsonProperty(PropertyName = "url3thumb")]
        public string URL3Thumb { get; set; }

        [JsonProperty(PropertyName = "url4")]
        public string URL4 { get; set; }

        [JsonProperty(PropertyName = "url4thumb")]
        public string URL4Thumb { get; set; }

        [JsonProperty(PropertyName = "url5")]
        public string URL5 { get; set; }

        [JsonProperty(PropertyName = "url5thumb")]
        public string URL5Thumb { get; set; }

        [JsonProperty(PropertyName = "url6")]
        public string URL6 { get; set; }

        [JsonProperty(PropertyName = "url6thumb")]
        public string URL6Thumb { get; set; }

        public static List<FoodModel> foodList;

        public static List<FoodModel> GetFoodList()
        {
            if(foodList == null)
            {
                foodList = LoadFoodListAsync();
            }

            return foodList;
        }

        public static List<FoodModel> LoadFoodListAsync()
        {
            //HttpClient _client = new HttpClient();
            //var url = "http://hungrydata.azurewebsites.net/foodURLList.txt";
            //var content = await _client.GetStringAsync(url);

            List<FoodModel> foodList = new List<FoodModel>();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "HungryBot.foodURLList.txt";
            string content;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }


            if (content != null)
            {
                string[] contentArray = content.Split('\n');
                int index = 0;

                //Loop through food types 
                //TODO: Clean up code
                while (index + 1 < contentArray.Length)
                {
                    FoodModel tempFood = new FoodModel();
                    string name = contentArray[index];
                    tempFood.Name = name;
                    index++;
                    index++;

                    string fullURL = contentArray[index];
                    tempFood.URL1 = fullURL;
                    index++;
                    index++;

                    fullURL = contentArray[index];
                    tempFood.URL2 = fullURL;
                    index++;
                    index++;

                    fullURL = contentArray[index];
                    tempFood.URL3 = fullURL;
                    index++;
                    index++;

                    fullURL = contentArray[index];
                    tempFood.URL4 = fullURL;
                    index++;
                    index++;

                    fullURL = contentArray[index];
                    tempFood.URL5 = fullURL;
                    index++;
                    index++;

                    fullURL = contentArray[index];
                    tempFood.URL6 = fullURL;
                    index++;
                    index++;

                    foodList.Add(tempFood);
                }
            }

            return foodList;
        }
    }
}