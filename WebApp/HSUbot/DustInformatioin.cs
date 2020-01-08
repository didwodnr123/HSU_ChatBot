using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace HSUbot
{
    public class DustInformation
    {

        private const String DUSTAPIKEY = "O0iOIuNyfXzOdW%2ByvKxdCuPR1PshWXlN4X3v3W4fy0KXyJ2hJqvJl0sLuCHT6ZRfgm67jp5X2BlkWCcs8zpqFg%3D%3D";
        //API
        private const String DUSTRUTEURL = "http://openapi.airkorea.or.kr/" ;
        // 서비스 요청 URL
        private const byte DUSTSTATIONID = 0;

        public DustInformation() { }

        public static async Task<string> GetDustInformationAsync(string dustservice, string dosistationname) // dustDetail.Dustservice, dustDetail.StationName  서울(종로구,광진구) 부산(광복동 초량동) -> 서울(서울미세먼지) 부산(부산미세먼지)

        // dustDetail.Dustservice, dustDetail.StationName
        {   // dustservice = 날씨 미세먼지 초미세먼지  stationname  서울 종로구 
           
            string msg = String.Empty;
            string galastationname = await GetDosiStationnameAsync(dosistationname);// 서울이 들어간ㄷ. 

         // List<string> stationInfor = await GetDustStationAsync(dosistationname, galastationname);// 서울 종로

            string dusturl = $"{DUSTRUTEURL}openapi/services/rest/ArpltnInforInqireSvc/getMsrstnAcctoRltmMesureDnsty?stationName={galastationname}"
                               + $"&dataTerm=month&pageNo=1&numOfRows=10&ServiceKey={ DUSTAPIKEY }&ver=1.3";


            XmlDocument xml = await DustApiReponseToXMLAsync(dusturl); //api 데이터 요청
            if (xml != null)
            {
                XmlNode root = xml.SelectNodes("response")[0].SelectNodes("body")[0].SelectNodes("items")[0].SelectNodes("item")[0];
                string pm10Value = root.SelectNodes("pm10Value")[0].InnerText;
               int pm10Valueint = Int32.Parse(pm10Value);

                string pm10Value24 = root.SelectNodes("pm10Value24")[0].InnerText;



             
                  if(pm10Valueint <= 30)
                   {
                       msg = $"{dosistationname} 미세먼지(PM10) 농도는 {pm10Value} 이며 , {dosistationname} 미세먼지 24시간 예측이동농도는 {pm10Value24} 입니다. \n  " +
                              $"{dosistationname} 상태는  좋음 입니다. \n" + 
                              $" 참고 자료 : 에어코리아 Open API " ;

                   }
   
                else if(pm10Valueint <= 80)
                    {
                    msg = $"{dosistationname} 미세먼지(PM10) 농도는 {pm10Value} 이며 , {dosistationname} 미세먼지 24시간 예측이동농도는 {pm10Value24} 입니다. \n  " +
                               $"{dosistationname} 상태는  보통 입니다. \n" +
                               $" 참고 자료 : 에어코리아 Open API ";

                }
                else if (pm10Valueint <= 150)
                {
                    msg = $"{dosistationname} 미세먼지(PM10) 농도는 {pm10Value} 이며 , {dosistationname} 미세먼지 24시간 예측이동농도는 {pm10Value24} 입니다. \n  " +
                               $"{dosistationname} 상태는  나쁨 입니다. \n" +
                               $" 참고 자료 : 에어코리아 Open API ";

                }
                else 
                {
                    msg = $"{dosistationname} 미세먼지(PM10) 농도는 {pm10Value} 이며 , {dosistationname} 미세먼지 24시간 예측이동농도는 {pm10Value24} 입니다. \n  " +
                               $"{dosistationname} 상태는  매우나쁨 입니다. \n" +
                               $" 참고 자료 : 에어코리아 Open API ";

                }
                
            }

            return msg;
        }

        private static async Task<string> GetDosiStationnameAsync(string dosistationname) //첫번째 서울, 부산 들어간다. 
        {
            string url = $"{DUSTRUTEURL}openapi/services/rest/ArpltnInforInqireSvc/getCtprvnRltmMesureDnsty?sidoName={dosistationname}"
                       + $"&pageNo=1&numOfRows=10&ServiceKey={DUSTAPIKEY}&ver=1.3";
             // 서울 api 안에 있는 구 를 다 뽑아온다. 
            string galastationname = String.Empty;
           

            XmlDocument xml = await DustApiReponseToXMLAsync(url);
            if (xml != null)
            {
                XmlNode root = xml.SelectNodes("response")[0].SelectNodes("body")[0].SelectNodes("items")[0].SelectNodes("item")[2];
                galastationname = root.SelectNodes("stationName")[0].InnerText;
            }
            return galastationname; // 종로구  // 광복동 
        }




      /*private static async Task<List<string>> GetDustStationAsync(string dosistationname, string galastationname)
        {


            string url = $"{DUSTRUTEURL}openapi/services/rest/ArpltnInforInqireSvc/getCtprvnRltmMesureDnsty?sidoName={dosistationname}"
                       + $"&pageNo=1&numOfRows=10&ServiceKey={DUSTAPIKEY}&ver=1.3";

            List<string> stationInfor = new List<string>();
            stationInfor.Clear();


            XmlDocument xml = await DustApiReponseToXMLAsync(url);

            if (xml != null)
            {
                XmlNodeList root = xml.SelectNodes("response")[0].SelectNodes("body")[0].SelectNodes("items")[0].SelectNodes("item");
                foreach (XmlNode node in root)
                {
                    if (node.SelectNodes("stationName")[0].InnerText == galastationname)
                    {
                        string station = node.SelectNodes("stationName")[0].InnerText;
                       // string stationOrder = node.SelectNodes("seq")[0].InnerText;
                        stationInfor.Add(station);
                       // stationInfor.Add(stationOrder);
                        break;
                    }
                }
            }

            return stationInfor;
        }
        */

    

        private static async Task<XmlDocument> DustApiReponseToXMLAsync(string url)
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
