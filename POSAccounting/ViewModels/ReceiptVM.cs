using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace POSAccounting.ViewModels
{
    public class ReceiptVM : ViewModelBase
    {
        private CropAccountingAppEntities db;
        private ReceiptBL bl;
        private ReceiptM model;
        private bool isInvoice;


        private ObservableCollection<AccountM> accounts;
        private ObservableCollection<InvoiceM> invoices;

        public ReceiptM Model { get { return model; } set { model = value; NotifyPropertyChanged("Model"); } }
        public bool IsInvoice { get { return isInvoice; } set { isInvoice = value; NotifyPropertyChanged("IsInvoice"); } }

        public ObservableCollection<AccountM> Accounts { get { return accounts; } set { accounts = value; NotifyPropertyChanged("Accounts"); } }
        public ObservableCollection<InvoiceM> Invoices { get { return invoices; } set { invoices = value; NotifyPropertyChanged("Invoices"); } }


        public ReceiptVM(CropAccountingAppEntities db, ReceiptBL bl)
        {
            this.db = db;
            this.bl = bl;
            Model = new ReceiptM();
            Accounts = new ObservableCollection<AccountM>();
        }

        public void SetNew()
        {
            Invoices = new ObservableCollection<InvoiceM>();
            IsInvoice = false;
            Model.IsCash = true;
            Model.Num = bl.GetNewNum(Model.ReceiptTypeId);
            Model.Date = DateTime.Now;
            Model.Amount = 0;
            Model.Note = string.Empty;
        }

    }

    public class ReceiptsVM : PagingM<ReceiptM>
    {
        public CropAccountingAppEntities db;
        public ReceiptBL bl;
        public ReceiptUtils utils = new ReceiptUtils();

        public ReceiptsVM(CropAccountingAppEntities db, ReceiptBL bl)
        {
            Models = new ObservableCollection<ReceiptM>();
            this.db = db;
            this.bl = bl;
        }

        public override void Load()
        {
            WinName = Properties.Resources.Receipts;
            DisplayImg = ImgUtils.CategoryIcon;
            Refresh();
            RefreshHead();
        }

        private void RefreshHead()
        {
            CountDisplay = Count.ToString();
            CurPage = Count > 0 ? 1 : 0;
        }

        public override void Refresh(object param = null)
        {
            if (param != null)
            {
                string s = param.ToString();
                if (s.Equals("c"))
                {
                    StartDate = null;
                    EndDate = null;
                    KW = null;
                    CurPage = 0;
                }
            }
            try
            {
                var temp = bl.Get(null, Id, TypeId, KW, StartDate, EndDate, CurPage, PageSize);
                Models = temp.Models;
                Count = temp.Count;
                Amount = temp.Amount;
                Credit = temp.Credit;
                Debt = temp.Debt;
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void Add()
        {
        }

        public override void Update(object id)
        {
            if (id == null || id.GetType() != typeof(Guid))
                return;

            var model = Models.Where(m => m.Id == (Guid)id).FirstOrDefault();
            if (model == null)
                return;

            if(model.ReceiptTypeId == ConstReceiptTypes.Drawings)
                MyMainWindow.Instance.ReceiptDrwaing(model.Id);
            else
                MyMainWindow.Instance.Receipt(0, model.Id);
        }

        public override void Delete(object id)
        {

            if (id == null || id.GetType() != typeof(Guid))
                return;

            var model = Models.Where(m => m.Id == (Guid)id).FirstOrDefault();
            if (model == null)
                return;

            var res = MessageBox.Show(string.Format(ConstUserMsg.DeleteConfirmMsg, Properties.Resources.TheReceipt + " \"" + model.Num + "\"")
                 + "\n \n " + Properties.Resources.ReceiptDelNote,
                ConstUserMsg.DeleteConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (res == MessageBoxResult.Cancel)
                return;

            JournalBL journalBL = new JournalBL(db);
            JournalDetBL detBL = new JournalDetBL(db);

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //delete journal
                    var journalToDet = new JournalBL(db).Get(m => m.RefId == model.Id).FirstOrDefault();
                    if (journalToDet == null)
                        throw new Exception();
                    foreach (var v in journalToDet.JournalDets)
                        detBL.Delete(v.Id);
                    journalBL.Delete(journalToDet.Id);

                    //delete inv recipet table
                    var invRec = db.ReceiptInvoices.Where(m => m.ReceiptId == model.Id).FirstOrDefault();
                    if(invRec != null)
                        db.ReceiptInvoices.Remove(invRec);

                    //delete Receit
                    bl.Delete(model.Id);
                    Models.Remove(model);
                    Count--;
                    dbContextTransaction.Commit();
                    RefreshHead();
                    MessageBox.Show(ConstUserMsg.SuccessProcess, ConstUserMsg.SuccessMsg, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
