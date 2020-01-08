using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using HSUbot.Details;

namespace HSUbot
{
    public class PhoneAdress
    {
        public List<PhoneNum> PhoneDB(string Input)
        {
            string str = @"Server=cf-mssql-dev.database.windows.net; Initial Catalog = rdb1; uid=cloudfos; pwd=zmffkdnemvhtm1!";

            var phoneList = new List<PhoneNum>();
            //SqlConnection cnn = new SqlConnection(connetionString);
            //cnn.Open();
            using (SqlConnection cnn = new SqlConnection(str))
            {
                cnn.Open();
                
                string sqlbuser = $"SELECT * FROM dbo.PhoneAdress WHERE Department LIKE N'%{Input}%'";
                string sqlname = $"SELECT * FROM dbo.PhoneAdress WHERE Name LIKE N'%{Input}%'";
                phoneList.AddRange(PhoneListSQL(cnn, sqlbuser));
                phoneList.AddRange(PhoneListSQL(cnn, sqlname));
                phoneList = phoneList.Distinct().ToList();
            }
            return phoneList;
        }

        private List<PhoneNum> PhoneListSQL(SqlConnection sqlConnection, string sql)
        {
            var list = new List<PhoneNum>();
            using (var command = new SqlCommand(sql, sqlConnection))
            {
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var department = dataReader.GetValue(1).ToString();
                        var name = dataReader.GetValue(2).ToString();
                        var phoneNumber = dataReader.GetValue(3).ToString();
                        phoneNumber = PhoneNumFormating(phoneNumber);
                        list.Add(new PhoneNum
                        {
                            Department = department,
                            Name = name,
                            PhoneNumber = phoneNumber
                        });
                    }
                }
            }
            return list;
        }

        private string PhoneNumFormating(string phoneNumber)
        {
            if(phoneNumber.Length == 4)
            {
                phoneNumber = "02760" + phoneNumber;
            }
            else
            {
                phoneNumber = "02" + phoneNumber;
            }
            return phoneNumber;
        }
    }
}
