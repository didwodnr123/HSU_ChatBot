
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;

namespace HSUbot.Bots
{
    /// <summary>
    /// 봇의 주 진입점
    /// </summary>
    /// <typeparam name="T"> 시작 다이얼로그</typeparam>
    public class HSUBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly Dialog Dialog;
        protected readonly ConversationState ConversationState;
        protected readonly UserState UserState;
        protected readonly ILogger Logger;

        private const string ADAPTIVEINTROCARDPATH = "HSUbot.Cards.ServiceCard.json";
        public HSUBot(ConversationState conversationState, UserState userState,T dialog, ILogger<HSUBot<T>> logger)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            Logger = logger;

        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    CardGenerator cardGenerator = new CardGenerator();
                    await cardGenerator.AttachAdaptiveCard(turnContext, ADAPTIVEINTROCARDPATH , cancellationToken);
                }
            }
        }

    }
}
