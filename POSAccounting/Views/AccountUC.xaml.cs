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

namespace POSAccounting.Views
{
    /// <summary>
    /// Interaction logic for AccountWin.xaml
    /// </summary>
    public partial class AccountUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private AccountVM model;
        private AccountBL bl;      
        private Nullable<Guid> id;
        private bool isEdt = false;


        public AccountUC(Nullable<Guid> id)
        {
            this.id = id;
            bl = new AccountBL(db);
            model = new AccountVM(db, bl);

            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }


        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                model.Load();
                if (id != null)
                {
                    model.Model = bl.GetById((Guid)id);
                    isEdt = true;
                    txtNum.IsEnabled = false;
                    txtDate.IsEnabled = false;

                    for (int i = 0; i < model.Accounts.Count; i++)
                    {
                        if (model.Accounts[i].AccountParentId == id)
                        {
                            cbAcc.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    model.Set();
                    cbAcc.SelectedIndex = 0;
                }
            }
            catch
            {
            }
            this.DataContext = model;

        }

        private void cbAccounts_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (model.Accounts == null || cbAcc.SelectedItem == null)
                return;
            if (cbAcc.SelectedItem.GetType() == typeof(AccountM))
            {
                model.Model.Account = (AccountM)cbAcc.SelectedItem;
                model.Model.AccountParentId = model.Model.Account.AccountParentId;
                try
                {
                    model.Model.Num = bl.GetNewNum((Guid)model.Model.Account.AccountParentId, model.Model.Account.Num);

                }
                catch
                {
                    MessageBox.Show(ConstUserMsg.FaildProcess);
                }
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            //chek fields
            BindingExpression num = txtNum.GetBindingExpression(TextBox.TextProperty);
            num.UpdateSource();
            BindingExpression enName = txtEnName.GetBindingExpression(TextBox.TextProperty);
            enName.UpdateSource();

            if (num.HasError || enName.HasError)
            {
                saveBtn.IsEnabled = true;
                return;
            }

            model.Model.ArName = model.Model.EnName;
            //////////////////////////////////////////////////////          
            string msg = ConstUserMsg.FaildProcess;
            model.Model.UserId = Profile.UserProfile.Id;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //chek name 
                    var v = bl.Get(m => m.Id != model.Model.Id && (m.EnName.Equals(model.Model.EnName) || m.ArName.Equals(model.Model.ArName))).FirstOrDefault();
                    if (v != null)
                    {
                        msg = ConstUserMsg.NameExist;
                        throw new Exception();
                    }

                    // update void return or add with guid return
                    if (!isEdt)
                    {     
                        model.Model.Id = Guid.NewGuid();
                        model.Model.CropId = Profile.CorpProfile.Id;
                        bl.Insert(model.Model);
                    }
                    else
                    {
                        bl.Update(model.Model);
                    }

                    dbContextTransaction.Commit();
                    if(!isEdt)
                        model.Set();

                    MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show(msg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                saveBtn.IsEnabled = true;
            }
        }
    }
}
