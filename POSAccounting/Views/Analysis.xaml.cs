using CorpAccountingApp.ViewModels;
using POSAccounting.Models;
using POSAccounting.Utils;
using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
    /// Interaction logic for MainUC.xaml
    /// </summary>
    public partial class Analysis : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        public MainVM model;
        public Analysis()
        {
            model = new MainVM(db);
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }

        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {

            await Task.Delay(100);

           
            try
            {

                model.Load();

                this.DataContext = model;
                invChart.Series = model.InvSeries;


                Formatter.LabelFormatter = value => new DateTime((long)value).ToString("MM:dd HH:mm");
                Formatter.MinValue = DateTime.Now.AddDays(-30).Ticks;

                double inc = 0, ex = 0;

                // today accounts
                for (int c = 0; c < model.AccountsToday.Count(); c++)
                {
                    if (c > 0)
                        ex = ((ChartValues<ObservableValue>)model.AccountsToday[c].Values)[0].Value;

                    else
                        inc = ((ChartValues<ObservableValue>)model.AccountsToday[c].Values)[0].Value;

                    txtTodEx.Text = ex.ToString();
                    txtTodIn.Text = inc.ToString();

                    if (inc > 0 || ex > 0)
                        TodayTxt.Visibility = Visibility.Collapsed;
                }

                // Week accounts
                inc = 0; ex = 0;
                for (int c = 0; c < model.AccountsWeek.Count(); c++)
                {
                    if (c > 0)
                        ex = ((ChartValues<ObservableValue>)model.AccountsWeek[c].Values)[0].Value;

                    else
                        inc = ((ChartValues<ObservableValue>)model.AccountsWeek[c].Values)[0].Value;

                    txtWeekEx.Text = ex.ToString();
                    txtWeekIn.Text = inc.ToString();

                    if (inc > 0 || ex > 0)
                        WeekTxt.Visibility = Visibility.Collapsed;
                }

                // Month accounts
                inc = 0; ex = 0;
                for (int c = 0; c < model.AccountsMonth.Count(); c++)
                {
                    if (c > 0)
                        ex = ((ChartValues<ObservableValue>)model.AccountsMonth[c].Values)[0].Value;

                    else
                        inc = ((ChartValues<ObservableValue>)model.AccountsMonth[c].Values)[0].Value;

                    txtMoEx.Text = ex.ToString();
                    txtMoIn.Text = inc.ToString();

                    if (inc > 0 || ex > 0)
                        MonthTxt.Visibility = Visibility.Collapsed;
                }
                
                //// Today Invs
                //double i = 0;
                //foreach (var t in model.InvoicesToday)
                //{
                //    if (t.Values.Count > 0)
                //    {
                //        i += ((ChartValues<ObservableValue>)t.Values)[0].Value;
                //    }
                //}

                //if (i > 0)
                //    InvTodayTxt.Visibility = Visibility.Collapsed;

                //// Week Invs
                //i = 0;
                //foreach (var t in model.InvoicesWeek)
                //{
                //    if (t.Values.Count > 0)
                //    {
                //        i += ((ChartValues<ObservableValue>)t.Values)[0].Value;
                //    }
                //}

                //if (i > 0)
                //    InvWeekTxt.Visibility = Visibility.Collapsed;

                //// Month Invs
                //i = 0;
                //foreach (var t in model.InvoicesMonth)
                //{
                //    if (t.Values.Count > 0)
                //    {
                //        i += ((ChartValues<ObservableValue>)t.Values)[0].Value;
                //    }
                //}

                //if (i > 0)
                //    InvMonthTxt.Visibility = Visibility.Collapsed;
            }
            catch
            {

            }
        }

        private void GridMouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void GridMouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void GridSale_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Invoice(ConstInvoiceType.Sales);
        }

        private void GridPur_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Invoice(ConstInvoiceType.Purchases);
        }

        private void GridReSale_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Invoice(ConstInvoiceType.SalesReturns);
        }

        private void GridRePur_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Invoice(ConstInvoiceType.PurchasesReturns);
        }

        private void GridCash_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Receipt(ConstReceiptTypes.CatchReceipt);
        }

        private void GridPay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Receipt(ConstReceiptTypes.PayReceipt);
        }

        private void GridIn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Receipt(ConstReceiptTypes.Revenues);
        }

        private void GridEx_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Receipt(ConstReceiptTypes.Expenses);
        }
        private void InvCredit_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Invoices(0, null, true);
        }

        private void InvDebt_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MyMainWindow.Instance.Invoices(0, null, false);
        }

    }
}
