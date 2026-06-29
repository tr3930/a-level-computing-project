using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CompSciProjectFinal
{
    internal class Security
    {
        public static string GenerateNewSalt() //This generates the salt which will be used with the password to increase obfuscation
        {
            return DataFunctions.GenerateRandomAlphaNeumericString(32);
        }

        public static string GeneratePasswordHash(string password, string salt)
        {
            string hashOutput = ""; //Variable that will store the hashed string
            string hashInput = salt + password + salt; //Creating the input string
            SHA256 sha256 = SHA256.Create(); //This is what will hash the password
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashInput)); //turning the string intosha256 bytes
            foreach (byte b in hashBytes)
            {
                hashOutput += $"{b:X2}"; //Converting these bytes back to a string
            }
            return hashOutput; //returning the string
        }

        public static bool VerifyPassword(string password, string userTextId) //Used to verify the user's password on login
        {
            PasswordDetails details = DatabaseManagementV2.GetUsersPasswordDetailsByTextId(userTextId); //Gets password details from database
            password = GeneratePasswordHash(password, details.salt); //Hashes password given by user
            if (password == details.hash) //Is the password correct?
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void UpdateUserPassword(string textUserId, string pin)
        {
            PasswordDetails passwordDetails = new PasswordDetails()
            {
                salt = GenerateNewSalt()
            };
            passwordDetails.hash = GeneratePasswordHash(pin, passwordDetails.salt);
            DatabaseManagementV2.UpdateUserPassword(passwordDetails, textUserId);
        }
    }
}
