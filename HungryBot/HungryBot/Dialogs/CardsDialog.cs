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
        private const string MoreOption = "Show me more";
        private const string NextOption = "Next food";
        private const string FindOption = "Find Restaurant";

        private IEnumerable<string> options = new List<string> { MoreOption, NextOption, FindOption };


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var userText = await result as Activity;
            switch (userText.Text)
            {
                case MoreOption:
                    await DisplayFoodCard(context, result);
                    break;
                case NextOption:
                    await DisplayFoodCard(context, result);
                    break;
                case FindOption:
                    break;
                default:
                    WelcomeDialog(context);
                    break;
            }
        }

        private void WelcomeDialog(IDialogContext context)
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
            var heroCard = new HeroCard
            {
                Title = "How about Burger?",
                Images = new List<CardImage> { new CardImage("https://farm3.staticflickr.com/2880/33359463604_c5c8bc6b10_z.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, MoreOption, value: MoreOption),
                    new CardAction(ActionTypes.PostBack, NextOption, value: NextOption),
                    new CardAction(ActionTypes.PostBack, FindOption, value: FindOption)}
            };

            return heroCard.ToAttachment();
        }
    }
}