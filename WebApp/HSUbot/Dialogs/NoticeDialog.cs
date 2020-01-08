using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Choices;
using HSUbot.Details;

namespace HSUbot.Dialogs
{
    public class NoticeDialog : CancelAndHelpDialog
    {
        NoticeDetail noticeDetail = new NoticeDetail();

        public NoticeDialog()
            : base(nameof(NoticeDialog))
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
            noticeDetail = (NoticeDetail)stepContext.Options;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("공지사항이 궁금하신가요?"),
                    RetryPrompt = MessageFactory.Text("아래 항목 중에서 고르세요"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "한성공지", "학사공지", "장학공지" })
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            noticeDetail.Case = noticeDetail.Case == null ? stepContext.Context.Activity.Text : stepContext.Result.ToString();
            var notice = stepContext.Context.Activity.Text;
            var msg = String.Empty;

            switch (noticeDetail.Case)
            {
                case "한성공지":
                    msg = "https://www.hansung.ac.kr/web/www/cmty_01_01";
                    break;

                case "학사공지":
                    msg = "https://www.hansung.ac.kr/web/www/cmty_01_03";
                    break;

                case "장학공지":
                    msg = "https://www.hansung.ac.kr/web/www/552";
                    break;
            }
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}