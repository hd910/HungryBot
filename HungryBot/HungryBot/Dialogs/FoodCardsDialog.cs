using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using HungryBot.Model;

namespace HungryBot.Dialogs
{
    [Serializable]
    public class FoodCardsDialog : IDialog<object>
    {
        private const string MoreOption = "Show me more";
        private const string NextOption = "Next food";
        private const string FindOption = "Find Restaurant";
        private const string StartOption = "Show me food!";
        private const string yelpUrl = "https://www.yelp.com/search?find_desc={0}&ns=1";
        private FoodCardModel currentFood;

        Task IDialog<object>.StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            PromptDialog.Choice<string>(
            context,
            GetStarted,
            new string[] { StartOption },
            "Hi there! I'm the Hungry Bot. Are you hungry?",
            "Ooops, what you wrote is not a valid option, please try again",
            3,
            PromptStyle.Auto);
            
        }

        private async Task GetStarted(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result;
            List<FoodModel> foodList = await FoodModel.GetFoodList();
            //await context.PostAsync($@"Hi {activity}!");

            if (currentFood == null)
            {
                //No current food - generate random
                currentFood = getRandomFood(foodList);
            }

            showFood(context, currentFood);
        }

        private async Task BotOptions(IDialogContext context, IAwaitable<object> result)
        {
            var userText = await result;
            List<FoodModel> foodList = await FoodModel.GetFoodList();

            if (userText.ToString() == MoreOption)
            {
                //More of the same food
                //await DisplayFoodCard(context, result);
                if (currentFood == null)
                {
                    //No current food - generate random
                    currentFood = getRandomFood(foodList);
                }
                else
                {
                    currentFood.IncrementIndex();
                }
                showFood(context, currentFood);
            }
            else if (userText.ToString() == NextOption)
            {
                //Next food type
                currentFood = getRandomFood(foodList);
                showFood(context, currentFood);
            }
            else if (userText.ToString() == FindOption)
            {
                //showFood(context);
            }
            else
            {

            }
        }

        private void showFood(IDialogContext context, FoodCardModel current)
        {
            PromptDialog.Choice<string>(
                context,
                BotOptions,
                new string[] { MoreOption, NextOption, FindOption },
                current.name,
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        private FoodCardModel getRandomFood(List<FoodModel> list)
        {
            //Randomize order
            Random rnd = new Random();
            int index = rnd.Next(0, list.Count);
            FoodModel selectedFood = list[index];

            var name = selectedFood.Name;
            List<string> urlList = new List<string>();
            urlList.Add(selectedFood.URL1);
            urlList.Add(selectedFood.URL2);
            urlList.Add(selectedFood.URL3);
            urlList.Add(selectedFood.URL4);
            urlList.Add(selectedFood.URL5);
            urlList.Add(selectedFood.URL6);

            FoodCardModel foodCard = new FoodCardModel(name, urlList);

            return foodCard;
        }
    }
}