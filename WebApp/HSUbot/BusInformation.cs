using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using System.IO;

namespace HSUbot
{
    public class BusInformation
    {
        private const String SEOULBUSAPIKEY = "M5WwmZIUyxwShvv%2FQ9nw6cvD%2FuMkkIqkj2cHG4LGkoAtfjwYJZFrActDvQvmgT4h3ki3JSjOyur4cH93fKGMRA%3D%3D";
        // APIKEY
        private const String ABSOLRUTEURL = "http://ws.bus.go.kr/api/rest/"; 
        // 서비스 요청 URL
        private const byte STATIONID = 0;
        private const byte STATIONORDER = 1;
        //정류장ID와 순서를 받아오기 위한 인덱스넘버
        //private const byte STATIONNAME = 2;
        //private const String BUSROUTESERVICE = "http://openapi.tago.go.kr/openapi/service/";


        public BusInformation() { }
       
        //노선번호와 정류장이름으로 버스 도착 예정시간 반환
        public static async Task<string> GetBusArrTimeAsync(string routeNum, string stationName)
       { 
            string msg = String.Empty;
            string routeId = await GetRouteIdAsync(routeNum); // 노선번호로 노선 ID 얻기
            List<string> stationInfor = await GetStationIdAsync(stationName, routeId); // 정류장 이름으로 정류장 ID와 순서 얻기

            string url = $"{ABSOLRUTEURL}arrive/getArrInfoByRoute?ServiceKey={SEOULBUSAPIKEY}" +
                $"&stId={stationInfor[STATIONID]}&busRouteId={routeId}&ord={stationInfor[STATIONORDER]}";

            XmlDocument xml = await BusApiReponseToXMLAsync(url); //api 데이터 요청
            if(xml != null)
            {
                XmlNode root = xml.SelectNodes("ServiceResult")[0].SelectNodes("msgBody")[0].SelectNodes("itemList")[0];
                string arrmsg1 = root.SelectNodes("arrmsg1")[0].InnerText;
                string arrmsg2 = root.SelectNodes("arrmsg2")[0].InnerText;
                msg = $"{stationName}정류장으로 도착하는 첫번째 {routeNum}번 버스는 {arrmsg1}, 두번째 버스는 {arrmsg2}에 도착할 예정입니다";
            }

            return msg;
        }

        //버스 노선 이름으로 노선 id 반환
        private static async Task<string> GetRouteIdAsync(string routeNum) 
        {
            string url = $"{ABSOLRUTEURL}busRouteInfo/getBusRouteList?ServiceKey={SEOULBUSAPIKEY}&strSrch={routeNum}";
            string routeId = String.Empty;

            XmlDocument xml = await BusApiReponseToXMLAsync(url);
            if (xml != null)
            {
                XmlNode root = xml.SelectNodes("ServiceResult")[0].SelectNodes("msgBody")[0].SelectNodes("itemList")[0];
                routeId = root.SelectNodes("busRouteId")[0].InnerText;
            }
            return routeId;
        }
        //정류소 이름으로 정류소 순번과 ID 반환
        private static async Task<List<string>> GetStationIdAsync(string stationName, string routeId)
        {
            string url = $"{ABSOLRUTEURL}busRouteInfo/getStaionByRoute?ServiceKey={SEOULBUSAPIKEY}&busRouteId={routeId}";
            List<string> stationInfor = new List<string>();
            stationInfor.Clear();
            

            XmlDocument xml = await BusApiReponseToXMLAsync(url);
            if(xml != null)
            {
                XmlNodeList root = xml.SelectNodes("ServiceResult")[0].SelectNodes("msgBody")[0].SelectNodes("itemList");
                foreach(XmlNode node in root)
                {
                    if(node.SelectNodes("stationNm")[0].InnerText == stationName)
                    {
                        string stationId = node.SelectNodes("station")[0].InnerText;
                        string stationOrder = node.SelectNodes("seq")[0].InnerText;
                        stationInfor.Add(stationId);
                        stationInfor.Add(stationOrder);
                        break;
                    }
                }
            }

            return stationInfor;
        }

        //받아온 URL 에서 api 요청 후 xml형식으로 변환
        private static async Task<XmlDocument> BusApiReponseToXMLAsync(string url) 
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage reponse = await client.GetAsync(url);

            if (reponse.IsSuccessStatusCode)
            {
                XmlDocument xml = new XmlDocument();

                Stream stream = await reponse.Content.ReadAsStreamAsync();
                xml.Load(stream);

                return xml;
            }
            else
            {
                return null;
            }

        }
       

    }
}
