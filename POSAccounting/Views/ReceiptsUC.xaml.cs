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
    /// Interaction logic for Receipts4.xaml
    /// </summary>
    public partial class ReceiptsUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private ReceiptsVM model;
        private ReceiptBL bl;

        public ReceiptsUC(byte type, Nullable<Guid> accId)
        {
            db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
            bl = new ReceiptBL(db);
            model = new ReceiptsVM(db, bl);
            model.TypeId = type;
            model.Id = accId;
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var template = headerControl.Template;
            var txtKW = (TextBox)template.FindName("txtKW", headerControl);
            txtKW.SetValue(TextBoxHelper.WatermarkProperty, Properties.Resources.ReceiptSearchHint);
            var chk = (CheckBox)template.FindName("chkStatus", headerControl);
            chk.Visibility = Visibility.Collapsed;
            var typesCB = (ComboBox)template.FindName("typesCB", headerControl);
            typesCB.Visibility = Visibility.Collapsed;

            chk.Content = Properties.Resources.Status;
            ((TextBlock)template.FindName("blkFrom", headerControl)).Text = Properties.Resources.From;
            ((TextBlock)template.FindName("blkTo", headerControl)).Text = Properties.Resources.To;
            ((Button)template.FindName("btnSearch", headerControl)).Content = Properties.Resources.Search;
            ((Button)template.FindName("btnClear", headerControl)).Content = Properties.Resources.Clear;
            ((TextBlock)template.FindName("blkRecoreds", headerControl)).Text = Properties.Resources.Records;

            var card = cardCount.Template;
            ((TextBlock)card.FindName("blkTotal", cardCount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardCount)).Text = Properties.Resources.Receipt;
            card = cardAmount.Template;
            ((TextBlock)card.FindName("blkTotal", cardAmount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardAmount)).Text = Properties.Resources.Amount;
            await Task.Delay(20);

            model.Load();
            this.DataContext = model;

            if (model.Id != null)
            {
                var ac = new AccountBL(db).GetById((Guid)model.Id);
                string s = Properties.Resources.TheClient;
                if (ac != null)
                    s += ": " + ac.EnName;
                blkAccName.Text = s;
                grid.RowDefinitions[1].Height = new GridLength(40);
            }
        }

        private void NumMouseLeftBtnUpHandler(object sender, MouseEventArgs e)
        {
            if (dg.SelectedItem == null)
                return;

            if (!(dg.SelectedItem.GetType() == typeof(ReceiptM)))
                return;

            ReceiptM m = (ReceiptM)dg.SelectedItem;
            if (m == null)
                return;

            ReceiptPrintWin.ShowMe(m.Id);
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
