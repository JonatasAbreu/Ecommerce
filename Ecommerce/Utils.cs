using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Ecommerce
{
  
    public class Utils

    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        SqlDataReader sdr;
        DataTable dt;

        public static string getConnection()
        {
            return ConfigurationManager.ConnectionStrings["cs"].ConnectionString;
        }

        public static bool isValidExtension( string fileName) 
        {
            bool isValid = false;

            string[] fileExension = { ".jpg", ".jpeg", ".png" };
            foreach ( string file in fileExension )
            {
                if (fileName.Contains(file))
                {
                    isValid = true;
                    break;
                }
            }

            return isValid;
        }

        public static string getUniqueId() 
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
    }
}