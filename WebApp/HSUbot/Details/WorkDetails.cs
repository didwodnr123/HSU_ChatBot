using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HSUbot.GraphDB;

namespace HSUbot.Details
{
    public class WorkDetails : MainDetail
    {
        public WorkDetails()
        {
            sebu = new List<string>();
            ablt = new List<Ablt>();
            selectAbltIndex = -1;
        }
        public string work { get; set; }
        //검색한직업 이름
        public List<string> sebu { get; set; }
        //세부능력 이름
        public List<Ablt> ablt { get; set; }
        //능력 검색 결과리스트
        public int selectAbltIndex { get; set; }
        //선택한 직업 위치
    }
}
