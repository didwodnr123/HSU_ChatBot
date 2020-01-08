using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using HSUbot.Details;

namespace HSUbot
{
    public class CardGenerator
    {
        private Attachment CreateAdaptiveCardAttachment(string cardPath)
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(cardPath))
            {
                using (var reader = new StreamReader(stream))
                {
                    var adaptiveCard = reader.ReadToEnd();
                    return new Attachment()
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JsonConvert.DeserializeObject(adaptiveCard),
                    };
                }
            }
        }
        public async Task AttachAdaptiveCard(ITurnContext turnContext, string cardPath, CancellationToken cancellationToken)
        {
            var welcomeCard = CreateAdaptiveCardAttachment(cardPath);
            var response = MessageFactory.Attachment(welcomeCard);
            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        internal Task AttachAdaptiveCard(object turnContext, string v, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task AttachPhoneCard(ITurnContext turnContext, List<PhoneNum> list, string searchName, CancellationToken cancellationToken)
        {
            var reply = new List<IMessageActivity>();
            reply.Add(MessageFactory.Text($"{searchName}으로 검색한 결과는 다음과 같습니다.\n\n전화를 거시려면 버튼을 눌러주세요"));

            var cardActions = new List<CardAction>();
            foreach(PhoneNum phoneNum in list)
            {
                cardActions.Add(new CardAction()
                {
                    Title = phoneNum.Department + " - " + phoneNum.Name,
                    Type = ActionTypes.Call,
                    Value = phoneNum.PhoneNumber
                });
            }
            var attachments = new List<Attachment>();
            attachments.Add(new HeroCard
            {
                Buttons = cardActions
            }.ToAttachment()); ;
            reply.Add(MessageFactory.Attachment(attachments));


            await turnContext.SendActivitiesAsync(reply.ToArray(), cancellationToken);
        }

        public async Task AttachProfessorsCard(ITurnContext turnContext, List<ProfessorsNum> list, string searchName, CancellationToken cancellationToken)
        {
            var reply = new List<IMessageActivity>();
            foreach (ProfessorsNum professorsNum in list)
            {
                reply.Add(MessageFactory.Text
                    ($"{searchName}교수님으로 검색한 결과는 다음과 같습니다\n\n" +
                        $"대학 : {professorsNum.College}\n\n" +
                        $"학부 : {professorsNum.Department}\n\n" +
                        $"트랙 : {professorsNum.Track}\n\n" +
                        $"연구실 : {professorsNum.Lab}\n\n" +
                        $"이메일 : {professorsNum.Email}"));
            }
            
            var cardActions = new List<CardAction>();
            foreach (ProfessorsNum professorsNum in list)
            {
                cardActions.Add(new CardAction()
                {
                    Title = professorsNum.Name + "교수님께 전화걸기", 
                    Type = ActionTypes.Call,
                    Value = professorsNum.Adress
                });
            }
            var attachments = new List<Attachment>();
            attachments.Add(new HeroCard
            {
                Buttons = cardActions
            }.ToAttachment());
            reply.Add(MessageFactory.Attachment(attachments));


            await turnContext.SendActivitiesAsync(reply.ToArray(), cancellationToken);
        }
    }
}
