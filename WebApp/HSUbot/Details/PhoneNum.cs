using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSUbot.Details
{
    public class PhoneNum
    {
        public string Department { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }
            PhoneNum p = obj as PhoneNum;
            if((object)p == null)
            {
                return false;
            }

            return (Department == p.Department) && (Name == p.Name) && (PhoneNumber == PhoneNumber);
        }
        public override int GetHashCode()
        {
            return Department.GetHashCode() ^ Name.GetHashCode() ^ PhoneNumber.GetHashCode();
        }
    }
}
