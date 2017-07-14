﻿using System;
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
        private Random rnd = new Random();

        public FoodCardModel(string name, List<string> urls)
        {
            this.name = name;
            currentURLIndex = 0;

            //Randomise the order of the array
            URLList = urls.OrderBy(x => rnd.Next()).ToArray();   
        }

        public void IncrementIndex()
        {
            //Increment index. If end of array, reset to 0
            currentURLIndex = (currentURLIndex >= URLList.Length) ? 0 : currentURLIndex++;
        }

        public string getCurrentURL()
        {
            return URLList[currentURLIndex];
        }
    }
}