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
    public partial class PassRestWin : MetroWindow
    {
        private ResetPasswordM model;
        private static PassRestWin _instance;

        public static PassRestWin ShowMe(int id)
        {
            if (_instance == null)
            {
                _instance = new PassRestWin(id);
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public PassRestWin(int id)
        {
            model = new ResetPasswordM();
            model.Id = id;
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = model;
            txtPass.Focus();
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) => Login();

        private void Login()
        {
            //check inputs 
            btnSave.IsEnabled = false;
            model.Password = txtPass.Password;
            model.NewPassword = txtNewPass.Password;


            if (string.IsNullOrEmpty(model.Password))
            {
                model.PasswordError = ConstUserMsg.RequiredField;
                btnSave.IsEnabled = true;
                return;
            }

            if (string.IsNullOrEmpty(model.NewPassword))
            {
                model.NewPasswordError = ConstUserMsg.RequiredField;
                btnSave.IsEnabled = true;
                return;
            }

            string msg = ConstUserMsg.FaildProcess;
            try
            {
                var b =new UserBL(new CropAccountingAppEntities()).ResetPass(model);
                if (!b)
                {
                    msg = ConstUserMsg.WrongPassword;
                    throw new Exception();
                }

                MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(msg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            btnSave.IsEnabled = true;
            Close();
        }


        private void txtPAss_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (model == null)
                return;

            model.Password = txtPass.Password;
            if (!string.IsNullOrEmpty(model.Password))
            {
                model.PasswordError = "";
                model.ErrorMsg = "";
            }
        }

        private void txtNewPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (model == null)
                return;

            model.NewPassword = txtNewPass.Password;
            if (!string.IsNullOrEmpty(model.Password))
            {
                model.NewPassword = "";
                model.ErrorMsg = "";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
