using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

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
            //await context.PostAsync($@"Hi {activity}!");

            showFood(context);
        }

        private async Task FoodEntered(IDialogContext context, IAwaitable<object> result)
        {
            var userText = await result;

            showFood(context);

            //if (userText == MoreOption)
            //{
            //    context.Wait(GetStarted);
            //}
            //else if (userText == NextOption)
            //{
            //    context.Wait(GetStarted);
            //}
            //else if (userText == FindOption)
            //{
            //    context.Wait(GetStarted);
            //}
            //else
            //{

            //}
            //return;
        }

        private void showFood(IDialogContext context)
        {
            PromptDialog.Choice<string>(
                context,
                FoodEntered,
                new string[] { MoreOption, NextOption, FindOption },
                "[Show food here]",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }
    }
}