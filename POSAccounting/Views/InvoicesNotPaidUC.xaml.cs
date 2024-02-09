using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using MahApps.Metro.Controls;
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
    /// Interaction logic for Invoices4.xaml
    /// </summary>
    public partial class InvoicesNotPaidUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private InvoicesVM model;
        private InvoiceBL bl;
        private bool IsCleintDebt;
        private Nullable<Guid> accId;

        public InvoicesNotPaidUC(bool IsCleintDebt, Nullable<Guid> accId)
        {
            this.IsCleintDebt = IsCleintDebt;
            this.accId = accId;

            db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
            bl = new InvoiceBL(db);
            model = new InvoicesVM(db, bl, null);
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
             model.Models = bl.GetNotPayed(new JournalBL(db), accId, IsCleintDebt, true);
            }
            catch
            {

            }
            this.DataContext = model;
        }

        private void NumMouseLeftBtnUpHandler(object sender, MouseEventArgs e)
        {
            if (dg.SelectedItem == null)
                return;

            if (!(dg.SelectedItem.GetType() == typeof(InvoiceM)))
                return;

            InvoiceM m = (InvoiceM)dg.SelectedItem;
            if (m == null)
                return;

            InvoicePrintWin.ShowMe(m.Id);
        }
        private void NumMouseEnter_OnHandler(object sender, MouseEventArgs e)
        {
            dg.Cursor = Cursors.Hand;
        }

        private void NumMouseLeave_OnHandler(object sender, MouseEventArgs e)
        {
            dg.Cursor = null;
        }
    }
}
