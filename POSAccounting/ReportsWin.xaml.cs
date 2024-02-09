using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POSAccounting
{
    /// <summary>
    /// Interaction logic for ReportsWin.xaml
    /// </summary>
    public partial class ReportsWin : Window
    {
        private static ReportsWin _instance;
        private int reportType;
        private Nullable<Guid> id;
        private byte typeId;
        private string kw;
        private DateTime startDate;
        private DateTime endDate;
        private int pageSize;
        public static void ShowMe(int reportType, Nullable<Guid> id, byte typeId, string kw, 
            DateTime startDate, DateTime endDate, int pageSize)
        {
            if (_instance == null)
            {
                _instance = new ReportsWin(reportType, id, typeId, kw, startDate, endDate, pageSize);
                _instance.Closed += (senderr, args) => _instance = null;
                _instance.Show();
            }
        }

        public ReportsWin(int reportType, Nullable<Guid> id, byte typeId, string kw,
            DateTime startDate, DateTime endDate, int pageSize)
        {
            this.reportType = reportType;
            this.id = id;
            this.typeId = typeId;
            this.kw = kw;
            this.startDate = startDate;
            this.endDate = endDate;
            this.pageSize = pageSize;
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            //BtnPrint_Click();

        }


        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
        }

    }

    public class ReportTypes
    {
        public static int Journals { get; } = 1;
        public static int Invoices { get; } = 2;
        public static int Receipts { get; } = 3;
        public static int AccJournals { get; } = 4;
        public static int Visa { get; } = 5;
        public static int BoxSafe { get; } = 6;
        public static int ProfitLoss { get; } = 7;
        public static int TrialBlance { get; } = 8;
        public static int StatmentEquity { get; } = 9;
        public static int BalanceSheet { get; } = 10;


    }
}
