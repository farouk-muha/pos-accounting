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
using System.Windows.Shapes;

namespace POSAccounting
{
    /// <summary>
    /// Interaction logic for ReceiptPrintWin.xaml
    /// </summary>
    public partial class ReceiptPrintWin : Window
    {
            
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private ReceiptPrintVM model;
        private static ReceiptPrintWin _instance;
        private Guid id;

        public static ReceiptPrintWin ShowMe(Guid id)
        {
            if (_instance == null)
            {
                _instance = new ReceiptPrintWin(id);
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public ReceiptPrintWin(Guid id)
        {
            this.id = id;
            try
            {
                model = new ReceiptPrintVM(db, id);
            }
            catch
            {

            }
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = model;
            if (model.Receipt.IsCash)
            {
                txtVisa.Visibility = Visibility.Collapsed;
                txtBank.Visibility = Visibility.Collapsed;
            }
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            MyPrint.ShowPrintPreview(panel);   
        }
    }
}
