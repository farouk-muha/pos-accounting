using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSAccounting.Views
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class ReportsUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());

        public ReportsUC()
        {
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void borderJournals_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.Journals);
        }

        private void borderInvoices_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.Invoices);
        }

        private void borderReceipts_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.Receipts);
        }

        private void brVisa_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.Visa);
        }

        private void vrBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.BoxSafe);
        }
        private void brAcc_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.AccJournals);
        }
        
        private void ShowReport(int reportType, Nullable<Guid> id = null, Nullable<byte> typeId = null)
        {
            ReportOptionWin.ShowMe(reportType, id, typeId);            
        }

        private void borderProfitLoss_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.ProfitLoss);
        }

        private void TrialBlance_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.TrialBlance);
        }
        private void StatmentEquity_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.StatmentEquity);
        }

        private void BalanceSheet_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ShowReport(ReportTypes.BalanceSheet);
        }

        //private async void ShowReport(int reportType, Nullable<Guid> id = null, Nullable<byte> typeId = null, string kw = null,
        //  Nullable<DateTime> startDate = null, Nullable<DateTime> endDate = null, int pageSize = 20)
        //{
        //    Cursor = Cursors.Wait;
        //    rec.Visibility = Visibility.Visible;
        //    await Task.Delay(20);
        //    if (startDate == null)
        //        startDate = DateTime.Now.AddDays(-6);
        //    if (endDate == null)
        //        endDate = DateTime.Now;

        //    ReportsWin.ShowMe(reportType, null, null, null, (DateTime)startDate, (DateTime)endDate, 20);
        //    Cursor = null;
        //    rec.Visibility = Visibility.Collapsed;

        //}
        private void BrMouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void BrMouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
