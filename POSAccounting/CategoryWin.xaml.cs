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
    public partial class CategoryWin : MetroWindow
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private CategoryVM model;
        private CategoryBL bl;      
        private static CategoryWin _instance;
        private Nullable<Guid> id;
        private bool isEdt = false;
        

        public static CategoryWin ShowMe(Nullable<Guid> id)
        {
            if (_instance == null)
            {
                _instance = new CategoryWin(id);
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public CategoryWin(Nullable<Guid> id)
        {
            this.id = id;
            bl = new CategoryBL(db);
            model = new CategoryVM(db, bl);

            InitializeComponent();
            Loaded += MyWindow_Loaded;
        }


        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = model;

            if (id != null)
            {
                model.Model = bl.GetById((Guid)id);
             
                if (model.Model == null)
                {
                    Close();
                    return;
                }
                
            }
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
