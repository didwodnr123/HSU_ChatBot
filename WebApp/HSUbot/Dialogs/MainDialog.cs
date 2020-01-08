using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs.Choices;
using HSUbot.Details;
using Microsoft.Bot.Schema;
using System.IO;
using Newtonsoft.Json;

namespace HSUbot.Dialogs
{
    public class MainDialog : CancelAndHelpDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;
        private LuisHelper luisHelper;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger) : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;

            luisHelper = new LuisHelper(configuration);

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new BusInformationDialog());
            AddDialog(new TrackInfoDialog());
            AddDialog(new WeatherInformationDialog());
            AddDialog(new PhoneAdressDialog());
            AddDialog(new ProfessorsDialog());
            AddDialog(new WorkDialog());
            AddDialog(new MatjeepchoochunDialog());
            AddDialog(new NoticeDialog());
            AddDialog(new TrackInfoCardDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var text = stepContext.Context.Activity.Text;
            if (text != null && !(text == "도움말" || text == "help"))
            {
                return await stepContext.NextAsync(stepContext.Context.Activity.Text, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("필요한 서비스를 말씀해주십시오."),
                }, cancellationToken);
            }
            
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string result = stepContext.Context.Activity.Text;
            MainDetail detail = new MainDetail();
            //Luis 연동 체크
            if (string.IsNullOrEmpty(Configuration["LuisAppId"]) || string.IsNullOrEmpty(Configuration["LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostName"]))
            {
                detail = null;
            }
            else
            {
                detail = await luisHelper.ExecuteLuisQuery(Logger, stepContext.Context, cancellationToken);
            }
            //대화 의도에 따른 각각의 대화흐름 시작
            switch (detail.intent)
            {
                case "버스":
                    return await stepContext.BeginDialogAsync(nameof(BusInformationDialog), (BusDetails)detail, cancellationToken);
                case "미세먼지":
                    return await stepContext.BeginDialogAsync(nameof(WeatherInformationDialog), (DustDetails)detail, cancellationToken);
                case "직업":
                    return await stepContext.BeginDialogAsync(nameof(WorkDialog), (WorkDetails)detail, cancellationToken);
                case "학식메뉴":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("https://www.hansung.ac.kr/portlet-repositories/fckeditor/images/7qHyuRAiKcc=/1557707052583.png"));
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                case "트랙정보":
                    return await stepContext.BeginDialogAsync(nameof(TrackInfoDialog), (TrackInfoDetails)detail, cancellationToken);
                case "전화번호":
                    return await stepContext.BeginDialogAsync(nameof(PhoneAdressDialog), (PhoneDetails)detail, cancellationToken);
                case "트랙정보카드":
                    return await stepContext.BeginDialogAsync(nameof(TrackInfoCardDialog), (TrackInfoCardDetail)detail, cancellationToken);
                case "교수님":
                    return await stepContext.BeginDialogAsync(nameof(ProfessorsDialog), (ProfessorsDetail)detail, cancellationToken);
                case "맛집":
                    return await stepContext.BeginDialogAsync(nameof(MatjeepchoochunDialog), (MatjeepchoochunDetail)detail, cancellationToken);
                case "공지사항":
                    return await stepContext.BeginDialogAsync(nameof(NoticeDialog), (NoticeDetail)detail, cancellationToken);
                case "장학제도":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("https://www.hansung.ac.kr/web/www/life_02_01_t1"));
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                default:
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("잘 이해하지 못했어요. 다시 질문해주세요"));
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                   
                    
            }
        }
    }
}

