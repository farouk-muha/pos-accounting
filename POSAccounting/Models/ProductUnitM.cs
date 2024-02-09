using POSAccounting.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Models
{
   public class ProductUnitM : ErrorBaseVM 
    {
        private Guid id;
        private string productSKU;
        private decimal priceBuy;
        private decimal priceSale;
        private Guid productId;
        private int unitId;
        private double multiplier;
        private string unitName;
        public ProductM product;
        private bool isDefault;

        public Guid Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string ProductSKU { get { return productSKU; } set { productSKU = value; NotifyPropertyChanged("ProductSKU"); } }
        public decimal PriceBuy { get { return priceBuy; } set { priceBuy = value; NotifyPropertyChanged("PriceBuy"); } }
        public decimal PriceSale { get { return priceSale; } set { priceSale = value; NotifyPropertyChanged("PriceSale"); } }
        public Guid ProductId { get { return productId; } set { productId = value; NotifyPropertyChanged("ProductId"); } }
        public int UnitId { get { return unitId; } set { unitId = value; NotifyPropertyChanged("UnitId"); } }
        public double Multiplier { get { return multiplier; } set { multiplier = value; NotifyPropertyChanged("Multiplier"); } }
        public ProductM Product { get { return product; } set { product = value; NotifyPropertyChanged("Product"); } }
        public string UnitName { get { return unitName; } set { unitName = value; NotifyPropertyChanged("UnitName"); } }
        public bool IsDefault { get { return isDefault; } set { isDefault = value; NotifyPropertyChanged("IsDefault"); } }


        public ProductUnitM Copy()
        {
            return new ProductUnitM
            {
                Id = this.Id,
            ProductSKU = this.ProductSKU,
            PriceBuy = this.PriceBuy,
            PriceSale = this.PriceSale,
            ProductId = this.ProductId,
            UnitId = this.UnitId,
            Multiplier = this.Multiplier,
        };
        }
    }

    public class ProductUnitUtils
    {
        public static ProductUnitM FromEntity(ProductUnit entity)
        {
            return entity == null ? null :
                new ProductUnitM()
                {
                    Id = entity.Id,
                    ProductSKU = entity.ProductSKU,
                    PriceBuy = entity.PriceBuy,
                    PriceSale = entity.PriceSale,
                    ProductId = entity.ProductId,
                    UnitId = entity.UnitId,
                    Multiplier = entity.Multiplier,
                };
        }

        public static ProductUnit FromModel(ProductUnitM model)
        {
            return model == null ? null :
                new ProductUnit()
                {
                    Id = model.Id,
                    ProductSKU = model.ProductSKU,
                    PriceBuy = model.PriceBuy,
                    PriceSale = model.PriceSale,
                    ProductId = model.ProductId,
                    UnitId = (byte)model.UnitId,
                    Multiplier = model.Multiplier,
                };
        }
    }
}
