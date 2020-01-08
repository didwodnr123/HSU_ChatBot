using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSUbot.Details
{
    public class ProfessorsNum
    {
        public string College { get; set; }
        public string Department { get; set; }
        public string Track { get; set; }
        public string Name { get; set; }
        public string Lab { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            ProfessorsNum p = obj as ProfessorsNum;
            if((object)p == null)
            {
                return false;
            }

            return (College == p.College) && (Department == p.Department) && (Track == p.Track) && (Name == p.Name) && (Lab == p.Lab) && (Adress == Adress) && (Email == p.Email);
        }

        public override int GetHashCode()
        {
            return College.GetHashCode() ^ Department.GetHashCode() ^ Track.GetHashCode() ^ Name.GetHashCode() ^ Lab.GetHashCode() ^ Adress.GetHashCode() ^ Email.GetHashCode();
        }
    }
}
