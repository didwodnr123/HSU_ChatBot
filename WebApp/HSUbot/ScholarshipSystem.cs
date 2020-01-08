using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSUbot
{
    public class ScholarshipSystem 
    {
        public ScholarshipSystem() { }

        public static string GetScholarshipSystem()
        {
            string freshSS = String.Empty;
            string freshSS_url = "https://www.hansung.ac.kr/web/www/life_02_01_t1";

            string talentSS = String.Empty;
            string talentSS_url = "https://www.hansung.ac.kr/web/www/life_02_01_t4";

            freshSS = $"신입생 장학제도 : {freshSS_url}";
            talentSS = $"인재장학금 장학제도 : {talentSS_url}";

            return freshSS;
        }
    }
}
