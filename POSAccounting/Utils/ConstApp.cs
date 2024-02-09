
using POSAccounting.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace POSAccounting.Utils
{
    public static class ConstActionType
    {
        public static string Select { get; } = "Select";
        public static string Insert { get; } = "Insert";
        public static string Update { get; } = "Update";
        public static string Delete { get; } = "Delete";
        public static string Print { get; } = "Print";
    }

    public static class ConstTask
    {
        public static string Users { get; } = "Users";
        public static string Roles { get; } = "Roles";
        public static string Invoices { get; } = "invoices";
        public static string Receipts { get; } = "receipts";
        public static string Clients { get; } = "Clients";
        public static string Products { get; } = "Products";
        public static string Categories { get; } = "Categories";
        public static string Stores { get; } = "Stores";
        public static string Accounts { get; } = "Accounts";
        public static string Journals { get; } = "Journals";
        public static string Reports { get; } = "Reports";
        public static string Visas { get; } = "Visas";
        public static string Settings { get; } = "Settings";
    }

    public static class ConstRole
    {
        public static string Admin { get; } = "Admin";
        public static string Shop { get; } = "Shop";
        public static string Customer { get; } = "Customer";
    }

    public static class ConstSymbol
    {
        public static string Cities { get; } = "CITIES";
        public static string Genders { get; } = "GENDERS";
        public static string Units { get; } = "UNITS";
    }

    public static class ConstAccounts
    {
        //1 Assests
        public static Guid Assests { get; } = new Guid("c7118ce8-3a34-4007-ada9-eeeb1ed86114");
        public static Guid CurrentAssets { get; } = new Guid("ed145be7-5f84-4169-bcb2-6de5aa446006");
        public static Guid BoxSafe { get; } = new Guid("1ecd022d-f7a6-4bbb-9015-ba3f7e6280c0");
        public static Guid Bank { get; } = new Guid("f9ac54e9-0623-4b75-aedf-42b3a00b4840");
        public static Guid Debtors { get; } = new Guid("53290171-f240-4c28-bd1d-e55dc00f8113");
        public static Guid NotesReceivable { get; } = new Guid("bf665aff-3b9b-47eb-9b14-ff6d58bf17dd");
        public static Guid Inventory { get; } = new Guid("6fecee1f-1ba1-4990-8e8f-218dca19d57a");
        public static Guid AccruedRevenues { get; } = new Guid("df7f42d2-f4fd-43d8-9299-848305d878fe");
        public static Guid PrepaidExpenses { get; } = new Guid("cc787d24-f41c-47a5-906b-a8acde27622f");

        public static Guid FixedAssets { get; } = new Guid("b0a6e582-c853-4f48-ab66-049322fb3378");
        public static Guid Lands { get; } = new Guid("6336eec0-40d5-440d-8983-ca00d12318ef");
        public static Guid Buildings { get; } = new Guid("3f336911-721d-4938-a428-1a55c14c5b3e");
        public static Guid Cars { get; } = new Guid("33e58076-2f4a-4d83-a625-6e831cec9236");
        public static Guid Furniture { get; } = new Guid("7501a73b-53c1-44b5-b295-6e32f608840a");
        public static Guid Computers { get; } = new Guid("f5e4d072-c059-4534-b2a2-3ea83fd41b3f");

        public static Guid OtherDebtBalance { get; } = new Guid("7a26a767-1b9c-43e6-a5b5-7033571c6e89");
        public static Guid PurchaseTax { get; } = new Guid("020eac0f-3a4c-4cda-b454-455f56182597");


        //2 Liabilities
        public static Guid Liabilities { get; } = new Guid("c1cc4991-fb3d-43e7-b25d-d99ff488af73");
        public static Guid CurrentLiabilities { get; } = new Guid("7e0fe300-b79c-44a8-bf5d-df8fa63715c9");
        public static Guid Creditors { get; } = new Guid("38f25355-b680-4981-bdfc-703e19bdc015");
        public static Guid NotesPayable  { get; } = new Guid("8d548b5e-a347-472b-827f-c69d85ab82e8");
        public static Guid ShortTermloans { get; } = new Guid("46287b40-3c72-4bb3-a84f-908235d7bb2a");
        public static Guid ReceivedRevenues { get; } = new Guid("221a95bc-a37d-4cc7-899d-a91e5e202b81");
        public static Guid AccruedExpenses { get; } = new Guid("2c4436f5-0779-4a98-b3c1-ab2fd0a03c05");

        public static Guid Equity { get; } = new Guid("2c6b9e5f-4aba-4367-bb56-f0c068a12762");
        public static Guid Capital { get; } = new Guid("ce76f9c1-41e4-44d2-bd2d-5acc4873a8c4");
        public static Guid Drawings { get; } = new Guid("7771922d-a802-4e63-b9cb-8b3d3f75b197");
        public static Guid Partners { get; } = new Guid("88c22211-18c9-4f2f-89e3-91109afa0c21");
        public static Guid ProfitLoss { get; } = new Guid("88c22211-18c9-4f2f-89e3-91109afa0c21");

        public static Guid OtherCreditBalance { get; } = new Guid("838be450-78cb-4d71-8e6d-e5fd165bac8a");
        public static Guid SalesTax { get; } = new Guid("72ec4613-8720-423a-bf48-5a0f7952aaa0");

        //3 Revenue
        public static Guid Revenue { get; } = new Guid("fb595169-4491-48e8-a29d-2f208aaf53ea");
        public static Guid SalesRevenues { get; } = new Guid("85d30aa5-f4c5-4a5f-9d28-66bd58dd58ee");
        public static Guid Sales { get; } = new Guid("b99e213a-4145-490f-8c6f-bb3784a590c1");
        public static Guid SalesReturns { get; } = new Guid("ef5a6cd7-c3bf-4f0a-ac44-f018720a32e0");
        public static Guid AcquiredDiscount { get; } = new Guid("ae93afcc-494c-475a-94d7-b1e81c751563");

        public static Guid OtherRevenues { get; } = new Guid("a2f9727d-4c9f-4199-b603-feb3ed06a04b");
        public static Guid InvestmentRevenues { get; } = new Guid("75e16e97-2cf0-4b4f-b56f-50949aa9cb10");
        public static Guid RentalRevenues { get; } = new Guid("8b8db047-d24c-4a64-a6a5-e453971ac292");

        //4 Expenses
        public static Guid Expenses { get; } = new Guid("2c6417c4-2812-4a27-8b46-2af7008d1269");
        public static Guid TradingExpenses { get; } = new Guid("3476463d-78b2-4a51-9bfe-0b2ef3a366c2");
        public static Guid CostofGoodsSold { get; } = new Guid("2c27a6c9-11fd-433c-bac7-6020608494b9");
        public static Guid DiscountPermitted { get; } = new Guid("a7462594-58b0-412b-af74-ff34525b8cdb");
        public static Guid TransportationExpenses { get; } = new Guid("1d750e8c-5a48-40dc-a830-04fff7044051");
        public static Guid ShippingExpenses { get; } = new Guid("e0b1c9ca-bbf3-4582-a277-3be97377e7e0");

        public static Guid UtilitiesExpenses { get; } = new Guid("a7462594-58b0-412b-af74-ff34525b8cdb");
        public static Guid Rent { get; } = new Guid("47dc962d-740a-47e2-8fe8-6993a4156e2c");
        public static Guid Electricity { get; } = new Guid("1162dd61-f303-4806-a366-34f860e08aa3");
        public static Guid MoblieInternet { get; } = new Guid("4eff7367-ffe8-4791-bf0b-04109370798d");
        public static Guid Water { get; } = new Guid("30cbd8fa-7f72-4ee2-a3ff-aaa80a10757a");

    }

    public static class ConstJournalTypes
    {
        //public static byte InvoicePurchases { get; } = 1;
        //public static byte InvoicePurchasesReturns { get; } = 2;
        //public static byte InvoiceSales { get; } = 3;
        //public static byte InvoiceSalesReturns { get; } = 4;
        public static byte Manual { get; } = 1;
        public static byte Invoice { get; } = 2;
        public static byte Receipt { get; } = 3;
        public static byte OpeningBalance { get; } = 4;
        public static byte AccountClose { get; } = 5;
        public static byte TransferredBalance { get; } = 6;


        public static List<SimpleByteM> GetList()
        {
            return new List<SimpleByteM>
            {
                //new SimpleM() {Id = InvoicePurchases, Name="Invoice Purchases"},
                //new SimpleM() {Id = InvoicePurchasesReturns, Name="Invoice Purchases Returns"},
                //new SimpleM() {Id = InvoiceSales, Name="Invoice Sales"},
                //new SimpleM() {Id = InvoiceSalesReturns, Name="Invoice Sales Returns"},
                 new SimpleByteM() {Id = 0, Name=Properties.Resources.All},
                new SimpleByteM() {Id = Manual, Name=Properties.Resources.Manual},
                 new SimpleByteM() {Id = Invoice, Name=Properties.Resources.Invoice},
                  new SimpleByteM() {Id = Receipt, Name=Properties.Resources.Receipt},
                 new SimpleByteM() {Id = OpeningBalance, Name=Properties.Resources.OpeningBalance},
                 new SimpleByteM() {Id = AccountClose, Name=Properties.Resources.AccountClose},
                 new SimpleByteM() {Id = TransferredBalance, Name=Properties.Resources.TransferredBalance},
            };
        }
    }

    public static class ConstInvoiceType
    {
        public static byte Purchases { get; } = 1;
        public static byte PurchasesReturns { get; } = 2;
        public static byte Sales { get; } = 3;
        public static byte SalesReturns { get; } = 4;

        public static List<SimpleByteM> GetList()
        {
            return new List<SimpleByteM>
            {
                new SimpleByteM() {Id = Purchases, Name=Properties.Resources.Purchases},
                new SimpleByteM() {Id = PurchasesReturns, Name=Properties.Resources.PurchasesReturns},
                new SimpleByteM() {Id = Sales, Name=Properties.Resources.Sales},
                new SimpleByteM() {Id = SalesReturns, Name=Properties.Resources.SalesReturns},
            };
        }

        public static List<SimpleByteM> GetListTitle()
        {
            return new List<SimpleByteM>
            {
                new SimpleByteM() {Id = Purchases, Name=Properties.Resources.PurchasesInvoices},
                new SimpleByteM() {Id = PurchasesReturns, Name=Properties.Resources.PurchasesReturnsInvoices},
                new SimpleByteM() {Id = Sales, Name=Properties.Resources.SalesInvoices},
                new SimpleByteM() {Id = SalesReturns, Name=Properties.Resources.SalesReturnsInvoices},
            };
        }
    }

    public static class ConstUserStatus
    {
        public static byte Active { get; } = 1;
        public static byte Disable { get; } = 2;
        public static byte Bloked { get; } = 3;

        public static List<SimpleByteM> GetList()
        {
            return new List<SimpleByteM>
            {
                new SimpleByteM() {Id = Active, Name=Properties.Resources.Active},
                new SimpleByteM() {Id = Disable, Name=Properties.Resources.Disable},
                new SimpleByteM() {Id = Bloked, Name=Properties.Resources.Bloked},
            };
        }
    }

    public static class ConstClientTypes
    {
        public static byte Supplier { get; } = 1;
        public static byte Customer { get; } = 2;
        public static byte Both { get; } = 3;

        public static List<SimpleByteM> GetList()
        {
            return new List<SimpleByteM>
            {
                new SimpleByteM() {Id = Supplier, Name=Properties.Resources.Supplier},
                new SimpleByteM() {Id = Customer, Name=Properties.Resources.Customer},
                new SimpleByteM() {Id = Both, Name=Properties.Resources.Both},
            };
        }
    }

    public static class ConstReceiptTypes
    {
        public static byte CatchReceipt { get; } = 1;
        public static byte PayReceipt { get; } = 2;
        public static byte Expenses { get; } = 3;
        public static byte Revenues { get; } = 4;
        public static byte Drawings { get; } = 5;

        public static List<SimpleByteM> GetList()
        {
            return new List<SimpleByteM>
            {
               new SimpleByteM() {Id = CatchReceipt, Name=Properties.Resources.CatchReceipt},
               new SimpleByteM() {Id = PayReceipt, Name=Properties.Resources.PayReceipt},
               new SimpleByteM() {Id = Expenses, Name=Properties.Resources.Expense},
               new SimpleByteM() {Id = Revenues, Name=Properties.Resources.Revenue},
                new SimpleByteM() {Id = Drawings, Name=Properties.Resources.Drawing},

            };
        }

        public static List<SimpleByteM> GetListReview()
        {
            return new List<SimpleByteM>
            {
               new SimpleByteM() {Id = CatchReceipt, Name=Properties.Resources.CatchReceipts},
               new SimpleByteM() {Id = PayReceipt, Name=Properties.Resources.PayReceipts},
               new SimpleByteM() {Id = Expenses, Name=Properties.Resources.Expenses},
               new SimpleByteM() {Id = Revenues, Name=Properties.Resources.Revenues},
               new SimpleByteM() {Id = Drawings, Name=Properties.Resources.Drawings},

            };
        }
    }

    public enum ModelSortOrder
    {
        Name,
        NameDesc,
        Date,
        DateDesc
    }

    public static class ConstSetting
    {
        public static int PageSize { get; } = 10;
    }

    public static class ConstPeriod
    {
        public static byte Today { get; } = 1;
        public static byte Week { get; } = 2;
        public static byte Month { get; } = 3;
    }

    public static class ConstLang
    {
        public static byte En { get; } = 1;
        public static byte Ar { get; } = 2;
    }
}
