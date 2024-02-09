using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
using MahApps.Metro.Controls;
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
    /// Interaction logic for UserWin.xaml
    /// </summary>
    public partial class StoreWin : MetroWindow
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private StoreVM model;
        private StoreBL bl;      
        private static StoreWin _instance;
        private Nullable<Guid> id;
        private bool isEdt = false;

        public static StoreWin ShowMe(Nullable<Guid> id)
        {
            if (_instance == null)
            {
                _instance = new StoreWin(id);
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public StoreWin(Nullable<Guid> id)
        {
            this.id = id;
            bl = new StoreBL(db);
            model = new StoreVM(db, bl);

            InitializeComponent();
            Loaded += MyWindow_Loaded;
        }


        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;
            try
            {
                if (id != null)
                {
                    model.Model = bl.GetById((Guid)id);
                    if (model.Model == null)
                        throw new Exception();
                    isEdt = true;
                    Title = Properties.Resources.EditStore;
                }
                else
                {
                    model.Set();
                    Title = Properties.Resources.AddNewStore;
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
            BindingExpression num = txtNum.GetBindingExpression(TextBox.TextProperty);
            num.UpdateSource();
            BindingExpression name = txtName.GetBindingExpression(TextBox.TextProperty);
            name.UpdateSource();

            if (num.HasError || name.HasError)
            {
                saveBtn.IsEnabled = true;
                return;
            }

            //////////////////////////////////////////////////////
            string msg = ConstUserMsg.FaildProcess;
            model.Model.CropId = Profile.CorpProfile.Id;
            try
            {
                //chek num, name is exist
                var v = bl.Get(m => m.Num.Equals(model.Model.Num) || m.Name.Equals(model.Model.Name)).FirstOrDefault();
                if (v != null && v.Id != model.Model.Id)
                {
                    if (v.Num.Equals(model.Model.Num))
                        msg = ConstUserMsg.CodeExist;
                    else
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
                }

                MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
                model.Set();
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
    }
}
