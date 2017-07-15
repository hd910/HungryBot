using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HungryBot.Model
{
    [Serializable]
    public class FoodCardModel
    {
        public string name { get; }
        private string[] URLList;
        private int currentURLIndex;
        

        public FoodCardModel(string name, List<string> urls)
        {
            this.name = name;
            currentURLIndex = 0;
            Random rnd = new Random();

            //Randomise the order of the array
            URLList = urls.OrderBy(x => rnd.Next()).ToArray();   
        }

        public void IncrementIndex()
        {
            //Increment index. If end of array, reset to 0
            currentURLIndex = (currentURLIndex +1 >= URLList.Length) ? 0 : currentURLIndex+1;
        }

        public string getCurrentURL()
        {
            return URLList[currentURLIndex];
        }
    }
}