using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder;
using HSUbot.Details;
using Newtonsoft.Json.Linq;

namespace HSUbot
{
    public class LuisHelper
    {
        private LuisApplication luisApplication { get; }
        private IConfiguration configuration { get; set; }
        private RecognizerResult recognizerResult;
        public LuisHelper(IConfiguration configuration)
        {
            luisApplication = new LuisApplication(configuration["LuisAppId"], configuration["LuisAPIKey"], configuration["LuisAPIHostName"]);
        }
        public async Task<MainDetail> ExecuteLuisQuery(ILogger logger, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            MainDetail detail = new MainDetail();
            try {
                LuisRecognizer luisRecognizer = new LuisRecognizer(luisApplication);
                recognizerResult = await luisRecognizer.RecognizeAsync(turnContext, cancellationToken);
                //높은 점수의 의도 저장
                var (intent, score) = recognizerResult.GetTopScoringIntent();
                //대화의도에 따른 엔티티 저장
                intent = "트랙정보카드";
                switch (intent)
                {
                    case "버스":
                        BusDetails busDetails = new BusDetails();
                        busDetails.intent = intent;
                        busDetails = getLuisEntites(busDetails);
                        detail = busDetails;
                        break;
                    case "미세먼지":
                        DustDetails dustDetails = new DustDetails();
                        dustDetails.intent = intent;
                        dustDetails = getDustLuisEntites(dustDetails);
                        detail = dustDetails;
                        break;
                    case "장학제도":
                        detail.intent = intent;
                        break;
                    case "전화번호":
                        PhoneDetails phoneDetails = new PhoneDetails();
                        phoneDetails.intent = intent;
                        phoneDetails = getPhoneEntites(phoneDetails);
                        detail = phoneDetails;
                        break;
                    case "교수님":
                        ProfessorsDetail pfDetail = new ProfessorsDetail();
                        pfDetail.intent = intent;
                        pfDetail = getProfessorEntites(pfDetail);
                        detail = pfDetail;
                        break;
                    case "트랙정보":
                        TrackInfoDetails trackInfoDetails = new TrackInfoDetails();
                        trackInfoDetails.intent = intent;
                        detail = trackInfoDetails;
                        break;
                    case "트랙정보카드":
                        TrackInfoCardDetail trackInfoCardDetail = new TrackInfoCardDetail();
                        trackInfoCardDetail.intent = intent;
                        detail = trackInfoCardDetail;
                        break;
                    case "공지사항":
                        NoticeDetail noticeDetail = new NoticeDetail();
                        noticeDetail.intent = intent;
                        detail = noticeDetail;
                        break;
                    case "맛집":
                        MatjeepchoochunDetail matjeepchoochunDetail = new MatjeepchoochunDetail();
                        matjeepchoochunDetail.intent = intent;
                        matjeepchoochunDetail = getMatjeepchoochunEntites(matjeepchoochunDetail);
                        detail = matjeepchoochunDetail;
                        break;
                    case "학식메뉴":
                        detail.intent = intent;
                        break;
                    case "직업":
                        WorkDetails workDetails = new WorkDetails();
                        workDetails.intent = intent;
                        workDetails = getWorkentites(workDetails);
                        detail = workDetails;
                        break;
                }
            }
            catch(Exception e)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("예기치 못한 오류가 발생하였습니다. 다시 시도해주십시오\n" +e.Message),cancellationToken);
            }

            return detail;
        }

        private BusDetails getLuisEntites(BusDetails busDetails)
        {
            busDetails.RouteNum = recognizerResult.Entities["노선번호"]?.FirstOrDefault()?.FirstOrDefault()?.ToString().Replace(" ", "");
            busDetails.StationName = recognizerResult.Entities["버스정류장"]?.FirstOrDefault()?.FirstOrDefault()?.ToString().Replace(" ", "");
            return busDetails;
        }
        private DustDetails getDustLuisEntites(DustDetails dustDetails)
        {
            dustDetails.Dustservice = recognizerResult.Entities["지역자음"]?.FirstOrDefault()?.FirstOrDefault()?.ToString().Replace(" ", "");
            dustDetails.Dosistationname = recognizerResult.Entities["도시이름"]?.FirstOrDefault()?.FirstOrDefault()?.ToString().Replace(" ", "");
            return dustDetails;
        }

        private PhoneDetails getPhoneEntites(PhoneDetails phoneDetails)
        {
            phoneDetails.Name = recognizerResult.Entities["부서"]?.FirstOrDefault()?.ToString().Replace(" ", "");

            return phoneDetails;
        }

        private ProfessorsDetail getProfessorEntites(ProfessorsDetail pfDetail)
        {
            pfDetail.Name = recognizerResult.Entities["교수님"]?.FirstOrDefault()?.ToString().Replace(" ", "");

            return pfDetail;
        }

        private MatjeepchoochunDetail getMatjeepchoochunEntites(MatjeepchoochunDetail matjeepchoochunDetail)
        {
            matjeepchoochunDetail.Category = recognizerResult.Entities["맛집"]?.FirstOrDefault()?.ToString().Replace(" ", "");

            return matjeepchoochunDetail;
        }      

        private WorkDetails getWorkentites(WorkDetails workDetails)
        {
            workDetails.work = recognizerResult.Entities["직업"]?.FirstOrDefault()?.ToString().Replace(" ", "");
            JArray sebu = (JArray)recognizerResult.Entities.SelectToken("세부능력");
            int i = 0;
            try
            {
                foreach (JArray value in sebu)
                {
                    workDetails.sebu.Add(value?.FirstOrDefault().ToString().Replace(" ", ""));
                }
            }
            catch(Exception e)
            {
            }
            return workDetails;
        }

    }
}
