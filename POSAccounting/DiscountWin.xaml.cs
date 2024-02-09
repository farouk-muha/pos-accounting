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
    /// Interaction logic for DiscountWin.xaml
    /// </summary>
    public partial class DiscountWin : Window
    {
        public DiscountVM model = new DiscountVM();

        public DiscountWin(decimal amount, decimal discount)
        {
            model.SenderAmount = amount;
            model.Amount = discount;
            model.Percent = MathFun.GetPercent(model.Amount, model.SenderAmount);

            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            txtPercent.Focus();
            this.DataContext = model;
            this.Owner = MyMainWindow.Instance;
        }

        private void txtDisount_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SetDiscount();
            else
            {
                if (sender == txtPercent)
                {
                    model.Amount = MathFun.GetAmount(model.SenderAmount, model.Percent);
                }
                else
                {
                    model.Percent = MathFun.GetPercent(model.Amount, model.SenderAmount);
                }
            }
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
            BindingExpression percent = txtPercent.GetBindingExpression(TextBox.TextProperty);
            percent.UpdateSource();
            if (amount.HasError || percent.HasError)
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
