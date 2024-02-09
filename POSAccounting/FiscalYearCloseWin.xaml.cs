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
    public partial class FiscalYearCloseWin : MetroWindow
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private FiscalYearVM model;
        private FiscalYearBL bl;
        private static FiscalYearCloseWin _instance;

        public static FiscalYearCloseWin ShowMe()
        {
            if (_instance == null)
            {
                _instance = new FiscalYearCloseWin();
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public FiscalYearCloseWin()
        {
            bl = new FiscalYearBL(db);
            model = new FiscalYearVM(db, bl);

            InitializeComponent();
            Loaded += MyWindow_Loaded;
        }


        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            try
            {
                model.Load();
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
            //chek fields
            BindingExpression start = txtNewStartDate.GetBindingExpression(DatePicker.TextProperty);
            start.UpdateSource();
            BindingExpression end = txtNewEndDate.GetBindingExpression(DatePicker.TextProperty);
            end.UpdateSource();

            if (start.HasError || end.HasError )
            {
                saveBtn.IsEnabled = true;
                return;
            }

            if (model.Model.NewStartDate <= model.Model.EndDate || model.Model.NewEndDate <= model.Model.NewStartDate)
            {
                MessageBox.Show(ConstUserMsg.StartDateBiger, ConstUserMsg.FaildMsg);
                return;
            }

            MessageBox.Show(Properties.Resources.AccountsCloseNote);

            FiscalYearFun fun = new FiscalYearFun(db, new AccountBL(db));
            string msg = ConstUserMsg.FaildProcess;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (!RoleHelper.chkUserLocalRole(db, ConstTask.Settings, ConstActionType.Insert))
                    {
                        MessageBox.Show(string.Format(ConstNotAllowed.Insert, "Settings"), ConstUserMsg.NotAllowed, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    txtWait.Text = Properties.Resources.PleaseWait + " " + Properties.Resources.FiscalYearBeingClosed;
                    rec.Visibility = Visibility.Visible;
                    var v = fun.CloseAccounts(DateTime.Now, model.Model.StartDate, model.Model.EndDate);

                    if (v == 0)
                        MessageBox.Show(ConstUserMsg.SuccessProcess);
                    else if (v == 1)
                    {
                        msg = Properties.Resources.ErrorLastYearNotClosed;
                        throw new Exception();
                    }
                    else if (v == 2)
                    {
                        msg = Properties.Resources.ErrorYearAlreadyClosed;
                        throw new Exception();
                    }
                    else
                        throw new Exception();

                    txtWait.Text += " " + Properties.Resources.FiscalYearBeingTrans;
                    var b = fun.TransAccounts(DateTime.Now, model.Model.StartDate, model.Model.EndDate);

                    if(!b)
                        throw new Exception();

                    bl.Insert(new FiscalYearM() { StartDate = model.Model.NewStartDate, EndDate = model.Model.NewEndDate });


                    dbContextTransaction.Commit();
                    txtWait.Text = ConstUserMsg.SuccessProcess;

                }
                catch
                {
                    txtWait.Text = msg;
                }
                btnOk.Visibility = Visibility.Visible;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
