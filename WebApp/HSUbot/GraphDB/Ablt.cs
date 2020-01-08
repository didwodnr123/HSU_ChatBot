using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSUbot.GraphDB
{
    public class Ablt
    {
        /// <summary>
        /// 능력단위노드ID
        /// </summary>
        public int AbltId { get; set; }
        /// <summary>
        /// 능력단위 코드
        /// </summary>
        public string AbltCd { get; set; }
        /// <summary>
        /// 능력단위 코드명
        /// </summary>
        public string AbltCn { get; set; }
        /// <summary>
        /// 능력단위 정의
        /// </summary>
        public string AbltDfnCn { get; set; }
        /// <summary>
        /// 능력단위 수준
        /// </summary>
        public string AbltUnitLvl { get; set; }
    }
}
