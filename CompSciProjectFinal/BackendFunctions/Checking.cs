using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CompSciProjectFinal
{
    internal class Checking //Class containing a few functions for validation
    {
        private static bool RegexValidate(string stringToValidate, string regexString)
        {
            return new Regex(regexString, RegexOptions.IgnoreCase).IsMatch(stringToValidate);
        }
        
        public static bool WillProductTransactionStayAboveZero(ProductTransaction productTransaction)
        {
            ProductTransactionData productTransactionData = DatabaseManagementV2.GetTransactionHistoryForProductById(productTransaction.productId);
            if (productTransaction.confirmed)
            {
                if (productTransactionData.totalStock + productTransaction.changeAmount < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (productTransactionData.totalSellableStock + productTransaction.changeAmount < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool WillMaterialTransactionStayAboveZero(MaterialTransaction materialTransaction)
        {
            MaterialTransactionData materialTransactionData = DatabaseManagementV2.GetMaterialTransactionsByMaterialId(materialTransaction.materialId);
            if (materialTransactionData.totalStock + materialTransaction.amount < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool ValidateEmailAddressFormatting(string emailInput)
        {
            if (emailInput == null)
            {
                throw new ArgumentNullException("No email address was given. Likely not user error");
            }
            else
            {
                return RegexValidate(emailInput, @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$");
            }
        }

        public static bool ValidatePostcodeFormatting(string postcodeInput)
        {
            if (postcodeInput == null)
            {
                throw new ArgumentNullException("No postcode was given. Likely not user error");
            }
            else
            {
                return RegexValidate(postcodeInput, @"(GIR 0AA)|((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)|(([A-Z-[QVX]][0-9][A-HJKSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY]))))\s?[0-9][A-Z-[CIKMOV]]{2})$");
            }
        }

        public static bool ValidateSmtpFormatting(string smtpInput)
        {
            if (smtpInput == null)
            {
                throw new ArgumentNullException("No server address given");
            }
            else
            {
                return RegexValidate(smtpInput, @"[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$");
            }
        }
    }
}
