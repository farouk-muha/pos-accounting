using CorpAccountingApp.ViewModels;
using POSAccounting.Models;
using POSAccounting.Utils;
using LiveCharts;
using LiveCharts.Defaults;
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
    /// Interaction logic for MainUC.xaml
    /// </summary>
    public partial class MainUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        public MainVM model;
        public MainUC()
        {
            model = new MainVM(db);
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            model.Load();
            this.DataContext = model;
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

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
