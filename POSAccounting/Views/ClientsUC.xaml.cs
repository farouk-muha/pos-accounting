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
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class ClientsUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private ClientsVM model;

        public ClientsUC(byte typeId)
        {
            model = new ClientsVM(db, new ClientBL(db), typeId);
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var template = headerControl.Template;
            var txtKW = (TextBox)template.FindName("txtKW", headerControl);
            txtKW.SetValue(TextBoxHelper.WatermarkProperty, Properties.Resources.ClientNumName);
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
            ((TextBlock)card.FindName("blkT", cardCount)).Text = Properties.Resources.Clients;
            card = cardAmount.Template;
            ((TextBlock)card.FindName("blkTotal", cardAmount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardAmount)).Text = Properties.Resources.Balance;
            card = cardCredit.Template;
            ((TextBlock)card.FindName("blkTotal", cardCredit)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardCredit)).Text = Properties.Resources.Credit;
            card = cardDebt.Template;
            ((TextBlock)card.FindName("blkTotal", cardDebt)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardDebt)).Text = Properties.Resources.Debt;

            await Task.Delay(20);

            model.Load();
            this.DataContext = model;
        }

        private void NumMouseLeftBtnUpHandler(object sender, MouseEventArgs e)
        {
            if (dg.SelectedItem == null)
                return;

            if (!(dg.SelectedItem.GetType() == typeof(ClientM)))
                return;

            ClientM m = (ClientM)dg.SelectedItem;
            if (m == null)
                return;

            MyMainWindow.Instance.Client(m.Id, true);
        }
        private void NumMouseEnter_OnHandler(object sender, MouseEventArgs e)
        {
            dg.Cursor = Cursors.Hand;
        }

        private void NumMouseLeave_OnHandler(object sender, MouseEventArgs e)
        {
            dg.Cursor = null;
        }

        private void InvMouseLeftBtnUpHandler(object sender, MouseEventArgs e)
        {
            if (dg.SelectedItem == null)
                return;

            if (!(dg.SelectedItem.GetType() == typeof(ClientM)))
                return;

            ClientM m = (ClientM)dg.SelectedItem;
            if (m == null)
                return;

            MyMainWindow.Instance.Invoices(0, m.AccountId);
        }

        private void RecMouseLeftBtnUpHandler(object sender, MouseEventArgs e)
        {
            if (dg.SelectedItem == null)
                return;

            if (!(dg.SelectedItem.GetType() == typeof(ClientM)))
                return;

            ClientM m = (ClientM)dg.SelectedItem;
            if (m == null)
                return;

            MyMainWindow.Instance.Receitpts(0, m.AccountId);
        }
    }
}
