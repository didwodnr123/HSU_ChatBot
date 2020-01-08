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
    public class ProfessorsDialog : CancelAndHelpDialog
    {
        ProfessorsDetail pfDetail = new ProfessorsDetail();

        public ProfessorsDialog() : base(nameof(ProfessorsDialog))
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
            pfDetail = (ProfessorsDetail)stepContext.Options;
            if(pfDetail.Name == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("어느 교수님을 찾으시나요?\n\n(교수님 성함만 작성해주세요)"),
                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(pfDetail.Name, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            pfDetail.Name = pfDetail.Name == null ? stepContext.Context.Activity.Text : stepContext.Result.ToString();
            Professors pf = new Professors();
            var list = pf.ProfessorsDB(pfDetail.Name);

            if (list.Count > 0)
            {
                CardGenerator cardGenerator = new CardGenerator();
                await cardGenerator.AttachProfessorsCard(stepContext.Context, list, pfDetail.Name, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{pfDetail.Name} 으로 검색한 결과가 없습니다."));
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}
