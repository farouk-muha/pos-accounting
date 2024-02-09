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
    public partial class InvoicesUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private InvoicesVM model;
        private InvoiceBL bl;
        private bool? isaccClientDeb;

        public InvoicesUC(byte type, Nullable<Guid> accId, bool? isaccClientDeb)
        {
            db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
            bl = new InvoiceBL(db);
            model = new InvoicesVM(db, bl, isaccClientDeb);

            model.TypeId = type;
            model.Id = accId;
            this.isaccClientDeb = isaccClientDeb;

            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var template = headerControl.Template;
            var txtKW = (TextBox)template.FindName("txtKW", headerControl);
            txtKW.SetValue(TextBoxHelper.WatermarkProperty, Properties.Resources.InvoiceSearchHint);
            var typesCB = (ComboBox)template.FindName("typesCB", headerControl);
            typesCB.Visibility = Visibility.Collapsed;

            var chkbox = ((CheckBox)template.FindName("chkStatus", headerControl));
            chkbox.Content = Properties.Resources.Due;
            ((TextBlock)template.FindName("blkFrom", headerControl)).Text = Properties.Resources.From;
            ((TextBlock)template.FindName("blkTo", headerControl)).Text = Properties.Resources.To;
            ((Button)template.FindName("btnSearch", headerControl)).Content = Properties.Resources.Search;
            ((Button)template.FindName("btnClear", headerControl)).Content = Properties.Resources.Clear;
            ((TextBlock)template.FindName("blkRecoreds", headerControl)).Text = Properties.Resources.Records;

            var card = cardCount.Template;
            ((TextBlock)card.FindName("blkTotal", cardCount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardCount)).Text = Properties.Resources.Invoice;
            card = cardAmount.Template;
            ((TextBlock)card.FindName("blkTotal", cardAmount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardAmount)).Text = Properties.Resources.Amount;
            card = cardCredit.Template;
            ((TextBlock)card.FindName("blkTotal", cardCredit)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardCredit)).Text = Properties.Resources.Paid;
            card = cardDebt.Template;
            ((TextBlock)card.FindName("blkTotal", cardDebt)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardDebt)).Text = Properties.Resources.Due;

            await Task.Delay(20);

            if (isaccClientDeb != null)
            {
                model.Status = true;
                chkbox.IsEnabled = false;

            }

            model.Load();
            this.DataContext = model;


            try
            {
                if (model.Id != null)
                {
                    var ac = new AccountBL(db).GetById((Guid)model.Id);
                    string s = Properties.Resources.TheClient;
                    if (ac != null)
                        s += ": " + ac.EnName;
                    blkAccName.Text = s;
                    grid.RowDefinitions[2].Height = new GridLength(40);

                }
            }
            catch
            {

            }
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
