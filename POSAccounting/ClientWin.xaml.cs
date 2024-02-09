using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ClientWin.xaml
    /// </summary>
    public partial class ClientWin : MetroWindow
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private ClientVM model;
        private ClientBL bl;      
        private static ClientWin _instance;
        private Nullable<Guid> id;
        private bool isEdt = false;
        private bool onlyShow = false;
        private ImgUtils imgutils = new ImgUtils();
        private bool isImgUpdate = false;


        public static ClientWin ShowMe(Nullable<Guid> id, bool onlyShow)
        {
            if (_instance == null)
            {
                _instance = new ClientWin(id, onlyShow);
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public ClientWin(Nullable<Guid> id = null, bool onlyShow = false)
        {
            this.id = id;
            this.onlyShow = onlyShow;
            bl = new ClientBL(db);
            model = new ClientVM(db, bl);

            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (id != null)
                {
                    model.Model = bl.GetById((Guid)id);
                    if (model.Model == null)
                    {
                        throw new Exception();
                    }

                    isEdt = true;
                    //chek local img is null set default or get img
                    if (!String.IsNullOrEmpty(model.Model.LocalImg))
                    {
                        var path = imgutils.GetImgFullPath(model.Model.LocalImg, imgutils.ClientImgs);
                        if (File.Exists(path))
                            model.Model.DisplayImg = path;
                    }

                    if (onlyShow)
                    {
                        txtName.IsEnabled = false;
                        txtPhone.IsEnabled = false;
                        txtAddress.IsEnabled = false;
                        coType.IsEnabled = false;
                        chkActive.IsEnabled = false;
                        chkCompany.IsEnabled = false;
                        saveBtn.Visibility = Visibility.Collapsed;
                        cancelBtn.Visibility = Visibility.Collapsed;
                        Title = model.Model.Name;
                    }
                    else
                        Title = Properties.Resources.EditClient;
                }
                else
                {
                    Title = Properties.Resources.AddNewClient;
                    model.Set();
                }
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            this.DataContext = model;

            if (string.IsNullOrEmpty(model.Model.DisplayImg))
                model.Model.DisplayImg = ImgUtils.CategoryIcon;
        }

        private void img_MouseDown(object sender, MouseEventArgs e)
        {
            if (onlyShow)
                return;

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
            BindingExpression name = txtName.GetBindingExpression(TextBox.TextProperty);
            name.UpdateSource();
            BindingExpression phone = txtPhone.GetBindingExpression(TextBox.TextProperty);
            phone.UpdateSource();
            BindingExpression address = txtAddress.GetBindingExpression(TextBox.TextProperty);
            address.UpdateSource();

            if (name.HasError || phone.HasError || address.HasError)
            {
                saveBtn.IsEnabled = true;
                return;
            }
            //////////////////////////////////////////////////////
            String msg = ConstUserMsg.FaildProcess;
            try
            {
                //save image and delete old one
                if (isImgUpdate)
                    model.Model.LocalImg = imgutils.SaveImg(model.Model.DisplayImg, imgutils.ClientImgs,
                        model.Model.LocalImg);


                //chek name
                var v = bl.Get(m => m.Name.Equals(model.Model.Name.Trim())).FirstOrDefault();
                if (v != null && v.Id != model.Model.Id)
                {
                    msg = ConstUserMsg.NameExist;
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show(msg, msg, MessageBoxButton.OK, MessageBoxImage.Error);
                saveBtn.IsEnabled = true;
                return;
            }

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {                   
                    // update void return or add with guid return
                    if (!isEdt)
                    {
                        //getc parent client account
                        AccountBL accBL = new AccountBL(db);
                        var parent = accBL.GetById(ConstAccounts.CurrentAssets);
                        //add client account
                        var account = new AccountM();
                        account.Id = Guid.NewGuid();
                        account.Deleteable = true;
                        account.Num = accBL.GetNewNum(parent.Id, parent.Num);
                        account.EnName = model.Model.Name;
                        account.ArName = model.Model.Name;
                        account.AccountParentId = parent.Id;
                        account.UserId = Profile.UserProfile.Id;
                        account.CropId = Profile.CorpProfile.Id;
                        account.AccountEndId = 1;
                        var i = accBL.Insert(account);

                        model.Model.AccountId = i;
                        model.Model.Id = Guid.NewGuid();
                        model.Model.Num = account.Num;
                        bl.Insert(model.Model);
                    }
                    else
                    {
                        bl.Update(model.Model);
                    }

                    dbContextTransaction.Commit();
                    MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
                    if(!isEdt)
                        model.Set();

                }
                catch
                {
                    MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                saveBtn.IsEnabled = true;
            }

        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
