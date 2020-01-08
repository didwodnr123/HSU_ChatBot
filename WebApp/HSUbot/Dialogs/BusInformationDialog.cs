using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Choices;
using HSUbot.Details;
namespace HSUbot.Dialogs
{
    /// <summary>
    /// 버스정보 대화 흐름
    /// </summary>
    public class BusInformationDialog : CancelAndHelpDialog
    {
        BusDetails busDetail = new BusDetails();
        private const string suttleBus =
            "학교 <> 삼선교\n" +
            "오전 : 08:25 ~ 08:40 (1대운행) (학교 <> 기숙사)\n" +
            "오전 : 08:30 ~ 10:30 (3대 수시운행)\n" +
            "점심 : 12:00 ~ 13:00 (1대 수시운행)\n" +
            "오후 : 13:00 ~ 13:15 (1대 운행) (학교<> 기숙사)\n" +
            "저녁 : 17:00 ~ 19:00 (2대 수시운행)";
        public BusInformationDialog() : base(nameof(BusInformationDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                BusTypeStepAsync,
                BusStationStepAsync,
                BusInfoStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
            //시작 다이얼로그 지정
        }

        private async Task<DialogTurnResult> BusTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            busDetail = (BusDetails)stepContext.Options;
            if(busDetail.RouteNum == null) {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions {
                    Prompt = MessageFactory.Text("어떤 버스를 이용하시나요?"),
                    RetryPrompt = MessageFactory.Text("아래 버스 중에서 골라주십시오"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "셔틀버스", "성북02", "종로03" })
                }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(busDetail.RouteNum, cancellationToken);
            }
            
        }
        private async Task<DialogTurnResult> BusStationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            busDetail.RouteNum = busDetail.RouteNum == null ? stepContext.Context.Activity.Text :stepContext.Result.ToString();
            
            if(busDetail.StationName == null)
            {
                List<string> stationsList = new List<string>();
                if (busDetail.RouteNum == "성북02")
                {
                    stationsList.Add("한성대정문");
                    stationsList.Add("삼선교역2번출구");
                }
                else if (busDetail.RouteNum == "종로03")
                {
                    stationsList.Add("창신쌍용2단지.한성대후문");
                    stationsList.Add("창신역.4번출구");
                }
                else if (busDetail.RouteNum == "셔틀버스")
                {
                    string msg = "셔틀버스 시간표 안내\n" + suttleBus;
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }


                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("어디서 출발하시나요?"),
                        RetryPrompt = MessageFactory.Text("아래 정류장에서 골라주십시오"),
                        Choices = ChoiceFactory.ToChoices(stationsList)
                    }, cancellationToken);
            }
            else
            {
                if(busDetail.StationName == "지하철역")
                {
                    busDetail.StationName = (busDetail.RouteNum == "성북02") ? "삼선교역2번출구" : "창신역.4번출구";
                }
                else if(busDetail.StationName == "학교")
                {
                    busDetail.StationName = (busDetail.RouteNum == "성북02") ? "한성대정문" :"창신쌍용2단지.한성대후문";
                }
                return await stepContext.NextAsync(busDetail.StationName, cancellationToken);
            }
            
        }

        private async Task<DialogTurnResult> BusInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            busDetail.StationName = busDetail.StationName == null?stepContext.Context.Activity.Text : stepContext.Result.ToString();

            string msg = await BusInformation.GetBusArrTimeAsync(busDetail.RouteNum, busDetail.StationName);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);

        }
    }
}
