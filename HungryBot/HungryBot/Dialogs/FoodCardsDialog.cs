﻿using Microsoft.Bot.Builder.Dialogs;
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

            await context.PostAsync("Hi there!");
            await context.PostAsync("I'm called the Hungry Bot and I'm here to help you find out what you feel like eating today.");

            PromptDialog.Choice<string>(
                context,
                UserChoice,
                new string[] { StartOption },
                "When you're ready press the button below!",
                "Ooops! I didnt get that... but press the button below to get started!",
                3,
                PromptStyle.Auto);
        }

        private async Task UserChoice(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result;
            //await context.PostAsync($@"Hi {activity}!");
            List<FoodModel> foodList = FoodModel.GetFoodList();

            var userText = activity.ToString();

            if (userText.Contains(MoreOption))
            {
                //More of the same food
                if (currentFood == null)
                {
                    //No current food - generate random
                    currentFood = getRandomFood(foodList);
                }
                else
                {
                    currentFood.IncrementIndex();
                }
            } else if (userText.Contains(NextOption))
            {
                //Next food type
                currentFood = getRandomFood(foodList);
            }
            else if (userText.Contains(FindOption))
            {
                //Find restaurant 
                if(currentFood != null)
                {
                    Uri uri = new Uri(String.Format(yelpUrl, currentFood.name));
                    await context.PostAsync("Our friends at Yelp will help you find " + currentFood.name.ToLower() + " here: " + uri.AbsoluteUri);
                    return;
                }
                else
                {
                    PromptDialog.Choice<string>(
                        context,
                        UserChoice,
                        new string[] { StartOption },
                        "Not sure what you mean... but I'm guessing you're hungry?",
                        "Ooops, what you wrote is not a valid option, please try again",
                        3,
                        PromptStyle.Auto);
                }
            }
            else
            {
                //New session- Show random food
                currentFood = getRandomFood(foodList);
            }

            //Image attachment message
            var message = context.MakeMessage();
            var attachment = GetImageAttachment(currentFood.getCurrentURL());
            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            //Next available choice dialog
            PromptDialog.Choice<string>(
                context,
                UserChoice,
                new string[] { MoreOption, NextOption, FindOption },
                "How about "+currentFood.name + "?",
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

        private static Attachment GetImageAttachment(string link)
        {
            return new Attachment
            {
                ContentType = "image/png",
                ContentUrl = link
            };
        }
    }
}