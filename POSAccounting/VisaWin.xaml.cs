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
    /// Interaction logic for UserWin.xaml
    /// </summary>
    public partial class VisaWin : MetroWindow
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private AccountVM model;
        private AccountBL bl;
        private static VisaWin _instance;
        private Nullable<Guid> id;
        private bool isEdt = false;
        private Nullable<Guid> oldParentId;
        private int oldNum;

        // Title="{x:Static p:Resources}"
        // xmlns:p = "clr-namespace:POSAccounting.Properties"
        public static VisaWin ShowMe(Nullable<Guid> id = null)
        {
            if (_instance == null)
            {
                _instance = new VisaWin(id);
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public VisaWin(Nullable<Guid> id = null)
        {
            this.id = id;
            bl = new AccountBL(db);
            model = new AccountVM(db, bl);

            InitializeComponent();
            Loaded += MyWindow_Loaded;
        }


        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;
            try
            {
                model.Accounts = bl.GetModels(m => m.AccountParentId == ConstAccounts.Bank);
                if (model.Accounts.Count() == 0)
                {
                    txtError.Visibility = Visibility.Visible;
                    saveBtn.IsEnabled = false;
                    return;
                }

                if (id == null)
                {
                    SetNew();
                }
                else
                {
                    model.Model = bl.GetById((Guid)id);
                    if (model.Model == null)
                        throw new Exception();
                    isEdt = true;
                    oldParentId = model.Model.AccountParentId;
                    oldNum = model.Model.Num;
                }
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            this.DataContext = model;
        }


        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            //chek fields
            BindingExpression name = txtName.GetBindingExpression(TextBox.TextProperty);
            name.UpdateSource();

            if (name.HasError || c.SelectedItem == null)
            {
                saveBtn.IsEnabled = true;
                return;
            }

            model.Model.ArName = model.Model.EnName;
            //////////////////////////////////////////////////////
            string msg = ConstUserMsg.FaildProcess;
            model.Model.UserId = Profile.UserProfile.Id;
            model.Model.CropId = Profile.CorpProfile.Id;
            try
            {
                //chek name
                var vv = model.Model;
                var v = bl.Get(m => m.Id != model.Model.Id && (m.EnName.Equals(model.Model.EnName)
                || m.ArName.Equals(model.Model.ArName)) && m.AccountParentId == model.Model.AccountParentId)
                .FirstOrDefault();
                if (v != null)
                {
                    msg = ConstUserMsg.NameExist;
                    throw new Exception();
                }

                // update with void return or add with int return
                if (isEdt)
                    bl.Update(model.Model);
                else
                {
                    model.Model.Id = Guid.NewGuid();
                    bl.Insert(model.Model);
                    SetNew();
                }

                MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(msg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            saveBtn.IsEnabled = true;
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void c_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetNewNum();
        }
        private void GetNewNum()
        {
            if (c.SelectedItem == null || c.SelectedItem.GetType() != typeof(AccountM))
                return;
            var parentAcc = (AccountM)c.SelectedItem;
            if (oldParentId != null && oldParentId == parentAcc.Id)
            {
                model.Model.Num = oldNum;
                return;
            }
            model.Model.Num = bl.GetNewNum(parentAcc.Id, parentAcc.Num);
        }

        private void SetNew()
        {
            model.Model.AccountParentId = model.Accounts[0].Id;
            GetNewNum();
            model.Model.AccountEndId = 1;
            model.Model.Deleteable = true;
        }
    }
}
