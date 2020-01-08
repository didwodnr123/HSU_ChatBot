using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using System.Net.Http;
using Microsoft.Bot.Builder.Dialogs;
using HSUbot.Details;
using Neo4jClient;
using Neo4jClient.Cypher;
using HSUbot.GraphDB;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Text.RegularExpressions;

namespace HSUbot.Dialogs
{
    /// <summary>
    /// 직업관련 대화 흐름
    /// </summary>
    public class WorkDialog : CancelAndHelpDialog
    {
        private WorkDetails workDetails;
        private const string GRAPHDB_ENDPOINT = "http://cf-neo4j-dev.eastasia.azurecontainer.io:7474/db/data";
        private const string GRAPHDB_USERNAME = "neo4j";
        private const string GRAPHDB_PASSWORD = "zmffkdnemvhtm1!";
        private static readonly TimeSpan GRAPHDB_HTTP_TIMEOUT = TimeSpan.FromMinutes(60);
        private ICypherFluentQuery cypher;
        private GraphClient client;

        private List<string> sebuList = new List<string> { "자격증", "지식및기술", "훈련", "전공", "수행태도" };
        private List<string> EndList = new List<string> { "다른 세부능력 보기", "검색한 다른 직업 보기", "새로운 직업 검색", "끝내기" };
        public WorkDialog() : base(nameof(WorkDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                WorkJobCheckAsync,
                WorkSelectAsync,
                SebuSelectAsync,
                SebuViewAsync,
                EndOrReturnAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> WorkJobCheckAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            client = new GraphClient(new Uri(GRAPHDB_ENDPOINT), new HttpClientWrapper(GRAPHDB_USERNAME, GRAPHDB_PASSWORD, new HttpClient() { Timeout = GRAPHDB_HTTP_TIMEOUT }));
            client.Connect();
            //그래프 데이터 클라이언트 연동
            workDetails = (WorkDetails)stepContext.Options;
            //루이즈로 받아온 엔터티 저장
            if (workDetails.work == null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("알고싶은 직무를 입력해 주십시오")
                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(cancellationToken);
            }

        }
       
        private async Task<DialogTurnResult> WorkSelectAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //직업정보 없을 시 결과값 받아옴
            if (workDetails.work == null)
                workDetails.work = stepContext.Result.ToString();
            string msg = "";
            List<string> abltNames = new List<string>();
            //원래 있던 능력 단위 없을시에만 사이퍼쿼리 요청
            if (workDetails.ablt.Count == 0) {
                workDetails.ablt = client.Cypher.Match("(ablt:Ablt)").
                Where($"(ablt.AbltCn) = \"{workDetails.work}\"").
                Return(ablt => ablt.As<Ablt>()).
                Results.ToList();
                foreach (Ablt ablt in workDetails.ablt)
                {
                    abltNames.Add(ablt.AbltCn);
                }
            }
            //받아온 능력단위가 여러개 일시 그 중 한가지 선택
            if (workDetails.ablt.Count > 1 && workDetails.selectAbltIndex == -1)
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text($"요청하신 관련 직무가 {abltNames.Count}가지 있습니다.\n직업들 중에서 선택해 주세요"),
                    RetryPrompt = MessageFactory.Text("다음 내용중에서 골라주세요"),
                    Choices = ChoiceFactory.ToChoices(abltNames)
                }, cancellationToken) ;
            }
            //능력단위 결과 없을 시 다시 시작
            else if(workDetails.ablt.Count == 0)
            {
                workDetails.work = null;
                return await stepContext.ReplaceDialogAsync(nameof(WorkDialog), workDetails, cancellationToken);
            }
            //한개 일때
            else
            {
                return await stepContext.NextAsync(workDetails.ablt[0].AbltCn, cancellationToken);
            }
        }
        
        private async Task<DialogTurnResult> SebuSelectAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string select = string.Empty;
            if(workDetails.ablt.Count > 1)
            {
                var choice = (FoundChoice)stepContext.Result;
                select = choice.Value;
            }
            else
            {
                select = stepContext.Result.ToString();
            }
            //선택한 직업의 리스트 인덱스 값 찾기
            if (workDetails.selectAbltIndex == -1)
            {
                foreach (Ablt ablt in workDetails.ablt)
                {
                    workDetails.selectAbltIndex++;
                    if (ablt.AbltCn.Equals(select))
                        break;
                }
            }

            Ablt selectAblt = workDetails.ablt[workDetails.selectAbltIndex];

            string msg = $"선택하신 직무는 {selectAblt.AbltCn} 입니다.\n" +
                $"선택하신 직무의 내용입니다\n" +
                $"{selectAblt.AbltDfnCn}";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);

            if(workDetails.sebu.Count == 0)
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("더 알고싶은 분야를 선택해주세요"),
                    RetryPrompt = MessageFactory.Text("다시 선택해 주세요"),
                    Choices = ChoiceFactory.ToChoices(sebuList)
                }, cancellationToken);
            }

            return await stepContext.NextAsync(cancellationToken);
        }

        private async Task<DialogTurnResult> SebuViewAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (workDetails.sebu.Count == 0)
            {
                var choice = (FoundChoice)stepContext.Result;
                workDetails.sebu.Add(choice.Value);
            }

            Ablt selectAblt = workDetails.ablt[workDetails.selectAbltIndex];
            string msg = $"{selectAblt.AbltCn}에 대해 선택하신 세부 내용은 다음과 같습니다.";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);

            //MATCH쿼리 직무 능력 노드 찾아놓기
            ICypherFluentQuery query = client.Cypher.Match("(ablt:Ablt)").
                            Where((Ablt ablt) => ablt.AbltCn == selectAblt.AbltCn);

            foreach (string sebuString in workDetails.sebu)
            {
                string sebuMsg = "";
                //원하는 세부 능력에 따른 쿼리 이어 붙인 후 결과값 도출
                switch (sebuString)
                {
                    case "훈련":
                        var abltTrng = query.Match("(ablt)-[:TRNG]->(trng:AbltTrng)").
                            Return(trng => trng.As<AbltTrng>()).Results;
                        sebuMsg += "----- 훈련 -----\n";
                        foreach (AbltTrng trng in abltTrng)
                            sebuMsg += (trng.Name + "\n");
                        break;
                    case "전공":
                        var abltMajor = query.Match("(ablt)-[:MAJOR]->(major:AbltMajor)").
                            Return(major => major.As<AbltMajor>()).Results;
                        sebuMsg += "----- 전공 -----\n";
                        foreach (AbltMajor major in abltMajor)
                            sebuMsg += (major.Name + "\n");
                        break;
                    case "자격증":
                        var abltCrfItem = query.Match("(ablt)-[:CRFITEM]->(crfItem:AbltCrfItem)").
                            Return(crfItem => crfItem.As<AbltCrfItem>()).Results;
                        sebuMsg += "----- 자격증 -----\n";
                        foreach (AbltCrfItem crfItem in abltCrfItem)
                            sebuMsg += (crfItem.Name + "\n");
                        break;
                    case "수행태도":
                        var abltAtt = query.Match("(ablt)-[:ATT]->(att:AbltAtt)").
                            Return(att => att.As<AbltAtt>()).Results;
                        sebuMsg += "----- 수행태도 -----\n";
                        foreach (AbltAtt att in abltAtt)
                            sebuMsg += (att.Name + "\n");
                        break;
                    case "지식및기술":
                        var abltKnwSkl = query.Match("(ablt)-[:KNWSKL]->(knwSkl:AbltKnwSkl)").
                            Return(knwSkl => knwSkl.As<AbltKnwSkl>()).Results;
                        sebuMsg += "-----지식 및 기술 -----\n";
                        foreach (AbltKnwSkl knwSkl in abltKnwSkl)
                            sebuMsg += (knwSkl.Name + "\n");
                        break;
                }
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(sebuMsg), cancellationToken);
            }
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("더 필요하신 것 있으신가요?"),
                RetryPrompt = MessageFactory.Text("아래에서 골라주세요"),
                Choices = ChoiceFactory.ToChoices(EndList)
            });
        }

        private async Task<DialogTurnResult> EndOrReturnAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            client.Dispose();
            var choice = (FoundChoice)stepContext.Result;
            string resultText = choice.Value;
            // 마지막 대화 분기(세부능력 다시 찾기, 검색결과 다시 보기, 새로운 직업 검색하기, 끝내기)
            if (resultText.Equals(EndList[0]))
            {
                workDetails.sebu = new List<string>();
                return await stepContext.ReplaceDialogAsync(nameof(WorkDialog), workDetails, cancellationToken);
            }
            else if (resultText.Equals(EndList[1]))
            {
                workDetails.sebu = new List<string>();
                workDetails.selectAbltIndex = -1;
                return await stepContext.ReplaceDialogAsync(nameof(WorkDialog), workDetails, cancellationToken);
            }
            else if (resultText.Equals(EndList[2]))
            {
                return await stepContext.ReplaceDialogAsync(nameof(WorkDialog), new WorkDetails { intent = "직업" }, cancellationToken);
            }

            return await stepContext.EndDialogAsync(cancellationToken);

        }

    }
}
