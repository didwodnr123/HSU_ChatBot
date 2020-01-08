using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using HSUbot.Details;

namespace HSUbot
{
    public class Professors
    {
        public List<ProfessorsNum> ProfessorsDB(string Input)
        {
            string str = @"Server=cf-mssql-dev.database.windows.net; Initial Catalog = rdb1; uid=cloudfos; pwd=zmffkdnemvhtm1!";
            var professorsList = new List<ProfessorsNum>();

            using (SqlConnection cnn = new SqlConnection(str))
            {
                cnn.Open();

                string sqlTrack = $"SELECT * FROM dbo.Professors WHERE 트랙 LIKE N'%{Input}%'";
                string sqlName = $"SELECT * FROM dbo.Professors WHERE 교수님 LIKE N'%{Input}%'";
                professorsList.AddRange(ProfessorsListSQL(cnn, sqlTrack));
                professorsList.AddRange(ProfessorsListSQL(cnn, sqlName));
                professorsList = professorsList.Distinct().ToList();
            }
            return professorsList;
        }

        private List<ProfessorsNum> ProfessorsListSQL(SqlConnection sqlConnection, string sql)
        {
            var list = new List<ProfessorsNum>();
            using (var command = new SqlCommand(sql, sqlConnection))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var college = dataReader.GetValue(1).ToString();
                        var department = dataReader.GetValue(2).ToString();
                        var track = dataReader.GetValue(3).ToString();
                        var name = dataReader.GetValue(4).ToString();
                        var lab = dataReader.GetValue(5).ToString();
                        var adress = dataReader.GetValue(6).ToString();
                        var email = dataReader.GetValue(7).ToString();
                        adress = PhoneNumFormating(adress);
                        list.Add(new ProfessorsNum
                        {
                            College = college,
                            Department = department,
                            Track = track,
                            Name = name,
                            Lab = lab,
                            Adress = adress,
                            Email = email
                        });
                    }
                }
            }
            return list;
        }

        private string PhoneNumFormating(string adress)
        {
            if(adress.Length == 4)
            {
                adress = "02760" + adress;
            }
            return adress;
        }
    }
}
