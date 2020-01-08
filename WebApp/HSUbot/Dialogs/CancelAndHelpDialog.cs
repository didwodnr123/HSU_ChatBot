using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using HSUbot.Details;

namespace HSUbot.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog  //인터럽트 관리 
    {
        public CancelAndHelpDialog(string id)
            : base(id)
        {
        }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        //인터럽트 시 취할 행동
        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text?.ToLowerInvariant();

                switch (text)
                {
                    case "help":
                    case "?":
                    case "도움말":
                        CardGenerator cardGenerator = new CardGenerator();
                        await cardGenerator.AttachAdaptiveCard(innerDc.Context, "HSUbot.Cards.HelpCard.json", cancellationToken);
                        return null;
                    case "cancel":
                    case "quit":
                    case "취소":
                        await innerDc.Context.SendActivityAsync($"Cancelling", cancellationToken: cancellationToken);
                        return await innerDc.CancelAllDialogsAsync();
                }
            }

            return null;
        }
    }
}
