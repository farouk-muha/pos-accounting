using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
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
    /// Interaction logic for CashPayWin.xaml
    /// </summary>
    public partial class CashPayWin : Window
    {
        public DiscountVM model = new DiscountVM();

        public CashPayWin(decimal amount, decimal cash)
        {

            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            this.Owner = MyMainWindow.Instance;

            model.SenderAmount = amount;
            if (cash > 0)
                model.Amount = cash;
            else
                model.Amount = amount;
            txtAmount.Focus();
            this.DataContext = model;
        }

        private void txtDisount_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SetDiscount();
        }

        private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetDiscount();
        }

        private void NoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SetDiscount()
        {
            BindingExpression amount = txtAmount.GetBindingExpression(TextBox.TextProperty);
            amount.UpdateSource();
            if (amount.HasError)
            {
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
            }

        }
    }
}
