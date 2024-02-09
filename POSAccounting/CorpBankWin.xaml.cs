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
    public partial class CorpBankWin : MetroWindow
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private AccountVM model;
        private AccountBL bl;      
        private static CorpBankWin _instance;
        private Nullable<Guid> id;
        private bool isEdt = false;
        private ObservableCollection<BankM> Banks;


        public static CorpBankWin ShowMe(Nullable<Guid> id = null)
        {
            if (_instance == null)
            {
                _instance = new CorpBankWin(id);
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public CorpBankWin(Nullable<Guid> id = null)
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
                }

                Banks = new BankBL(db).GetModels(null);
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (Banks.Count() == 0)
            {
                saveBtn.IsEnabled = false;
                return;
            }

            this.DataContext = model;
            c.ItemsSource = Banks;
            if(!isEdt)
            c.SelectedValue = Banks[0].Id;
            else
                c.SelectedValue = Banks.Where(m => m.Name.Equals(model.Model.EnName)).Select(m => m.Id).FirstOrDefault();
        }


        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;

            model.Model.EnName = c.Text.ToString();
            model.Model.ArName = model.Model.EnName;

            //////////////////////////////////////////////////////
            string msg = ConstUserMsg.FaildProcess;
            model.Model.UserId = Profile.UserProfile.Id;
            model.Model.CropId = Profile.CorpProfile.Id;
            try
            {
                //chek name
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

        private void SetNew()
        {
            var parent = bl.GetById(ConstAccounts.Bank);
            model.Model.Num = bl.GetNewNum(parent.Id, parent.Num);
            model.Model.AccountParentId = parent.Id;
            model.Model.AccountEndId = 1;
            model.Model.Deleteable = true;
        }
    }
}
