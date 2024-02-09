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
    public partial class AccountsUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private AccountsVM model;
        private AccountBL bl;
        public AccountsUC()
        {
            bl = new AccountBL(db);
            model = new AccountsVM(db, bl);
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }
        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
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

            model.Load();
            this.DataContext = model;
        }

        public void Refreshitem_Click(object sender, RoutedEventArgs e)
        {
            model.Refresh();
        }

        public void AddItem_Click(object sender, RoutedEventArgs e)
        {
            model.Add();
        }

        public void EdtItem_Click(object sender, RoutedEventArgs e)
        {
            object id = ((MenuItem)sender).CommandParameter;
            model.Update(id);
        }

        public void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            object id = ((MenuItem)sender).CommandParameter;
            model.Delete(id);
        }
    }
}
