using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

namespace POSAccounting.Views
{
    /// <summary>
    /// Interaction logic for UserUC.xaml
    /// </summary>
    public partial class UserUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private UserVM model;
        private UserBL bl;
        private int id = 0;
        private bool isEdt = false;
        private ImgUtils imgutils = new ImgUtils();
        private bool isImgUpdate = false;


        public UserUC(int id = 0)
        {
            this.id = id;
            bl = new UserBL(db);
            model = new UserVM(db);

            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }


        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtPassword.BorderThickness = new Thickness(0);

            try
            {
                if (id > 0)
                {
                    await Task.Delay(20);

                    model.Model = bl.GetModelById(id);

                    if (model.Model == null)
                        throw new Exception();
                    isEdt = true;
                    //chek local img is null set default or get img
                    if (!String.IsNullOrEmpty(model.Model.LocalImg))
                    {
                        var path = imgutils.GetImgFullPath(model.Model.LocalImg, imgutils.UserImgs);
                        if (Directory.Exists(path))
                            model.Model.DisplayImg = imgutils.GetImgFullPath(model.Model.LocalImg, imgutils.UserImgs);

                    }
                    txtPassword.IsEnabled = false;
                    if(model.Model.Id == Profile.UserProfile.Id)
                    {
                        cbRole.IsEnabled = false;
                        cbStauts.IsEnabled = false;
                    }
                }
                else
                    model.Set(db);

            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(model.Model.DisplayImg))
                model.Model.DisplayImg = ImgUtils.UserIcon;

            this.DataContext = model;
        }

        private void img_MouseDown(object sender, MouseEventArgs e)
        {
            var re = imgutils.ImgDialog();
            if (re == null)
                return;

            model.Model.DisplayImg = re.FileName;
            isImgUpdate = true;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            //chek fields
            BindingExpression name = txtUserName.GetBindingExpression(TextBox.TextProperty);
            name.UpdateSource();
            BindingExpression email = txtEmail.GetBindingExpression(TextBox.TextProperty);
            email.UpdateSource();
            BindingExpression firstName = txtFirstName.GetBindingExpression(TextBox.TextProperty);
            firstName.UpdateSource();
            BindingExpression lastName = txtLastName.GetBindingExpression(TextBox.TextProperty);
            lastName.UpdateSource();
            BindingExpression Phone = txtPhone.GetBindingExpression(TextBox.TextProperty);
            Phone.UpdateSource();
            BindingExpression address = txtAddress.GetBindingExpression(TextBox.TextProperty);
            address.UpdateSource();

            if (name.HasError || email.HasError || firstName.HasError || lastName.HasError
                 || Phone.HasError || address.HasError)
            {
                if (!isEdt && string.IsNullOrEmpty(model.Model.Password))
                {
                    txtPassword.BorderThickness = new Thickness(1, 1, 1, 1);
                    txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                saveBtn.IsEnabled = true;
                return;
            }

            if (!isEdt && string.IsNullOrEmpty(model.Model.Password))
            {
                txtPassword.BorderThickness = new Thickness(1);
                txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                saveBtn.IsEnabled = true;
                return;
            }
            //////////////////////////////////////////////////////


            String msg = ConstUserMsg.FaildProcess;
            try
            {
                //chek name and email
                Expression<Func<SecUser, bool>> filter;
                if (isEdt)
                    filter = m => (m.UserName.Equals(model.Model.UserName) || m.Email.Equals(model.Model.Email)) && m.Id != model.Model.Id;
                else
                    filter = m => (m.UserName.Equals(model.Model.UserName) || m.Email.Equals(model.Model.Email));

                var v = bl.Get(filter).FirstOrDefault();
                if (v != null && v.Id != model.Model.Id)
                {
                    if (v.UserName.Equals(model.Model.UserName))
                        msg = ConstUserMsg.NameExist;
                    else
                        msg = ConstUserMsg.EmailExist;

                    throw new Exception();
                }
                ////////////////////////////////////


                model.Model.RoleId = 1;
                model.Model.StatusId = 1;

                //save image and delete old one
                if (isImgUpdate)
                    model.Model.LocalImg = imgutils.SaveImg(model.Model.DisplayImg, imgutils.UserImgs,
                        model.Model.LocalImg);

                if (isEdt)
                    bl.Update(model.Model);
                else
                    bl.Insert(model.Model);

                MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(msg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            saveBtn.IsEnabled = true;
        }

        private void dontSave()
        {

        }
        private void passwordTxt_Changed(object sender, RoutedEventArgs e)
        {
            model.Model.Password = txtPassword.Password;
            if (!string.IsNullOrEmpty(model.Model.Password))
            {
                txtPassword.BorderThickness = new Thickness(0);
                txtPassword.BorderBrush = new SolidColorBrush(Colors.Black);
            }
        }

    }
}
