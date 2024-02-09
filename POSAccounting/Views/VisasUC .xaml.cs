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
    /// Interaction logic for AccountsUC.xaml
    /// </summary>
    public partial class VisasUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private AccountsVM model;
        private AccountBL bl;
        public VisasUC()
        {
            bl = new AccountBL(db);
            model = new AccountsVM(db, bl);
            model.TypeId = AccountType.Visa;
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            model.WinName = Properties.Resources.Visas;
            model.DisplayImg = ImgUtils.VisaIcon;
            this.DataContext = model;

            var card = cardCount.Template;
            ((TextBlock)card.FindName("blkTotal", cardCount)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardCount)).Text = Properties.Resources.Account;
            card = cardCredit.Template;
            ((TextBlock)card.FindName("blkTotal", cardCredit)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardCredit)).Text = Properties.Resources.Credit;
            card = cardDebt.Template;
            ((TextBlock)card.FindName("blkTotal", cardDebt)).Text = Properties.Resources.Total;
            ((TextBlock)card.FindName("blkT", cardDebt)).Text = Properties.Resources.Debt;
            await Task.Delay(20);

            refresh();
        }

        private void refresh()
        {
            try
            {
                var v = bl.GetTreeVisas();
                model.Accounts = v.Models;
                model.Credit = v.Credit;
                model.Debt = v.Debt;
                model.Count = v.Count;
                model.Amount = v.Amount;
                model.CountDisplay = model.Count.ToString();
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Refreshitem_Click(object sender, RoutedEventArgs e)
        {
            model.Refresh();
        }

        public void AddItem_Click(object sender, RoutedEventArgs e)
        {
            object ob = ((MenuItem)sender).CommandParameter;

            if (ob != null || ob.GetType() != typeof(Guid))
            {
                Guid id = (Guid)ob;

                foreach (var v in model.Accounts)
                {
                    if (v.Id == id)
                    {
                        CorpBankWin.ShowMe();
                        return;
                    }
                    foreach(var vv in v.Accounts)
                    {
                        if (vv.Id == id)
                        {
                            VisaWin.ShowMe();
                            return;
                        }
                    }
                }
            }
        }

        public void EdtItem_Click(object sender, RoutedEventArgs e)
        {
            object ob = ((MenuItem)sender).CommandParameter;
            if (ob != null || ob.GetType() != typeof(Guid))
            {
                Guid id = (Guid)ob;
                foreach (var v in model.Accounts)
                {
                    if (v.Id == id)
                    {
                        CorpBankWin.ShowMe(id);
                        return;
                    }
                    foreach (var vv in v.Accounts)
                    {
                        if (vv.Id == id)
                        {
                            VisaWin.ShowMe(id);
                            return;
                        }
                    }
                }
            }
        }

        public void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            object ob = ((MenuItem)sender).CommandParameter;
            if (ob != null || ob.GetType() != typeof(Guid))
            {

                Guid id = (Guid)ob;
                AccountM acc = new AccountM();
                foreach (var v in model.Accounts)
                {
                    if (v.Id == id)
                    {
                        acc = v;
                        goto found;
                    }
                    foreach (var vv in v.Accounts)
                    {
                        if (vv.Id == id)
                        {
                            acc = v;
                            goto found;
                        }
                    }
                }

                found:
                {
                    if (acc == null)
                        return;
                }

                var res = MessageBox.Show(string.Format(ConstUserMsg.DeleteConfirmMsg, Properties.Resources.Visa
        + " \"" + acc.EnName + "\"")
        + "\n \n " + Properties.Resources.AccDelNotAllowed,
       ConstUserMsg.DeleteConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);

                if (res == MessageBoxResult.Cancel)
                    return;

                try
                {
                    model.DelAcc(acc);
                    model.Accounts.Remove(acc);
                    treeView.Items.Refresh();
                    model.Count--;

                    MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show(ConstUserMsg.FaildMsg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
        }

        private void VisaBtn_Click(object sender, RoutedEventArgs e)
        {
          VisaWin.ShowMe();
        }

        private void BankBtn_Click(object sender, RoutedEventArgs e)
        {
          CorpBankWin.ShowMe();
        }
    }
}
