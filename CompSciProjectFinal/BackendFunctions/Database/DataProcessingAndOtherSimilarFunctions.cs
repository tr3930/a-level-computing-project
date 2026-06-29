using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompSciProjectFinal
{
    class DataFunctions
    {
        public static long GenerateIdForPrimaryKey()
        {
            TimeSpan timeSinceX = DateTime.Now - new DateTime(2015, 1, 1); //This will be similar to unix epoch but it starts from 1990 not 1970
            return (long)timeSinceX.TotalMilliseconds;
        }

        public static DateTime FindPrimaryKeyTime(long key)
        {
            DateTime dateTime = new DateTime(2015, 1, 1);
            return dateTime.AddMilliseconds(key);
        }

        public static string GenerateRandomAlphaNeumericString(int length) //used for generating an alphaneumeric string
        {
            Random random = new Random(); //Defining the random generator
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //All characters that can be used in the string
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()); //Taking the characters above and turning them into a random string
        }

        public static type NullCheckTypeConversion<type>(object dbentry)
        {
            if (dbentry == null || dbentry == DBNull.Value)
            {
                return default(type);
            }
            else
            {
                return (type)dbentry;
            }
        }


    }
}
