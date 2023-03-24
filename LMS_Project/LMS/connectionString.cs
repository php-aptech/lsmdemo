using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LMS_Project.LMS
{
    public class connectionString
    {
        public static string getConnectionString()
        {
            string result = ConfigurationManager.ConnectionStrings["DbContext"].ConnectionString;
            return result;
        }
    }
}