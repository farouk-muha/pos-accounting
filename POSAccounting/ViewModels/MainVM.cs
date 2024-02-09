using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CorpAccountingApp.ViewModels
{
    public class MainVM : ViewModelBase
    {
        public CropAccountingAppEntities db;
        private SeriesCollection invoicesToday;
        private SeriesCollection invoicesWeek;
        private SeriesCollection invoicesMonth;
        private SeriesCollection accountsToday;
        private SeriesCollection accountsWeek;
        private SeriesCollection accountsMonth;
        private string invoicesCount;
        private string receiptsCount;
        private int invoicesDue;
        private int billsDue;
        public SeriesCollection InvSeries;
        public Func<double, string> Formatter { get; set; }
        public DateTime StartDate = DateTime.Now.AddMonths(-1);


        public SeriesCollection InvoicesToday { get { return invoicesToday; } set { invoicesToday = value; NotifyPropertyChanged("InvoicesToday"); } }
        public SeriesCollection InvoicesWeek { get { return invoicesWeek; } set { invoicesWeek = value; NotifyPropertyChanged("InvoicesWeek"); } }
        public SeriesCollection InvoicesMonth { get { return invoicesMonth; } set { invoicesMonth = value; NotifyPropertyChanged("InvoicesMonth"); } }
        public SeriesCollection AccountsToday { get { return accountsToday; } set { accountsToday = value; NotifyPropertyChanged("AccountsToday"); } }
        public SeriesCollection AccountsWeek { get { return accountsWeek; } set { accountsWeek = value; NotifyPropertyChanged("AccountsWeek"); } }
        public SeriesCollection AccountsMonth { get { return accountsMonth; } set { accountsMonth = value; NotifyPropertyChanged("AccountsMonth"); } }
        public string InvoicesCount { get { return invoicesCount; } set { invoicesCount = value; NotifyPropertyChanged("InvoicesCount"); } }
        public string ReceiptsCount { get { return receiptsCount; } set { receiptsCount = value; NotifyPropertyChanged("ReceiptsCount"); } }
        public int InvoicesDue { get { return invoicesDue; } set { invoicesDue = value; NotifyPropertyChanged("InvoicesDue"); } }
        public int BillsDue { get { return billsDue; } set { billsDue = value; NotifyPropertyChanged("BillsDue"); } }



        public MainVM(CropAccountingAppEntities db)
        {
            this.db = db;  
        }
        public void Load()
        {
            var now = DateTime.Now;

            InvoiceBL invBL = new InvoiceBL(db);
            JournalDetBL jBL = new JournalDetBL(db);
            ReceiptBL reBL = new ReceiptBL(db);

            //invoices and receipts
            //var inv = invBL.GetChart();

            //InvoicesToday = inv[0];
            //InvoicesWeek = inv[1];
            //InvoicesMonth = inv[2];
            InvSeries = invBL.GetSeries(StartDate);

            //income and expencese
            var income = jBL.GetChart();
            AccountsToday = income[0];
            AccountsWeek = income[1];
            AccountsMonth = income[2];

            InvoicesCount = invBL.GetCount(m => m.Date.Year == now.Year).ToString();
            ReceiptsCount = reBL.GetCount(m => m.Date.Year == now.Year).ToString();

            var date = DateTime.Now.AddDays(-AppSettings.GetInvoiceReminder());
            InvoicesDue = invBL.GetCount(m => m.IsRemind && m.Date <= date && (m.InvoiceTypeId == ConstInvoiceType.Sales
            || m.InvoiceTypeId == ConstInvoiceType.PurchasesReturns));
            BillsDue = invBL.GetCount(m => m.IsRemind && m.Date <= date && (m.InvoiceTypeId == ConstInvoiceType.Purchases
            || m.InvoiceTypeId == ConstInvoiceType.SalesReturns));
        }
    }

}