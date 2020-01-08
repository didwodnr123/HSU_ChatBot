using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs;
using HSUbot.Details;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace HSUbot.Dialogs
{
    public class PhoneAdressDialog : CancelAndHelpDialog
    {
        PhoneDetails phoneDetails = new PhoneDetails();

        public PhoneAdressDialog() : base(nameof(PhoneAdressDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NameStepAsync,
                ActStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            phoneDetails = (PhoneDetails)stepContext.Options;
            if (phoneDetails.Name == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("어느 전화부서를 찾으시나요?"),
                }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(phoneDetails.Name, cancellationToken);
            }

        }       
        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            phoneDetails.Name = phoneDetails.Name == null ? stepContext.Context.Activity.Text : stepContext.Result.ToString();
            //phoneDetails.Name = stepContext.Context.Activity.Text;
            PhoneAdress phone = new PhoneAdress();
            var list = phone.PhoneDB(phoneDetails.Name);
            if (list.Count > 0)
            {
                CardGenerator cardGenerator = new CardGenerator();
                await cardGenerator.AttachPhoneCard(stepContext.Context, list, phoneDetails.Name, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{phoneDetails.Name} 으로 검색한 결과가 없습니다."));
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
