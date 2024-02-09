using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for VisaPayWin.xaml
    /// </summary>
    public partial class VisaPayWin : Window
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        public DiscountVM model = new DiscountVM();
        private bool isAmount = true;

        public VisaPayWin(bool isAmount, decimal amount, decimal visa , Nullable<Guid> visaId)
        {
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            this.Owner = MyMainWindow.Instance;

            this.isAmount = isAmount;
            model.VisaId = visaId;

            this.db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
            model.Accounts = new AccountBL(db).GetVisas();
            if (model.Accounts.Count() > 0)
            {
                if(model.VisaId == null)
                model.VisaId = model.Accounts[0].Id;
            }
            else
                YesButton.IsEnabled = false;


            if (isAmount)
            {
                model.SenderAmount = amount;
                if (visa > 0)
                    model.Amount = visa;
                else
                    model.Amount = amount;
                txtAmount.Focus();
            }
            else
            {
                amountPanal.Visibility = Visibility.Collapsed;
                idPanal.Margin = new Thickness(0, 30, 0, 0);
            }

            DataContext = model;

        }

        private void txtDisount_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && model.VisaId != null)
                SetVisaPay();
        }

        private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetVisaPay();
        }

        private void NoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SetVisaPay()
        {
            BindingExpression amount = txtAmount.GetBindingExpression(TextBox.TextProperty);
            amount.UpdateSource();
            if (amount.HasError)
            {
                DialogResult = false;
            }
            else
            {
                if (model.Amount == 0 && isAmount)
                    model.VisaId = null;
                DialogResult = true;
            }
        }
    }
}
