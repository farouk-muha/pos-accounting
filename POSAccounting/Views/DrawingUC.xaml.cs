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
    public partial class DrawingUC : UserControl
    {
        private CropAccountingAppEntities db = new CropAccountingAppEntities(AppSettings.GetEntityConnectionStringBuilder());
        private ReceiptVM model;
        private ReceiptBL bl;
        private AccountBL accBL;
        private AccountUtils accUtils = new AccountUtils();
        private Nullable<Guid> id = null;
        private bool isEdt = false;

        public DrawingUC(Nullable<Guid> id = null)
        {
            //model.Accounts = accBL.GetModels(m => m.AccountParentId == ConstAccounts.Partners);

            this.id = id;
            bl = new ReceiptBL(db);
            accBL = new AccountBL(db);
            model = new ReceiptVM(db, bl);
            model.Model.ReceiptTypeId = ConstReceiptTypes.Drawings;
            InitializeComponent();
            if (AppSettings.GetLang() == ConstLang.Ar)
                FlowDirection = FlowDirection.RightToLeft;

            Loaded += MyWindow_Loaded;
        }

        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {

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

                model.Accounts = accBL.GetModels(m => m.AccountParentId == ConstAccounts.Partners);

                this.DataContext = model;

                if (isEdt)
                {
                    model.Model.Account = model.Accounts.Where(m => m.Id == model.Model.AccountId).FirstOrDefault();
                }

            }
            catch
            {
            }

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            saveBtn.IsEnabled = false;
            string msg = ConstUserMsg.FaildProcess;
            bool errors = false;

            try
            {

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

                   
                    detBL.Insert(acc);
                    detBL.Insert(box);

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

        private void cbAcc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (model.Accounts == null || cbAcc.SelectedItem == null)
                return;
            if (cbAcc.SelectedItem.GetType() == typeof(AccountM))
            {
                model.Model.Account = (AccountM)cbAcc.SelectedItem;
            }
        }
    }
}
