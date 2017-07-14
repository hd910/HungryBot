using HungryBot.Model;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HungryBot
{
    public class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<FoodModel> foodModelTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://hungrymobileapp.azurewebsites.net");
            this.foodModelTable = this.client.GetTable<FoodModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;

            }
        }

        public async Task<List<FoodModel>> GetFoodList()
        {
            return await this.foodModelTable.ToListAsync();
        }
    }
}
}