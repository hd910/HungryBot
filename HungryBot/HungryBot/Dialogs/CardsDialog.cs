﻿using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using HungryBot.Model;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq;

namespace HungryBot.Dialogs
{
    [Serializable]
    public class CardsDialog : IDialog<object>
    {
        private const string MoreOption = "Show me more";
        private const string NextOption = "Next food";
        private const string FindOption = "Find Restaurant";
        private const string StartOption = "Show me food!";

        private IEnumerable<string> options = new List<string> { MoreOption, NextOption, FindOption };
        private FoodCardModel currentFood;


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            List<FoodModel> foodList = await FoodModel.LoadFoodListAsync();

            //Get state
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            var sentGreeting = userData.GetProperty<bool>("SentGreeting");

            if (activity.Text.Contains(MoreOption))
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
            }
            else if (activity.Text == NextOption)
            {
                //Next food type
                currentFood = getRandomFood(foodList);
            }
            else if (activity.Text.Contains(FindOption))
            {
                //Find restaurant closest
                await DisplayFoodCard(context, result);
                return;
            }
            else
            {
                if (!sentGreeting)
                {
                    WelcomePrompt(context);

                    //Save state
                    var data = context.UserData;
                    data.SetValue("SentGreeting", true);
                }
                else
                {
                    //Unknown - unrecognized message
                    UnrecognisedPrompt(context);
                }
                return;
            }

            ShowFoodCard(currentFood, context);

        }


        private async void ShowFoodCard(FoodCardModel currentFood, IDialogContext context)
        {
            var message = context.MakeMessage();

            var attachment = BuildHeroCard(currentFood);
            message.Attachments.Add(attachment);

            await context.PostAsync(message);

            context.Wait(this.MessageReceivedAsync);
        }

        private FoodCardModel getRandomFood(List<FoodModel> list)
        {
            //Randomize order
            Random rnd = new Random();
            int index = rnd.Next(0, list.Count);
            FoodModel selectedFood = list[index];

            var name = selectedFood.Name;
            List<string> urlList =  new List<string>();
            urlList.Add(selectedFood.URL1);
            urlList.Add(selectedFood.URL2);
            urlList.Add(selectedFood.URL3);
            urlList.Add(selectedFood.URL4);
            urlList.Add(selectedFood.URL5);
            urlList.Add(selectedFood.URL6);

            FoodCardModel foodCard = new FoodCardModel(name, urlList);

            return foodCard;
        }

        private void UnrecognisedPrompt(IDialogContext context)
        {
            PromptDialog.Choice<string>(
                context,
                this.DisplayFoodCard,
                new string[] { "Show me food!" },
                "Sorry, I didn't understand that :( \n Press the button below to see food!",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        private void WelcomePrompt(IDialogContext context)
        {
            //Introduction dialog
            PromptDialog.Choice<string>(
                context,
                this.DisplayFoodCard,
                new string[] {"Show me food!"},
                "Hi there! I'm the Hungry Bot. Are you hungry?",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        public async Task DisplayFoodCard(IDialogContext context, IAwaitable<Object> result)
        {
            var userText = await result;

            var message = context.MakeMessage();

            var attachment = GetHeroCard();
            message.Attachments.Add(attachment);

            await context.PostAsync(message);

            context.Wait(this.MessageReceivedAsync);
        }

        private static Attachment GetHeroCard()
        {
            var foodName = "Burger";
            var heroCard = new HeroCard
            {
                Title = String.Format("How about {0}?", foodName),
                Images = new List<CardImage> { new CardImage("https://farm3.staticflickr.com/2880/33359463604_c5c8bc6b10_z.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, MoreOption, value: MoreOption),
                    new CardAction(ActionTypes.ImBack, NextOption, value: NextOption),
                    new CardAction(ActionTypes.ImBack, FindOption, value: "Show me more " + foodName)}
            };

            return heroCard.ToAttachment();
        }

        private static Attachment BuildHeroCard(FoodCardModel currentFood)
        {
            var foodName = currentFood.name;
            var foodURL = currentFood.getCurrentURL();

            var heroCard = new HeroCard
            {
                Title = String.Format("How about {0}?", foodName),
                Images = new List<CardImage> { new CardImage(foodURL) },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, MoreOption, value: MoreOption),
                    new CardAction(ActionTypes.ImBack, NextOption, value: NextOption),
                    new CardAction(ActionTypes.ImBack, FindOption, value: "Show me more " + foodName)}
            };

            return heroCard.ToAttachment();
        }


    }
}