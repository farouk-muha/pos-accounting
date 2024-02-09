using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using POSAccounting.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSAccounting.Views
{
    /// <summary>
    /// Interaction logic for ReceiptUC.xaml
    /// </summary>
    public partial class ReceiptUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private ReceiptVM model;
        private ReceiptBL bl;
        private AccountBL accBL;
        private AccountUtils accUtils = new AccountUtils();
        private Nullable<Guid> id = null;
        private bool isEdt = false;
        private List<Guid> ids;

        public ReceiptUC(byte typeId, Nullable<Guid> id = null)
        {
            this.id = id;
            bl = new ReceiptBL(db);
            accBL = new AccountBL(db);
            model = new ReceiptVM(db, bl);
            model.Model.ReceiptTypeId = typeId;
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }

        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dg.Visibility = Visibility.Collapsed;

            txtDate.Height = 4;
            try
            {
                if (id != null)
                {
                    await Task.Delay(20);
                    isEdt = true;
                    model.Model = bl.GetById((Guid)id);

                    if (model.Model == null)
                    {
                        return;
                    }
                    txtDate.IsEnabled = false;
                    model.Model.Amount = 0;
                }
                else
                    model.SetNew();

                if (model.Model.ReceiptTypeId == ConstReceiptTypes.PayReceipt)
                {
                    var temp = accBL.GetChildeGuids(new Guid?[] { ConstAccounts.Creditors, ConstAccounts.Debtors});
                    ids = new List<Guid>(temp);
                }
                else if (model.Model.ReceiptTypeId == ConstReceiptTypes.CatchReceipt)
                {
                    var temp = accBL.GetChildeGuids(new Guid?[] { ConstAccounts.Creditors, ConstAccounts.Debtors });
                    ids = new List<Guid>(temp);
                    txtBlkAccount.Text = Properties.Resources.FromClient;
                }
                else if (model.Model.ReceiptTypeId == ConstReceiptTypes.Expenses)
                {
                    var temp = accBL.GetChildeGuids(new Guid?[] { ConstAccounts.TradingExpenses,
                        ConstAccounts.UtilitiesExpenses });
                    ids = new List<Guid>(temp);
                    txtBlkAccount.Text = Properties.Resources.ToAccount;
                }
                else if (model.Model.ReceiptTypeId == ConstReceiptTypes.Revenues)
                {
                    var temp = accBL.GetChildeGuids(new Guid?[] { ConstAccounts.Revenue });
                    ids = new List<Guid>(temp);
                    ids.Add(ConstAccounts.Sales);
                    txtBlkAccount.Text = Properties.Resources.FromAccount;
                }
            }
            catch
            {
            }


            this.DataContext = model;
           if(isEdt)
            {
                model.Model.Account = accBL.GetById(model.Model.AccountId);
                cbAcc.Text = model.Model.Account.EnName;
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            string msg = ConstUserMsg.FaildProcess;
            bool errors = false;

            try
            {
                if (model.IsInvoice && model.Invoices != null)
                {
                    errors = (from c in (from object i in dg.ItemsSource select dg.ItemContainerGenerator.ContainerFromItem(i))
                                  where c != null
                                  select Validation.GetHasError(c)).FirstOrDefault(x => x);
                }



                //chek fields
                BindingExpression num = txtNum.GetBindingExpression(TextBox.TextProperty);
                num.UpdateSource();
                BindingExpression amount = txtAmount.GetBindingExpression(TextBox.TextProperty);
                amount.UpdateSource();

                if (errors || num.HasError || amount.HasError)
                {
                    saveBtn.IsEnabled = true;
                    return;
                }

                if (model.Model.Account == null)
                {
                    msg = ConstUserMsg.MustChoseAccount;
                    throw new Exception();
                }
                model.Model.AccountId = model.Model.Account.Id;

                ////chk num exist
                //var v = bl.Get(m => m.Id != model.Model.Id && m.Num == model.Model.Num).FirstOrDefault();
                //if (v != null)
                //{
                //    msg = ConstUserMsg.NumExist;
                //    throw new Exception();
                //}
            }
            catch
            {
                MessageBox.Show(msg, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                saveBtn.IsEnabled = true;
                return;
            }

            save();
        }

        private void save()
        {
            InvoiceBL invBL = new InvoiceBL(db);
            JournalBL journalBL = new JournalBL(db);
            JournalDetBL detBL = new JournalDetBL(db);
            model.Model.UserId = Profile.UserProfile.Id;
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    // receipt
                    if (isEdt)
                    {
                        //delete journal
                        var journalToDet = journalBL.Get(m => m.RefId == model.Model.Id).FirstOrDefault();
                        if (journalToDet == null)
                            throw new Exception();
                        foreach (var v in journalToDet.JournalDets)
                            detBL.Delete(v.Id);
                        journalBL.Delete(journalToDet.Id);

                        // update receipt
                        bl.Update(model.Model);
                    }
                    else
                    {
                        model.Model.Id = Guid.NewGuid();
                        bl.Insert(model.Model);
                    }

                    if (model.IsInvoice && model.Invoices != null)
                    {
                        foreach (var v in model.Invoices)
                        {
                            if (v.IsPayed)
                            {
                                invBL.UpdatePayStatus(v.Id, false);
                                bl.InsertReceiptInvoice(model.Model.Id, v.Id);
                            }
                        }
                    }

                    //journal
                    JournalM journal = new JournalM()
                    {
                        Id = Guid.NewGuid(),
                        Num = journalBL.GetNewNum(),
                        Date = model.Model.Date,
                        JournalTypeId = ConstJournalTypes.Receipt,
                        RefId = model.Model.Id,
                        UserId = model.Model.UserId,
                    };

                    journalBL.Insert(journal);


                    //journal dets part account selected
                    int lastNum = detBL.GetNewNum(detBL.GetLastNum());
                    JournalDetM acc = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = lastNum,
                        JournalId = journal.Id,
                        AccountID = model.Model.AccountId,
                    };
                    // if visa add det with bacnk account,  if cash add det with box safe account 
                    JournalDetM box = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = journal.Id,
                    };

                    if (model.Model.IsCash)
                        box.AccountID = ConstAccounts.BoxSafe;
                    else
                        box.AccountID = (Guid)model.Model.VisaId;

                    //journal dets  discount
                    JournalDetM discount = new JournalDetM()
                    {
                        Id = Guid.NewGuid(),
                        Num = ++lastNum,
                        JournalId = journal.Id,
                    };

                    if (model.Model.ReceiptTypeId == ConstReceiptTypes.PayReceipt || model.Model.ReceiptTypeId == ConstReceiptTypes.Expenses)
                    {
                        acc.Debt = model.Model.Amount;
                        box.Credit = (model.Model.Amount - model.Model.Discount);
                        discount.Credit = model.Model.Discount;
                        discount.AccountID = ConstAccounts.AcquiredDiscount;
                    }
                    else
                    {
                        acc.Credit = model.Model.Amount;
                        box.Debt = (model.Model.Amount - model.Model.Discount);
                        discount.Debt = model.Model.Discount;
                        discount.AccountID = ConstAccounts.DiscountPermitted;
                    }
                    detBL.Insert(acc);
                    detBL.Insert(box);
                    if(model.Model.Discount > 0)
                        detBL.Insert(discount);

                    dbContextTransaction.Commit();
                    MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
                    if (!isEdt)
                        model.SetNew();
                }
                catch
                {
                    MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            saveBtn.IsEnabled = true;
        }

        /// /////////////////////////////////
        /// Search acc 
        #region
        private void coAcc_KeyUp(object sender, KeyEventArgs e)
        {

            string s = null;
            if (e.Key == Key.Enter)
            {
                s = cbAcc.Text;
                if (string.IsNullOrEmpty(s))
                    return;
                s = s.Trim();
                int i = 0; int.TryParse(s, out i);
                try
                {
                    if (i > 0)
                        model.Model.Account = accBL.GetModels(m => ids.Contains(m.Id) && m.Num == i).FirstOrDefault();
                    else
                        model.Model.Account = accBL.GetModels(m => ids.Contains(m.Id) && m.EnName.StartsWith(s)
                         || m.ArName.StartsWith(s)).FirstOrDefault();
                    if (model.Model.Account != null)
                    {
                        cbAcc.Text = model.Model.Account.EnName;
                        invoiceChkBox.IsChecked = false;
                    }

                }
                catch
                {
                }
            }
            else
            {
                if (model.Model.Account != null)
                    model.Model.Account = null;
            }
        }

        private void coAcc_DropDownOpened(object sender, EventArgs e)
        {
            cbAcc.IsDropDownOpen = false;
            if (brAccSearch.Visibility == Visibility.Collapsed)
            {
                model.Accounts = accBL.GetModels(m => ids.Contains(m.Id));
                brAccSearch.Visibility = Visibility.Visible;
            }
            else
            {
                brAccSearch.Visibility = Visibility.Collapsed;
                txtAccSearch.Text = string.Empty;
            }                        
        }
        private void cbAcc_LostFocus(object sender, RoutedEventArgs e)
        {
            if(brAccSearch.Visibility == Visibility.Visible)
            {
                brAccSearch.Visibility = Visibility.Collapsed;
                txtAccSearch.Text = string.Empty;
            }
        }

        private void txtAccSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string s = txtAccSearch.Text;
            if (string.IsNullOrEmpty(s))
                return;

            s = s.Trim();
            int i;
            int.TryParse(s, out i);
            if (i == 0)
                model.Accounts = accBL.GetModels(m => ids.Contains(m.Id) &&
                (m.EnName.StartsWith(s) || m.ArName.StartsWith(s)));
            else
                model.Accounts = accBL.GetModels(m => ids.Contains(m.Id) && m.Num == i);
        }
        private void blAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (model.Accounts == null || blAccounts.SelectedItem == null)
                return;
            if (blAccounts.SelectedItem.GetType() == typeof(AccountM))
            {
                model.Model.Account = (AccountM)blAccounts.SelectedItem;
                brAccSearch.Visibility = Visibility.Collapsed;
                txtAccSearch.Text = string.Empty;
                cbAcc.Text = model.Model.Account.EnName;
                invoiceChkBox.IsChecked = false;
            }
        }
        #endregion

        //visa dialog
        private void CashUnChk_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new VisaPayWin(false, 0, 0, null);
            if (dialog.ShowDialog() == true)
            {
                model.Model.VisaId = dialog.model.VisaId;
            }
            else
            {
                if (model.Model.VisaId == null)
                    cashBox.IsChecked = true;
            }
        }
        private void cashBox_Checked(object sender, RoutedEventArgs e)
        {
            model.Model.VisaId = null;
        }

        //invoice check box 
        private void invoiceChkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            model.Model.Amount = 0;
            model.Model.Discount = 0;
            dg.Visibility = Visibility.Collapsed;
            txtAmount.IsEnabled = true;
            txtDiscount.IsEnabled = true;
        }
        private void invoiceChkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (model.Model.Account == null)
            {
                model.IsInvoice = false;
                return;
            }
            dg.Visibility = Visibility.Visible;
            txtAmount.IsEnabled = false;
            txtDiscount.IsEnabled = false;
            if ((model.Invoices != null && model.Invoices.Count > 0))
                return;
            model.Model.Amount = 0;
            model.Model.Discount = 0;
            if (model.Model.ReceiptTypeId == ConstReceiptTypes.CatchReceipt)
                model.Invoices = new InvoiceBL(db).GetNotPayed(new JournalBL(db), model.Model.Account.Id, true, false);
            else
                model.Invoices = new InvoiceBL(db).GetNotPayed(new JournalBL(db), model.Model.Account.Id, false, false);
        }

        //datagrid 
        private void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);

                Control control = MyDataGridHelper.GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
                if (control != null)
                {
                    control.Focus();
                }
            }
        }
        private void OnChecked(object sender, RoutedEventArgs e)
        {

            if (dg.SelectedItem != null )
                UpdateInvoicesAmount();
        }
        private void unChecked(object sender, RoutedEventArgs e)
        {
            if (dg.SelectedItem != null )
                UpdateInvoicesAmount();
        }
        private void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var columnIndex = dg.Columns.IndexOf(dg.CurrentColumn);
            //if enter selecet first row
            var u = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && u != null && columnIndex == 6)
            {
                UpdateInvoicesAmount();
            }
        }

        private void UpdateInvoicesAmount()
        {
            decimal amount = 0;
            decimal discount = 0;

            foreach (var v in model.Invoices)
            {
                if (v.IsPayed)
                {
                    amount += v.Debt;
                    discount += MathFun.GetAmount(v.Debt, v.Percent);
                }
            }
            model.Model.Amount = (amount - discount);
            model.Model.Discount = discount;
        }
    }
}
