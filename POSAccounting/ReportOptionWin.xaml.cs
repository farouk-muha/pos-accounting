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
    public partial class ReportOptionWin : MetroWindow
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private AccountVM model;
        private AccountBL bl;
        private static ReportOptionWin _instance;
        private int reportType;
        private Nullable<Guid> id;
        private Nullable<byte> typeId;

        public static ReportOptionWin ShowMe(int reportType, Nullable<Guid> id, Nullable<byte> typeId)
        {
            if (_instance == null)
            {
                _instance = new ReportOptionWin(reportType, id, typeId);
                _instance.Closed += (senderr, args) => _instance = null;

                _instance.Show();
            }
            else
                _instance.Activate();
            return _instance;
        }

        public ReportOptionWin(int reportType, Nullable<Guid> id, Nullable<byte> typeId)
        {
            this.reportType = reportType;
            this.id = id;
            this.typeId = typeId;
            bl = new AccountBL(db);
            model = new AccountVM(db, bl);

            InitializeComponent();
            Loaded += MyWindow_Loaded;
        }


        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            txtStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
            txtEndDate.SelectedDate = DateTime.Now;
            try
            {

                if (reportType == ReportTypes.AccJournals)
                {
                    model.Accounts = new ObservableCollection<AccountM>();
                }
                else if (reportType == ReportTypes.Visa)
                {
                    model.Accounts = bl.GetVisas();
                    model.Model = model.Accounts.FirstOrDefault();
                    SetAccount();
                    txtCode.IsEnabled = false;
                    txtAccSearch.Visibility = Visibility.Collapsed;
                }
                else if (reportType == ReportTypes.BoxSafe)
                {
                    model.Model = bl.GetById(ConstAccounts.BoxSafe);
                    SetAccount();
                    txtCode.IsEnabled = false;
                    cbAcc.IsEnabled = false;
                }
                else
                {
                    Height = 220;
                    grid.RowDefinitions[1].Height = new GridLength(0);
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

        private async void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(reportType == ReportTypes.BoxSafe || reportType == ReportTypes.Visa 
                || reportType == ReportTypes.AccJournals)
            {
                if ( model.Model == null || model.Model.Id == null || model.Model.Id == Guid.Empty)
                {
                    MessageBox.Show(ConstUserMsg.MustChoseAccount, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                id = model.Model.Id;
            }

            var start = txtStartDate.SelectedDate.Value;
            var end = txtEndDate.SelectedDate.Value;
            if (start >= end)
            {
                MessageBox.Show(ConstUserMsg.StartDateBiger, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            Cursor = Cursors.Wait;
            rec.Visibility = Visibility.Visible;
            await Task.Delay(20);


            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //search account
        #region
        private void txtCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string s = txtCode.Text;
                if (!string.IsNullOrEmpty(s))
                {
                    int i;
                    int.TryParse(s.Trim(), out i);
                    model.Model = bl.GetModels(m => m.Num == i).FirstOrDefault();

                    if (model.Model != null)
                        SetAccount();
                    else
                        cbAcc.Text = string.Empty;
                }
            }
            else
            {
                if (model.Model != null)
                {
                    model.Model = null;
                    cbAcc.Text = string.Empty;
                }
            }
        }

        private void cbAcc_KeyUp(object sender, KeyEventArgs e)
        {
            if (reportType != ReportTypes.AccJournals)
                return;

            string s = null;
            if (e.Key == Key.Enter)
            {
                s = cbAcc.Text;
                if (string.IsNullOrEmpty(s))
                    return;
                s = s.Trim();
                try
                {
                    model.Model = bl.GetModels(m => m.EnName.StartsWith(s) || m.ArName.StartsWith(s)).FirstOrDefault();
                    if (model.Model != null)
                        SetAccount();
                    else
                        txtCode.Text = string.Empty;
                }
                catch
                {

                }
            }
            else
            {
                if (model.Model != null)
                {
                    model.Model = null;
                    txtCode.Text = string.Empty;
                }
            }
        }

        private void cbAcc_DropDownOpened(object sender, EventArgs e)
        {
            cbAcc.IsDropDownOpen = false;
            if (brAcc.Visibility == Visibility.Collapsed)
            {
                brAcc.Visibility = Visibility.Visible;

                if (reportType == ReportTypes.AccJournals)
                {
                    model.Accounts = bl.GetModels(null);
                }
            }
            else
            {
                brAcc.Visibility = Visibility.Collapsed;
            }
        }

        private void txtAccSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            if (string.IsNullOrEmpty(txtAccSearch.Text))
                model.Accounts = bl.GetModels(null);
            else
                model.Accounts = bl.GetModels(m => m.EnName.StartsWith(txtAccSearch.Text.Trim())
                || m.ArName.StartsWith(txtAccSearch.Text.Trim()));
        }

        private void lbAcc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (model.Accounts != null && lbAcc.SelectedItem != null && lbAcc.SelectedItem.GetType() == typeof(AccountM))
            {
                model.Model = (AccountM)lbAcc.SelectedItem;
                SetAccount();
                brAcc.Visibility = Visibility.Collapsed;
            }
        }
        private void SetAccount()
        {
            cbAcc.Text = model.Model.EnName;
            txtCode.Text = model.Model.Num.ToString();
        }
        #endregion
    }
}
