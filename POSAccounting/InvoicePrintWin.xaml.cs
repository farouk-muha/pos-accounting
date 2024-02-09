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
    /// Interaction logic for InvoicePrintWin.xaml
    /// </summary>
    public partial class InvoicePrintWin : Window
    {
            
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private InvoicePrintVM model;
        private static InvoicePrintWin _instance;
        private Guid id;

        public static InvoicePrintWin ShowMe(Guid id)
        {
            if (_instance == null)
            {
                _instance = new InvoicePrintWin(id);
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public InvoicePrintWin(Guid id)
        {
            this.id = id;
            try
            {
                model = new InvoicePrintVM(db, id);
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
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            MyPrint.ShowPrintPreview(panel);   
        }
    }
}
