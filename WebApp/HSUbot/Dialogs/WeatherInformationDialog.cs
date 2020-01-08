using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using HSUbot.Details;

namespace HSUbot.Dialogs
{
    public class WeatherInformationDialog : CancelAndHelpDialog


    {
        DustDetails dustDetail = new DustDetails();



        private const string weatherSite =
           "  \n" +
           "서울 특별시 초미세먼지 = http://cleanair.seoul.go.kr/air_city.htm?method=measure&grp1=pm25 \n" + "\n"+
           "서울 특별시 미세먼지 = http://cleanair.seoul.go.kr/air_city.htm?method=measure&grp1=pm10 \n" + "\n" +
           " 기상청 날씨누리 = http://www.weather.go.kr/weather/main.jsp \n"
          ;


        public WeatherInformationDialog() : base(nameof(WeatherInformationDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
              //  areaStepAsync,
               serviceStepAsync,
            //    ConfirmStepAsync,
            //   FinalStepAsync,
              // ActStepAsync
              DustStationStepAsync,
              DustInfoStepAsync

            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /*   private async Task<DialogTurnResult> areaStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
           {
               var dustDetails = (DustDetails)stepContext.Options;

               if (dustDetails.Area == null)
               {
                   return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("어떤 지역을 찾으시나요?") }, cancellationToken);
               }
               else
               {
                   return await stepContext.NextAsync(dustDetails.Area, cancellationToken);
               }
           }*/

        private async Task<DialogTurnResult> serviceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            dustDetail = (DustDetails)stepContext.Options;

            if (dustDetail.Dustservice == null)
            {


                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("지역 첫 자음을 선택해 주세요"),
                        RetryPrompt = MessageFactory.Text("지역 첫 자음을 아래에서 선택해 주세요"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "ㄱ~ㄴ", "ㄷ~ㅅ", "ㅇ~ㅈ", "ㅊ~ㅂ", "대기환경 정보" })

                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(dustDetail.Dustservice, cancellationToken);
            }

        }

        private async Task<DialogTurnResult> DustStationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            dustDetail.Dustservice = dustDetail.Dustservice == null ? stepContext.Context.Activity.Text : stepContext.Result.ToString();

          //  dustDetail.Dosistationname = stepContext.Context.Activity.Text;
            

            if (dustDetail.Dosistationname == null)
            {
                List<string> stationsList = new List<string>();

                if (dustDetail.Dustservice == "ㄱ~ㄴ")
                {
                    stationsList.Add("광주");
                    stationsList.Add("경남");
                    stationsList.Add("경북");


                }
                else if (dustDetail.Dustservice == "ㄷ~ㅅ")
                {
                    stationsList.Add("대구");
                    stationsList.Add("대전");
                    stationsList.Add("서울");
                    stationsList.Add("세종");

                }

                else if (dustDetail.Dustservice == "ㅇ~ㅈ")
                {
                    stationsList.Add("울산");
                    stationsList.Add("인천");
                    stationsList.Add("제주");
                    stationsList.Add("전남");
                    stationsList.Add("전북");

                }
                else if (dustDetail.Dustservice == "ㅊ~ㅂ")
                {

                    stationsList.Add("충남");
                    stationsList.Add("충북");
                    stationsList.Add("부산");
                }

                else if (dustDetail.Dustservice == "대기환경 정보")
                {
                    string msg = "대기환경 정보\n" + weatherSite;
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
                    return await stepContext.EndDialogAsync(null, cancellationToken);

                }


                return await stepContext.PromptAsync(nameof(ChoicePrompt),

                new PromptOptions
                {
                    Prompt = MessageFactory.Text("원하시는 지역을 선택해 주세요"),
                    RetryPrompt = MessageFactory.Text("아래 지역에서 선택해 주세요"),
                    Choices = ChoiceFactory.ToChoices(stationsList)
                }, cancellationToken);
            }
            else
            {
                if (dustDetail.Dosistationname == "특별시")
                {
                    dustDetail.Dosistationname = (dustDetail.Dustservice == "ㄷ~ㅅ") ? "서울" : "광주";
                }
                else if (dustDetail.Dosistationname == "시")
                {
                    dustDetail.Dosistationname = (dustDetail.Dustservice == "ㅊ~ㅂ") ? "부산" : "인천";
                }
                return await stepContext.NextAsync(dustDetail.Dosistationname, cancellationToken);

            }
        }

        private async Task<DialogTurnResult> DustInfoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //dustDetail.Dosistationname = stepContext.Context.Activity.Text;

            dustDetail.Dosistationname = dustDetail.Dosistationname == null ? stepContext.Context.Activity.Text : stepContext.Result.ToString();

            string msg = await DustInformation.GetDustInformationAsync(dustDetail.Dustservice, dustDetail.Dosistationname);

           

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
          

            return await stepContext.EndDialogAsync(null, cancellationToken);

        }








        /*   private async Task<DialogTurnResult> serviceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
           {
               var dustDetails = (DustDetails)stepContext.Options;

               dustDetails.Area = (string)stepContext.Result;

               if (dustDetails.Service == null)
               {
                   return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("1.미세먼지 2. 날씨 \n 원하시는 서비스는 무엇인가요?") }, cancellationToken);
               }
               else
               {
                   return await stepContext.NextAsync(dustDetails.Service, cancellationToken);
               }
           }

           private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
           {
               var dustDetails = (DustDetails)stepContext.Options;

               dustDetails.Service = (string)stepContext.Result;

               var msg = $"원하시는 서비스는 {dustDetails.Area}의  {dustDetails.Service} 가 맞습니까?";

               return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
           }

           private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
           {
               if ((bool)stepContext.Result)
               {
                   var dustDetails = (DustDetails)stepContext.Options;

                   return await stepContext.EndDialogAsync(dustDetails, cancellationToken);
               }
               else
               {
                   return await stepContext.EndDialogAsync(null, cancellationToken);
               }
           }
           */
        /* private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
         {
             dustDetails.Dustservice = stepContext.Context.Activity.Text;

             if (dustDetails.Dustservice == "초미세먼지")
             {
                 string msg = "http://cleanair.seoul.go.kr/air_city.htm?method=measure&grp1=pm25";
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
                 return await stepContext.EndDialogAsync(null, cancellationToken);
             }

             else if (dustDetails.Dustservice == "미세먼지")
             {
                 string msg = "http://cleanair.seoul.go.kr/air_city.htm?method=measure&grp1=pm10";
                 await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
               return await stepContext.EndDialogAsync(null, cancellationToken);
             }

             else if (dustDetails.Dustservice == "날씨")
             {
                 // string msg = "http://www.weather.go.kr/weather/observation/currentweather.jsp";
                 string msg = "http://www.weather.go.kr/weather/main.jsp";
                 await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
               return await stepContext.EndDialogAsync(null, cancellationToken);
             }

             return await stepContext.EndDialogAsync(null, cancellationToken);
         }
         */
    }
}