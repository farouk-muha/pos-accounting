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
    public class ProductVM : ViewModelBase
    {
        public CropAccountingAppEntities db;
        private ProductBL bl;
        private ProductM model;
        private CategoryBL CatBL;

        private ProductUnitM selectedProdectUnit;
        private ObservableCollection<CategoryM> categories;
        private ObservableCollection<SymbolTypeM> units;
        private double qTY;
        private ObservableCollection<SymbolTypeM> qtyUnits;
        private int qtyUnitId;
        private ICommand _deleteCommand;

        public ProductM Model { get { return model; } set { model = value; NotifyPropertyChanged("model"); } }
        public ProductUnitM SelectedProdectUnit { get { return selectedProdectUnit; } set { selectedProdectUnit = value; NotifyPropertyChanged("SelectedProdectUnit"); } }
        public ObservableCollection<CategoryM> Categories { get { return categories; } set { categories = value; NotifyPropertyChanged("categories"); } }
        public ObservableCollection<SymbolTypeM> Units { get { return units; } set { units = value; NotifyPropertyChanged("units"); } }
        public ObservableCollection<SymbolTypeM> QtyUnits { get { return qtyUnits; } set { qtyUnits = value; NotifyPropertyChanged("QtyUnits"); } }
        public double QTY { get { return qTY; } set { qTY = value; NotifyPropertyChanged("qTY"); } }
        public int QtyUnitId { get { return qtyUnitId; } set { qtyUnitId = value; NotifyPropertyChanged("qtyUnitId"); } }
        public decimal QTYPrice { get; set; }



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
        private ICommand newRowCommand;
        public ICommand NewRowCommand
        {
            get
            {
                if (newRowCommand == null)
                {
                    newRowCommand = new RelayCommand(param => AddNewRow(), null);
                }
                return newRowCommand;
            }
        }

        public void AddNewRowCommand()
        {
            try
            {
                AddNewRow();
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ProductVM(CropAccountingAppEntities db, ProductBL bl)
        {
            this.db = db;
            this.bl = bl;
            CatBL = new CategoryBL(db);
            Model = new ProductM();
        }

        public void Load()
        {
            Categories = CatBL.Get(null, null, 1, 20).Models;
            Units = new ObservableCollection<SymbolTypeM>();
            QtyUnits = new ObservableCollection<SymbolTypeM>();

            var v = new SymbolTypeBL(db).Get(ConstSymbol.Units);
            SymbolTypeUtils symbolUtils = new SymbolTypeUtils();
            foreach (var vv in v)
            {
                Units.Add(vv);
            }
        }

        public void Set()
        {
            Model.ProductUnits = new ObservableCollection<ProductUnitM>();
            if (Categories != null && Categories.Count > 0)
                Model.CategoryId = Categories[0].Id;
            Model.Name = string.Empty;
            Model.Status = true;
            model.Date = DateTime.Now;
            model.Code = string.Empty;
            AddNewRow();
        }

        public void Delete(object obj)
        {
            if (obj == null || obj.GetType() != typeof(ProductUnitM))
                return;

            if (Model.ProductUnits.Count() == 1)
            {
                var v = Model.ProductUnits.FirstOrDefault();
                v = new ProductUnitM()
                {
                    Id = Guid.NewGuid(),
                    ProductSKU = "0",
                    PriceBuy = 0,
                    PriceSale = 0,
                };
                v.IsDefault = true;
                return;
            }

            var modelToDelete = (ProductUnitM)obj;
            Model.ProductUnits.Remove(modelToDelete);
        }

        public void AddNewRow()
        {
            var v = new ProductUnitM()
            {
                Id = Guid.NewGuid(),
                ProductSKU = "0",
                PriceBuy = 0,
                PriceSale = 0,
            };

            if (model.ProductUnits.Count == 0)
            {
                v.IsDefault = true;
            }
            Model.ProductUnits.Add(v);
        }

        public void SetQtyUnits()
        {
            QtyUnits = new ObservableCollection<SymbolTypeM>();
            foreach(var v in model.ProductUnits)
            {
                if (v.UnitId < 1)
                    continue;
                var addUnit = Units.Where(m => m.Id == v.UnitId).FirstOrDefault();
               if(addUnit != null)
                    QtyUnits.Add(addUnit);
            }

            if(QtyUnitId < 1)
            {
                if (QtyUnits != null && QtyUnits.Count > 0)
                    QtyUnitId = QtyUnits[0].Id;
            }
        }

    }

    public class ProductsVM : PagingM<ProductM>
    {
        private ProductM selectedProduct;
        public ProductM SelectedProduct { get { return selectedProduct; } set { selectedProduct = value; NotifyPropertyChanged("SelectedProduct"); } }

        public CropAccountingAppEntities db;
        public ProductBL bl;


        public ProductsVM(CropAccountingAppEntities db, ProductBL bl)
        {
            Models = new ObservableCollection<ProductM>();
            this.db = db;
            this.bl = bl;
        }

        public override void Load()
        {
            WinName = Properties.Resources.Products;
            DisplayImg = ImgUtils.ProductIcon;
            Refresh();
            RefreshHead();
        }

        private void RefreshHead()
        {
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

            try
            {
                var temp = bl.Get(null, KW, StartDate, EndDate, Status, CurPage, PageSize);
                Models = temp.Models;
                Count = temp.Count;
                Amount = temp.Amount;
            }
            catch
            {
                MessageBox.Show(ConstUserMsg.FaildProcess, ConstUserMsg.FaildMsg, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public override void Add()
        {
            MyMainWindow.Instance.Product();
        }

        public override void Update(object id)
        {
            if (id == null || id.GetType() != typeof(Guid))
                return;
            MyMainWindow.Instance.Product((Guid)id);
        }

        public override void Delete(object id)
        {

            if (id == null || id.GetType() != typeof(Guid))
                return;

            var model = Models.Where(m => m.Id == (Guid)id).FirstOrDefault();
            if (model == null)
                return;

            var res = MessageBox.Show(string.Format(ConstUserMsg.DeleteConfirmMsg, Properties.Resources.TheProduct + "\"" + model.Name + "\"")
                + "\n\n " + Properties.Resources.ProductDelNote,
                ConstUserMsg.DeleteConfirm, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (res == MessageBoxResult.Cancel)
                return;

            ProductUnitBL unitBL = new ProductUnitBL(db);
            JournalBL journalBL = new JournalBL(db);
            JournalDetBL detBL = new JournalDetBL(db);

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    //delete journal
                    var journalToDet = new JournalBL(db).Get(m => m.RefId == model.Id).FirstOrDefault();
                    if (journalToDet != null)
                    {
                        foreach (var v in journalToDet.JournalDets)
                            detBL.Delete(v.Id);
                        journalBL.Delete(journalToDet.Id);
                    }
                    

                    //delete ProductUnits
                    foreach (var vv in model.ProductUnits)
                        unitBL.Delete(vv.Id);

                    bl.Delete(model.Id);
                    dbContextTransaction.Commit();
                    ImgUtils img = new ImgUtils();
                    img.Delete(img.ProductImgs, model.LocalImg);

                    Models.Remove(model);
                    --Count;
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
