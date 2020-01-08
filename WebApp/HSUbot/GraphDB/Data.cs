using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSUbot.GraphDB
{
    public class Data
    {
        /// <summary>
        /// 데이터 노드 ID
        /// </summary>
        public int DataId { get; set; }
        /// <summary>
        /// 데이터사전 코드
        /// </summary>
        public string NcsCd { get; set; }
        /// <summary>
        /// 데이터사전 코드명
        /// </summary>
        public string NcsCn { get; set; }
        /// <summary>
        /// 데이터사전 분류 레벨
        /// </summary>
        public string NcsCdLvl { get; set; }
        /// <summary>
        /// 데이터샂너 분류명
        /// </summary>
        public string NcsCdLvlNm { get; set; }
        /// <summary>
        /// 카테고리
        /// 세분류에만 제공
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 능력단위 목록 갯수
        /// 세분류에만 제공
        /// </summary>
        public string DtyDfnCn { get; set; }
    }
}
