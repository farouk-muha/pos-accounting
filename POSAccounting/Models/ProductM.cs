using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
    public class ProductM : ViewModelBase
    {
        private Guid id;
        private string code;
        private string name;
        private DateTime date;
        private double qTY;
        private bool status;
        private string img;
        private string localImg;
        private string note;
        private int corpId;
        private string storeName;
        private Guid categoryId;
        private byte defaultUnitId;
        private ObservableCollection<ProductUnitM> productUnits;
        private ProductUnitM productUnit;
        private decimal totalPrice;
        private string displayImg;

        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("id"); } }
        public string Code { get { return code; } set { code = value; NotifyPropertyChanged("code"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("name"); } }
        public DateTime Date { get { return date; } set { date = value; NotifyPropertyChanged("date"); } }
        public double QTY { get { return qTY; } set { qTY = value; NotifyPropertyChanged("qTY"); } }
        public bool Status { get { return status; } set { status = value; NotifyPropertyChanged("status"); } }
        public string Img { get { return img; } set { img = value; NotifyPropertyChanged("img"); } }
        public string LocalImg { get { return localImg; } set { localImg = value; NotifyPropertyChanged("LocalImg"); } }
        public string Note { get { return note; } set { note = value; NotifyPropertyChanged("note"); } }
        public int CorpId { get { return corpId; } set { corpId = value; NotifyPropertyChanged("corpId"); } }
        public string StoreName { get { return storeName; } set { storeName = value; NotifyPropertyChanged("storeName"); } }
        public Guid CategoryId { get { return categoryId; } set { categoryId = value; NotifyPropertyChanged("categoryId"); } }
        public byte DefaultUnitId { get { return defaultUnitId; } set { defaultUnitId = value; NotifyPropertyChanged("DefaultUnitId"); } }
        public ObservableCollection<ProductUnitM> ProductUnits { get { return productUnits; } set { productUnits = value; NotifyPropertyChanged("productUnits"); } }
        public decimal TotalPrice { get { return totalPrice; } set { totalPrice = value; NotifyPropertyChanged("TotalPrice"); } }
        public ProductUnitM ProductUnit { get { return productUnit; } set { productUnit = value; NotifyPropertyChanged("ProductUnit"); } }
        public string DisplayImg { get { return displayImg; } set { displayImg = value; NotifyPropertyChanged("DisplayImg"); } }

    }

    public class ProductUtils
    {
        public static ProductM FromEntity(Product entity)
        {
            var model = entity == null ? null :
                new ProductM()
                {
                    Id = entity.Id,
                    Code = entity.Code,
                    Name = entity.Name,
                    Date = entity.Date,
                    QTY = entity.QTY,
                    Status = entity.Status,
                    Img = entity.Img,
                    LocalImg = entity.LocalImg,
                    Note = entity.Note,
                    CorpId = entity.CorpId,
                    CategoryId = entity.CategoryId,
                    DefaultUnitId = entity.DefaultUnitId,

                    
                };
            return model;
        }

        public static Product FromModel(ProductM model)
        {
            return model == null ? null :
                new Product()
                {
                    Id = model.Id,
                    Code = model.Code,
                    Name = model.Name,
                    Date = model.Date,
                    QTY = model.QTY > 0 ? model.QTY : 0,
                    Status = model.Status,
                    Img = model.Img,
                    LocalImg = model.LocalImg,
                    Note = model.Note,
                    CorpId = model.CorpId,
                    CategoryId = model.CategoryId,
                    DefaultUnitId = model.DefaultUnitId,
                };
        }

        public static UnitConversion FromModel(ProductUnitM model)
        {
            return model == null ? null :
                new UnitConversion()
                {
                    ProductId = model.ProductId,
                    Multiplier = model.Multiplier,
                    BaseUnitId = (byte)model.UnitId,
                };
        }
    }


}
