using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Net.Http;
using HtmlAgilityPack;
using System.Xml;
using System.IO;
using HSUbot.Details;
using Microsoft.Bot.Schema;

namespace HSUbot.Dialogs
{
    public class MatjeepchoochunDialog : CancelAndHelpDialog
    {

        MatjeepchoochunDetail matjeepchoochunDetail = new MatjeepchoochunDetail();

        public MatjeepchoochunDialog()
            : base(nameof(MatjeepchoochunDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FirstStepAsync,
                SecondStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("맛집추천!!"),
                    RetryPrompt = MessageFactory.Text("아래 항목 중에서 고르세요"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "한식", "중식", "일식", "분식", "치킨", "피자", "순대국" })
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            matjeepchoochunDetail.Category = stepContext.Context.Activity.Text;
            var msg = String.Empty;

            var attachments = new List<Attachment>();

            var reply = MessageFactory.Attachment(attachments);

            switch (matjeepchoochunDetail.Category)
            {
                case "한식":
                    reply.Attachments.Add(Cards.GetYoonGaNeCard().ToAttachment());
                    reply.Attachments.Add(Cards.GetRiceBurgerCard().ToAttachment());
                    reply.Attachments.Add(Cards.GetWoonBongCard().ToAttachment());
                    break;
                case "중식":
                    reply.Attachments.Add(Cards.GetSeunglijangCard().ToAttachment());
                    reply.Attachments.Add(Cards.GetJoongHwaMyungGaCard().ToAttachment());
                    break;
                case "일식":
                    reply.Attachments.Add(Cards.GetSushiHarooCard().ToAttachment());
                    reply.Attachments.Add(Cards.GetSushiHyeonCard().ToAttachment());
                    reply.Attachments.Add(Cards.GetMrDonkkasCard().ToAttachment());
                    reply.Attachments.Add(Cards.GetStarDongCard().ToAttachment());
                    break;
                case "분식":
                    reply.Attachments.Add(Cards.GetMecaDDuckCard().ToAttachment());
                    reply.Attachments.Add(Cards.GetSinJeonCard().ToAttachment());
                    break;
                case "치킨":
                    reply.Attachments.Add(Cards.GetHoChickenCard().ToAttachment());
                    break;
                case "피자":
                    reply.Attachments.Add(Cards.GetPizzaBellCard().ToAttachment());
                    break;
                case "순대국":
                    reply.Attachments.Add(Cards.GetGrandMamaCard().ToAttachment());
                    reply.Attachments.Add(Cards.GetAuneCard().ToAttachment());
                    reply.Attachments.Add(Cards.GetDonamgolCard().ToAttachment());
                    break;
            }

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);

        }

    }
}