using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompSciProjectFinal //This file contains definitions for the object classes that are used throughout the program. In the prototype, they were in random places throughout the program
{
    public partial class UserDetails //Stores details for a user of the program
    {
        public int user_id;
        public string text_user_id;
        public string display_name;
        public int admin_access;
    }

    public partial class PasswordDetails //Stores details of the user's password.
    {
        public string hash;
        public string salt;
    }

    public partial class ProductDetails //Stores details of a product
    {
        public string productId;
        public string productName;
        public double productCost;
    }

    public partial class ProductTransaction //Stores details of a product transaction (change in stock lvl)
    {
        public long transactionId;
        public DateTime time;
        public string productId;
        public int changeAmount;
        public bool confirmed;
    }

    public partial class FinancialTransaction //Stores details of a financial transaction (incoming or outgiong)
    {
        public DateTime time;
        public double amount;
    }

    public partial class MaterialDetails //Stores details for a material
    {
        public string materialId;
        public string materialName;
    }

    public partial class MaterialTransaction //Stores details for a material transaction
    {
        public long transactionId;
        public int amount;
        public string materialId;
    }

    public partial class ActiveUseFileReference //Stores a refernece for a file in active use
    {
        public string trueFileLocation;
        public string fileId;
    }

    public partial class OverviewSale //Basic sale used in the sale overview screen
    {
        public long saleId;
        public string saleTime; //String rather than datetime so it can be placed directly on a UI element without conversion
    }

    public partial class ProdTransSaleAssc //Stores data for a ProductTransactionSalesAssociation (Sale item)
    {
        public ProductTransaction productTransaction;
        public FinancialTransaction financialTransaction; 
    }

    public partial class CustomerDetails //Stores customer details
    {
        public int customerId;
        public string customerFirstName;
        public string customerLastName;
        public string customerEmail;
        public string customerAddress1;
        public string customerAddress2;
        public string customerAddress3;
        public string customerPostcode;
        public string customerNotes;
    }

    public partial class SaleDetailsV2 //Stores sale details (V2 suffix because it is an iterationon the version used in the prototype and they both existed in conjunction with each other during development
    {
        public long saleId;
        public double price;
        public UserDetails userDetails;
        public CustomerDetails customerDetails;
        public string paymentLink;
        public int status;
        public int method;
        public List<ProdTransSaleAssc> prodTransSaleAsscs;
    }

    public partial class Session //stores the details for a session
    {
        public long sessionId;
        public UserDetails userDetails;
    }

    public partial class SessionEvent //stores the details for a session event
    {
        public Session session;
        public long eventId;
        public string comment;
    }

    public partial class ProductRequirement //Stores the details for a requirement for a product
    {
        public int requirementId;
        public MaterialDetails materialDetails;
        public ProductDetails productDetails;
    }

    public partial class MaterialTransactionData //A material transaction data object to store the transaction data and a total amount
    {
        public List<MaterialTransaction> materialTransactions;
        public int totalStock;
    }

    public partial class ProductTransactionData //A material product transaction data object to store the transaction data and a total amount
    {
        public List<ProductTransaction> productTransactions;
        public int totalStock;
        public int totalSellableStock;
    }

    public partial class OverviewScreenStats //stores the data used for the overview screen
    {
        public double incomeAllTime;
        public int salesAllTime;
        public int salesPastWeek;
        public double incomePastWeek;
        public double outgoingsPastWeek;
    }
}
