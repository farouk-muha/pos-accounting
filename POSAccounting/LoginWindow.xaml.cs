using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using POSAccounting.Models;
using POSAccounting.BL;
using POSAccounting.Server;
using POSAccounting.Utils;
using System.Threading.Tasks;

namespace POSAccounting
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private LoginM model;

        public LoginWindow()
        {
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            model = new LoginM();
            this.DataContext = model;
            txtName.Focus();
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e) => Login();

        private void Login()
        {
            //check inputs 
            loginBtn.IsEnabled = false;
            model.Password = passwordTxt.Password;

            BindingExpression name = txtName.GetBindingExpression(TextBox.TextProperty);
            name.UpdateSource();

            if (name.HasError || string.IsNullOrEmpty(model.Password))
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    model.PasswordError = ConstUserMsg.RequiredField;
                }

                loginBtn.IsEnabled = true;
                return;
            }


            // check if user login locally before
            CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
            UserBL userBL = new UserBL(db);
            UserM user = userBL.Get(model.UserName, model.Password);
            if(user == null)
            {
                model.ErrorMsg = ConstUserMsg.WrongEmailOrPass;
                return;
            }

            if (user.LocalStatusId != ConstUserStatus.Active)
            {
                model.ErrorMsg = string.Format(ConstUserMsg.StatusNotActive, model.UserName);
                loginBtn.IsEnabled = true;
                return;
            }

            Profile.UserProfile = user;
            Profile.CorpProfile = new CorpInfoBL(db).GetById(1);

            #region
            //initialize the splash screen and set it as the application main window
            var splashScreen = SplashWin.NewInstance(true);
            Application.Current.MainWindow = splashScreen;
            Close();
            splashScreen.Show();

            #endregion
        }


        private void passwordTxt_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (model == null)
                return;

            model.Password = passwordTxt.Password;
            if (!string.IsNullOrEmpty(model.Password))
            {
                model.PasswordError = "";
                model.ErrorMsg = "";
            }
        }

        private void extTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => this.Close();

        private void signUpBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Forget_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
