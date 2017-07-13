using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace HungryBot.Dialogs
{
    [Serializable]
    public class CardsDialog : IDialog<object>
    {
        private IEnumerable<string> options = new List<string> { "More", "Next", "Find Food"};


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            //// calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            //// return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            //context.Wait(MessageReceivedAsync);
            PromptDialog.Choice<string>(
                context,
                this.DisplaySelectedCard,
                options,
                "Hi there, choose one of the options below",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        public async Task DisplaySelectedCard(IDialogContext context, IAwaitable<string> result)
        {
            var selectedCard = await result;

            var message = context.MakeMessage();

            var attachment = GetHeroCard();
            message.Attachments.Add(attachment);

            await context.PostAsync(message);

            context.Wait(this.MessageReceivedAsync);
        }

        private static Attachment GetHeroCard()
        {
            var heroCard = new HeroCard
            {
                Title = "How about Burger?",
                Images = new List<CardImage> { new CardImage("https://farm3.staticflickr.com/2880/33359463604_c5c8bc6b10_z.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "More", value: "More") }
            };

            return heroCard.ToAttachment();
        }
    }
}