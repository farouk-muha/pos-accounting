using POSAccounting.BL;
using POSAccounting.Models;
using POSAccounting.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace POSAccounting.ViewModels
{
    public class InvoiceVM : ViewModelBase
    {
        public CropAccountingAppEntities db;
        private InvoiceBL bl;
        private InvoiceM model;

        private ObservableCollection<StoreM> stores;
        private ObservableCollection<ProductM> products;
        private ObservableCollection<ClientM> client;
        private InvoiceLineM invLineToAdd;
        private InvoiceLineM selectedInv;
        private ProductUnitM selectedUnit;
        private ICommand _deleteCommand;
        private ICommand _unitSelctedCommand;

        public decimal TaxPercent { get; set; }
        public decimal DiscountPercent { get; set; }

        public InvoiceM Model { get { return model; } set { model = value; NotifyPropertyChanged("Model"); } }
        public ObservableCollection<ProductM> Products { get { return products; } set { products = value; NotifyPropertyChanged("Products"); } }
        public ObservableCollection<StoreM> Stores { get { return stores; } set { stores = value; NotifyPropertyChanged("Stores"); } }
        public ObservableCollection<ClientM> Clients { get { return client; } set { client = value; NotifyPropertyChanged("Clients"); } }
        public InvoiceLineM InvLineToAdd { get { return invLineToAdd; } set { invLineToAdd = value; NotifyPropertyChanged("InvLineToAdd"); } }
        public InvoiceLineM SelectedInv { get { return selectedInv; }  set { selectedInv = value; NotifyPropertyChanged("SelectedInv"); } }
        public ProductUnitM SelectedUnit { get { return selectedUnit; }
            set { selectedUnit = value; UnitSelcted(); NotifyPropertyChanged("SelectedUnit"); } }


        public InvoiceVM(CropAccountingAppEntities db, InvoiceBL bl)
        {
            this.db = db;
            this.bl = bl;
            Stores = new StoreBL(db).Get(null, null, 1, 20).Models;
            Model = new InvoiceM();
        }


        public void SetEdt() 
        {
            Model.Discount = 0;
            model.StoreId = Stores[0].Id;
            Clients = new ObservableCollection<ClientM>();
            Products = new ObservableCollection<ProductM>();

            InvLineToAdd = new InvoiceLineM();
            invLineToAdd.Product = new ProductM();
            invLineToAdd.Product.ProductUnits = new ObservableCollection<ProductUnitM>();

            Model.InvoiceLines = new ObservableCollection<InvoiceLineM>();
            SelectedInv = new InvoiceLineM();
            Model.UserId = Profile.UserProfile.Id;
        }
        public void SetNew()
        {
            SetEdt();
            model.IsPayed = true;
            Model.Num = bl.GetNewNum();
            Model.Date = DateTime.Now;
        }

        public void RestInvLineToAdd()
        {
            InvLineToAdd = new InvoiceLineM();
            InvLineToAdd.QTY = 0;
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(param => Delete(param), null);
                }
                return _deleteCommand;
            }
        }
        public ICommand UnitSelctedCommand
        {
            get
            {
                if (_unitSelctedCommand == null)
                {
                    _unitSelctedCommand = new RelayCommand(param => UnitSelcted(), null);
                }
                return _unitSelctedCommand;
            }
        }

        public void Delete(object model)
        {
            if (model == null)
                return;
            if (model.GetType() != typeof(InvoiceLineM))
                return;
            Model.InvoiceLines.Remove((InvoiceLineM)model);
            UpdateTotalSum();
        }

        public void UpdateTotalSum()
        {
            Model.TotalPrice = Model.InvoiceLines.Sum(m => m.TotalPrice);

            if (model.Tax > model.TotalPrice)
                model.Tax = 0;

            if (Model.Discount >= Model.TotalPrice)
                Model.Discount = 0;

            if (Model.Pay > Model.TotalPriceSubDisc)
            {
                Model.Pay = 0;
                Model.Cash = 0;
                model.Visa = 0;
                model.VisaId = null;
            }

            Model.TotalQty = Model.InvoiceLines.Sum(m => m.QTY);
            DeptUpdate();
        }

        public void DeptUpdate()
        {
            Model.TotalPriceSubDisc = Model.TotalPrice - Model.Discount + model.Tax;
            Model.Debt = Model.TotalPriceSubDisc - Model.Pay;
        }
        public void UnitSelcted()
        {
            if (SelectedInv == null || SelectedUnit == null)
                return;   
            selectedInv.Price = SelectedUnit.PriceSale;
            UpdateTotalSum();
            selectedInv.ProductUnit = SelectedUnit;

            var v = model.InvoiceLines;
        }
    }

    public class InvoicesVM : PagingM<InvoiceM>
    {
        public CropAccountingAppEntities db;
        public InvoiceBL bl;
        public InvoiceUtils utils = new InvoiceUtils();
        public bool? isaccClientDeb { get; set; }


        public InvoicesVM(CropAccountingAppEntities db, InvoiceBL bl, bool? isaccClientDeb)
        {
            Models = new ObservableCollection<InvoiceM>();
            this.db = db;
            this.bl = bl;
            this.isaccClientDeb = isaccClientDeb;
            Status = false;
        }

        public override void Load()
        {
            WinName = Properties.Resources.Invoices;
            DisplayImg = ImgUtils.InvoiceIcon;
            Refresh();
            RefreshHead();
        }
        private void RefreshHead()
        {
            CountDisplay = Count.ToString();
            CurPage = Models.Count > 0 ? 1 : 0;
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
            var temp = bl.Get(null, Id, TypeId, KW, StartDate, EndDate, Status, CurPage, PageSize, isaccClientDeb);

            try
            {
                //Stopwatch sw = new Stopwatch();
                //sw.Start();
                //sw.Stop();
                //Console.WriteLine("Elapsed={0}", sw.Elapsed);
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
            MyMainWindow.Instance.Invoice(0, (Guid)id);
        }

        public override void Delete(object id)
        {
            if (id == null || id.GetType() != typeof(Guid))
                return;

            var model = Models.Where(m => m.Id == (Guid)id).FirstOrDefault();
            if (model == null)
                return;

            var res = MessageBox.Show(string.Format(ConstUserMsg.DeleteConfirmMsg, Properties.Resources.TheInvoice + " " + model.Num + " ")
                 + "\n \n " + Properties.Resources.InvoiceDelNote,
                ConstUserMsg.DeleteConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (res == MessageBoxResult.Cancel)
                return;

            InvoicelineBL lineBL = new InvoicelineBL(db);
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

                    //delete inv lines
                    var linesToDelete = lineBL.Get(m => m.InvoiceId == model.Id);
                    foreach (var v in linesToDelete)
                    {
                        lineBL.Delete(v.Id);
                    }

                    bl.Delete(model.Id);
                    Models.Remove(model);
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
